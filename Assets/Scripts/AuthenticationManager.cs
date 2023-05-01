using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] private LoginPanel _login;
    [SerializeField] private RegistrationPanel _registration;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private Text _errorText;

    public string PlayFabId { get; private set; }

    private void OnEnable()
    {
        _login.LoginEvent += OnLoginCalled;
        _registration.RegisterEvent += OnRegisterCalled;
    }

    private void OnDisable()
    {

        _login.LoginEvent -= OnLoginCalled;
        _registration.RegisterEvent -= OnRegisterCalled;
    }

    private void OnLoginCalled(string userName , string password )
    {
        PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest()
                {
                    Email = userName,
                    Password = password,
                    TitleId = PlayFabSettings.TitleId
                    
                },(loginResult) => {
                    Debug.Log("Successfully Logged in");
                    PhotonNetwork.NickName = userName;
                    SceneManager.LoadScene(1);
                },(error) => {
                    Debug.LogError($"Failed to login: {error.ErrorMessage}");
                    _errorPanel.SetActive(true);
                    _errorText.text = $"Failed to login: {error.ErrorMessage}";
                }
            );
    }

    private void OnRegisterCalled(string userName, string password)
    {
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                TitleId = PlayFabSettings.TitleId
            }, (loginSuccess) =>
            {
                PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
                {
                    Email = userName,
                    Password = password,
                    Username = loginSuccess.PlayFabId
                }, (updateSuccess) =>
                    {
                        Debug.Log("Sucessfully Registered");
                    }, (updateFail) =>
                {
                    var msg = "";
                        foreach (var VARIABLE in updateFail.ErrorDetails)
                        {
                            msg += VARIABLE.Key + "\n";
                            foreach (var item in VARIABLE.Value)
                            {
                                msg += $"#{item}";
                            }
                        }
                        Debug.LogError($"Failed to Register: {updateFail.Error}\n {updateFail.ErrorMessage} \n {msg}");
                    _errorPanel.SetActive(true);
                    _errorText.text = $"Failed to Register: {updateFail.Error}\n {updateFail.ErrorMessage} \n {msg}";
            });

            }, (loginFailure) =>
            {
                Debug.LogError($"Unable to Login with custom Id: {loginFailure.ErrorMessage}");
            });
    }

    private void InitializeConfigValues(GetPlayerCombinedInfoResultPayload payload)
    {
        if (payload.AccountInfo != null)
        {
            PlayFabId = payload.AccountInfo.PlayFabId;
        }
    }
}
