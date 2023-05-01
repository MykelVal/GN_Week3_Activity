using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoContainer : MonoBehaviour
{
    [SerializeField] private Text _roomStats;
    [SerializeField] private Button _connectBtn;

    private RoomInfo info;

    public void UpdateRoomInfo(RoomInfo info)
    {
        _roomStats.text = $"{info.Name} | {info.PlayerCount}/{info.MaxPlayers}";
        _connectBtn.onClick.RemoveAllListeners();
        _connectBtn.onClick.AddListener(() => { PhotonNetwork.JoinRoom(info.Name); });
    }
}
