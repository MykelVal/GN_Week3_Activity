using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class InRoomPanel : MonoBehaviour
{
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _leaveRoom;

    private void OnEnable()
    {
        _startGame.onClick.AddListener(StartGame);
        _leaveRoom.onClick.AddListener(LeaveRoom);
    }

    private void OnDisable()
    {
        _startGame.onClick.RemoveListener(StartGame);
        _leaveRoom.onClick.RemoveListener(LeaveRoom);
    }

    private void StartGame()
    {
        Debug.Log("Game has been started");
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void Update()
    {
        _startGame.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
    }
}
