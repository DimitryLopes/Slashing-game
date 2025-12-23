using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

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

    private void Start()
    {
        var controller = new LobbyScreenController(Connect, JoinRoom, LeaveRoom, CreateRoom);
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

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "'s Room", new RoomOptions
        {
            MaxPlayers = 2,
            IsOpen = true,
            IsVisible = true
        });
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        EventManager.OnRoomListUpdateEvent?.Invoke(roomList);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        EventManager.OnPlayerJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom, player);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(Constants.Scenes.GAME);
        }
    }

    public override void OnJoinedRoom()
    {
        EventManager.OnJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom);
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(Constants.Scenes.GAME);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //EventManager.OnJoinRoomFailedEvent?.Invoke($"Falha ao entrar na sala: {message}");
    }
}