using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _userName;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] private TMP_InputField _retypePassword;
    [SerializeField] private Button _registerLogin;

    public event Action<string, string> RegisterEvent;

    private void OnEnable()
    {
        _registerLogin.onClick.AddListener(Register);
    }

    private void OnDisable()
    {
        _registerLogin.onClick.RemoveListener(Register);
    }


    private void Register()
    {
        if (string.IsNullOrEmpty(_userName.text) || string.IsNullOrEmpty(_password.text) || string.IsNullOrEmpty(_retypePassword.text))
        {
            return;
        }

        if (!_password.text.Equals(_retypePassword.text))
        {

            _password.text = "";
            _retypePassword.text = "";
            Debug.LogError("passwords do not match");
            return;
        }

        RegisterEvent?.Invoke(_userName.text, _password.text);

        _password.text = "";
        _retypePassword.text = "";
    }

}
