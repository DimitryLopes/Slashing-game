using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public const int ROOM_REFRESH_INTERVAL = 5;
    public const string ROOM_NAME_SUFIX = "'s Room";

    public static NetworkManager Instance { get; private set; }
    private float RoomRefreshTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (RoomRefreshTimer < ROOM_REFRESH_INTERVAL)
        {
            RoomRefreshTimer += Time.deltaTime;
        }
    }

    public void ShowLobbyScreen()
    {
        var controller = new LobbyScreenController(JoinRoom, LeaveRoom, CreateRoom, RefreshRooms);
        ScreenManager.Instance.Show<LobbyScreen>(controller);
    }

    public void Connect(string playerName)
    {
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "don't let this make into the final build"; //TODO: remove hardcoded version
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        EventManager.OnConnectedToMasterEvent?.Invoke();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.CreateCustomProperties(PhotonNetwork.GetPing(), false);
        EventManager.OnLobbyJoinedEvent.Invoke();
        PhotonNetwork.NickName = PhotonNetwork.LocalPlayer.UserId;
        StateManager.Instance.ChangeState(GameState.Lobby);
    }

    public void CreateRoom()
    {
        string roomName = PhotonNetwork.NickName + ROOM_NAME_SUFIX;
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable
        {
            { Constants.Networking.ROOM_NAME, roomName },
            { Constants.Networking.ROOM_IS_PLAYING, false }
        };        

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = Constants.Networking.MAX_PLAYERS_IN_ROOM,
            IsOpen = true,
            IsVisible = true,
            CustomRoomProperties = table,
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void RefreshRooms()
    {
        PhotonNetwork.JoinLobby();
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    #region Room Screen Callbacks
    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void ToggleReady()
    {
        Player player = PhotonNetwork.LocalPlayer;
        bool isReady = (bool)player.CustomProperties[Constants.Networking.PLAYER_READY];
        player.SetCustomProperty(Constants.Networking.PLAYER_READY, !isReady);
    }

    private void StartGame() 
    {
        var room = PhotonNetwork.CurrentRoom;
        room.SetCustomProperty(Constants.Networking.ROOM_IS_PLAYING, true);
        room.IsOpen = false;
    }

    private void KickPlayer(Player player)
    {
        PhotonNetwork.CloseConnection(player);
    }
    #endregion

    private void LoadGameScene()
    {
        PhotonNetwork.LoadLevel(Constants.Scenes.GAME);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey(Constants.Networking.PLAYER_READY))
        {
            EventManager.OnPlayerReadyStatusChanged?.Invoke(targetPlayer);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(RoomRefreshTimer >= ROOM_REFRESH_INTERVAL)
        {
            RoomRefreshTimer = 0f;
        }
        EventManager.OnRoomListUpdateEvent?.Invoke(roomList);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        PhotonNetwork.LocalPlayer.SetCustomProperty(Constants.Networking.PLAYER_READY, false);
        EventManager.OnPlayerLeftRoomEvent.Invoke(otherPlayer);
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        EventManager.OnPlayerJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom, otherPlayer);
    }

    public override void OnJoinedRoom()
    {
        StateManager.Instance.ChangeState(GameState.Room);
    }

    public void ShowRoomScreen()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        float ping = PhotonNetwork.GetPing();
        localPlayer.SetCustomProperty(Constants.Networking.PLAYER_PING, ping);

        RoomScreenController controller = new RoomScreenController(
            LeaveRoom,
            ToggleReady,
            StartGame,
            KickPlayer);

        ScreenManager.Instance.Show<RoomScreen>(controller);

        EventManager.OnJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey(Constants.Networking.ROOM_IS_PLAYING))
        {
            bool isPlaying = (bool)propertiesThatChanged[Constants.Networking.ROOM_IS_PLAYING];
            if (isPlaying)
            {
                LoadGameScene();
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Falha ao entrar na sala: {message}");
        //EventManager.OnJoinRoomFailedEvent?.Invoke($"Falha ao entrar na sala: {message}");
    }
}