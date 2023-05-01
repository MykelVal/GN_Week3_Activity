using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : MonoBehaviour
{
    [SerializeField] private InputField _roomName;
    [SerializeField] private Button _createBtn;

    private void OnEnable()
    {
        _createBtn.onClick.AddListener(CreateRoom);        
    }

    private void OnDisable()
    {
        _roomName.text = "";
        _createBtn.onClick.RemoveAllListeners();
    }

    private void CreateRoom()
    {
        if (!string.IsNullOrEmpty(_roomName.text))
        {
            PhotonNetwork.CreateRoom(_roomName.text, new RoomOptions() {MaxPlayers = 4});
        }
    }
}
