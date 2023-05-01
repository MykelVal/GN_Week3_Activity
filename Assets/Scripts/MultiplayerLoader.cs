using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private Button _connectBtn;
    [SerializeField] private Button _playBtn;

    
    
    #region Unity Callbacks
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnEnable()
    {
        if(_connectBtn)
        {
            _connectBtn.onClick.AddListener(ConnectToPhoton);
        }

        if (_playBtn)
        {
            _playBtn.onClick.AddListener(JoinRandomRoom);
        }
    }

    public override void OnDisable()
    {
        if (_connectBtn)
        {
            _connectBtn.onClick.RemoveListener(ConnectToPhoton);
        }
        
        if (_playBtn)
        {
            _playBtn.onClick.RemoveListener(JoinRandomRoom);
        }
    }

    private void Start()
    {
        _playBtn.gameObject.SetActive(false);
        _connectBtn.gameObject.SetActive(true);
    }
    
    private void Update()
    {
        Debug.Log(GetMessage($"Is Connected and Ready: {PhotonNetwork.IsConnectedAndReady}"));
    }
    #endregion

    #region Photon Helpers
    private string GetMessage(string message)
    {
        return $"{nameof(MultiplayerLoader)}: {message}";
    }

    private void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "1";
        Debug.Log(GetMessage($"Connecting To Photon, {name}"));
    }

    private void JoinRandomRoom()
    {
        var name = _playerName.text;
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError(GetMessage("Name is Empty, please assign a name"));
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log(GetMessage("Joining Random Room"));
            PhotonNetwork.NickName = name;
            _playerName.text = string.Empty;
        } 
    }
    #endregion
    
    #region Connection
    public override void OnConnectedToMaster()
    {
        _connectBtn.gameObject.SetActive(false);
        _playBtn.gameObject.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(GetMessage("Successfully Connected to Lobby"));
    }

    public override void OnConnected()
    {
        Debug.Log(GetMessage("Successfully Connected to Server"));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError(GetMessage($"Discoonected due to {cause}"));
    }

    
    #endregion

    #region Room Logic
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log(GetMessage(nameof(OnCreatedRoom)));
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(GetMessage(nameof(OnJoinedRoom)));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(GetMessage(nameof(OnJoinRoomFailed)));
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(GetMessage(nameof(OnJoinRandomFailed)));
    }
    #endregion

}
