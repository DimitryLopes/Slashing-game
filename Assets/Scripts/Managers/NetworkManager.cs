using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }
    public const int ROOM_REFRESH_INTERVAL = 5;
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
        EventManager.OnLobbyJoinedEvent.Invoke();
        ShowLobbyScreen();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "'s Room", new RoomOptions
        {
            MaxPlayers = Constants.Networking.MAX_PLAYERS_IN_ROOM,
            IsOpen = true,
            IsVisible = true,
            CustomRoomProperties = {{ Constants.Networking.ROOM_NAME, name }},
        });
    }

    private void RefreshRooms()
    {
        PhotonNetwork.JoinLobby();
    }

    private void JoinRoom(string roomName)
    {   
        PhotonNetwork.LocalPlayer.CreateCustomProperties(PhotonNetwork.GetPing(), false);
        PhotonNetwork.JoinRoom(roomName);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void LoadGameScene()
    {
        PhotonNetwork.LoadLevel(Constants.Scenes.GAME);
    }

    private void ToggleReady()
    {
        Player player = PhotonNetwork.LocalPlayer;
        bool isReady = (bool)player.CustomProperties[Constants.Networking.PLAYER_READY];
        player.SetCustomProperty(Constants.Networking.PLAYER_READY, !isReady);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey(Constants.Networking.PLAYER_READY))
        {
            EventManager.OnPlayerReadyStatusChanged?.Invoke(targetPlayer);
        }
    }

    private void KickPlayer(Player player)
    {
        PhotonNetwork.CloseConnection(player);
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
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        EventManager.OnPlayerJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom, otherPlayer);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(Constants.Scenes.GAME);
        }
    }

    public override void OnJoinedRoom()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        float ping = PhotonNetwork.GetPing();
        localPlayer.SetCustomProperty(Constants.Networking.PLAYER_PING, ping);
        string name = PhotonNetwork.CurrentRoom.GetRoomName();

        RoomScreenController controller = new RoomScreenController(
            name, LeaveRoom, ToggleReady, LoadGameScene, KickPlayer);
            
        ScreenManager.Instance.Show<RoomScreen>(controller);

        EventManager.OnJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //EventManager.OnJoinRoomFailedEvent?.Invoke($"Falha ao entrar na sala: {message}");
    }
}