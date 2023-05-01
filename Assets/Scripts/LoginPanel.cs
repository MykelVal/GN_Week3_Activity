using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _userName;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] private Button _login;

    public event Action<string, string> LoginEvent;

    private void OnEnable()
    {
        _login.onClick.AddListener(Login);
    }

    private void OnDisable()
    {
        _login.onClick.RemoveListener(Login);
    }


    private void Login()
    {
        if(string.IsNullOrEmpty(_userName.text ) || string.IsNullOrEmpty(_password.text))
        {
            return;
        }

        LoginEvent?.Invoke(_userName.text, _password.text);

        _password.text = "";
    }
}
