// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
	#pragma warning disable 649

    /// <summary>
    /// Launch manager. Connect, join a random room or create one if none or all full.
    /// </summary>
	public class Launcher : MonoBehaviourPunCallbacks
    {

		#region Private Serializable Fields

		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		[SerializeField]
		private GameObject controlPanel;

		[Tooltip("The Ui Text to inform the user about the connection progress")]
		[SerializeField]
		private Text feedbackText;

		[Tooltip("The maximum number of players per room")]
		[SerializeField]
		private byte maxPlayersPerRoom = 4;

		[Tooltip("The UI Loader Anime")]
		[SerializeField]
		private LoaderAnime loaderAnime;

		[SerializeField] private Text _roomStatistics;

		[SerializeField] private RoomInfoContainer _roomContainerPrefab;
		[SerializeField] private List<RoomInfoContainer> _roomContainerList;
		[SerializeField] private Transform _lobbyContainerParent;
		
		[SerializeField] private CreateRoomPanel _createRoomPanel;

		[SerializeField] private InRoomPanel _inRoomPanel;

		#endregion

		#region Private Fields
		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
		/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		/// <summary>
		/// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
		/// </summary>
		string gameVersion = "1";

		#endregion

		#region MonoBehaviour CallBacks

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		void Awake()
		{
			if (loaderAnime==null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.",this);
			}

			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.AutomaticallySyncScene = true;

		}

		private void Start()
		{
			_lobbyContainerParent.gameObject.SetActive(false);	
			_createRoomPanel.gameObject.SetActive(false);
			_inRoomPanel.gameObject.SetActive(false);
			
			_roomContainerList = new List<RoomInfoContainer>();
			for (var i = 0; i < 20; i++)
			{
				var container = Instantiate(_roomContainerPrefab, _lobbyContainerParent);
				_roomContainerList.Add(container);
				container.gameObject.SetActive(false);
			}
			Connect();
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Start the connection process. 
		/// - If already connected, we attempt joining a random room
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect()
		{
			// we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
			feedbackText.text = "";

			// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = true;

			// hide the Play button for visual consistency
			controlPanel.SetActive(false);

			// start the loader animation for visual effect.
			if (loaderAnime!=null)
			{
				loaderAnime.StartLoaderAnimation();
			}

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.IsConnected)
			{
				LogFeedback("Joining Room...");
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom();
			}else{

				LogFeedback("Connecting...");
				
				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
			}
		}

		/// <summary>
		/// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
		/// </summary>
		/// <param name="message">Message.</param>
		void LogFeedback(string message)
		{
			// we do not assume there is a feedbackText defined.
			if (feedbackText == null) {
				return;
			}

			// add new messages as a new line and at the bottom of the log.
			feedbackText.text += System.Environment.NewLine+message;
		}

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks
        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster()
		{
            // we don't want to do anything if we are not attempting to join a room. 
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
				Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
			}
			
			_lobbyContainerParent.gameObject.SetActive(true);
			_createRoomPanel.gameObject.SetActive(true);

			PhotonNetwork.JoinLobby();
		}

        /// <summary>
		/// Called after disconnecting from the Photon server.
		/// </summary>
		public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>OnDisconnected</Color> "+cause);
			Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

			// #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
			loaderAnime.StopLoaderAnimation();

			isConnecting = false;
			controlPanel.SetActive(true);

		}
        #endregion

        #region Lobby Room Callbacks
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
	        UpdateRoomsInLobby(roomList);
        }

        private void UpdateRoomsInLobby(List<RoomInfo> roomList)
        {
	        foreach (var roomContainer in _roomContainerList)
	        {
		        roomContainer.gameObject.SetActive(false);
	        }
	        
	        for (int i = 0; i < roomList.Count; i++)
	        {
		        Debug.Log($"Found Room {roomList[i].Name}");
		        if (roomList[i].RemovedFromList || !roomList[i].IsVisible || !roomList[i].IsOpen) continue;
		        
		        _roomContainerList[i].gameObject.SetActive(true);
		        _roomContainerList[i].UpdateRoomInfo(roomList[i]);
	        }
        }

        public override void OnJoinedLobby()
        {
	        _lobbyContainerParent.gameObject.SetActive(true);
        }

        public override void OnLeftLobby()
        {
	        _lobbyContainerParent.gameObject.SetActive(false);
        }

        public override void OnJoinedRoom()
        {
	        _lobbyContainerParent.gameObject.SetActive(false);
	        _createRoomPanel.gameObject.SetActive(false);
	        _inRoomPanel.gameObject.SetActive(true);
	        UpdateRoomStatistics();
        }

        public override void OnLeftRoom()
        {
	        _lobbyContainerParent.gameObject.SetActive(true);
	        _createRoomPanel.gameObject.SetActive(true);
	        _inRoomPanel.gameObject.SetActive(false);
	        UpdateRoomStatistics();
        }

        public override void OnCreatedRoom()
        {
	        _lobbyContainerParent.gameObject.SetActive(false);
	        _createRoomPanel.gameObject.SetActive(false);
	        UpdateRoomStatistics();
        }

        #endregion

		#region In Room Callbacks

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			UpdateRoomStatistics();	
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			UpdateRoomStatistics();
		}
		
		private void UpdateRoomStatistics()
		{
			var currentRoom = PhotonNetwork.CurrentRoom;
			if (currentRoom == null)
			{
				_roomStatistics.text = "";
				return;
			}
			
			_roomStatistics.text =
				$"Name: {currentRoom.Name} \n  {currentRoom.PlayerCount}/{currentRoom.MaxPlayers} \n";

			foreach (var player in PhotonNetwork.CurrentRoom.Players)
			{
				_roomStatistics.text += $"\n #{player.Key} | {player.Value.NickName}";
			}
		}

		#endregion
		
	}
}