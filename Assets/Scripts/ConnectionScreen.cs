using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionScreen : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button createRoomButton;
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField]
    private TMP_Text statusText;
    [SerializeField]
    private Transform roomListContainer;
    [SerializeField]
    private Button roomButtonPrefab;
    [SerializeField]
    private GameObject loginContainer;
    [SerializeField]
    private GameObject lobbyContainer;

    private const string GameVersion = "1.0";
    private List<RoomInfo> availableRooms = new List<RoomInfo>();

    private void Start()
    {
        statusText.text = "Digite seu nome e entre em uma sala";
        connectButton.onClick.AddListener(OnJoinClicked);
        createRoomButton.onClick.AddListener(CreateRoom);
    }

    public void OnJoinClicked()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            statusText.text = "Nome inválido";
            return;
        }

        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.GameVersion = GameVersion;

        statusText.text = "Conectando ao Photon...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Conectado! Entrando no lobby...";
        loginContainer.SetActive(false);
        lobbyContainer.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(playerNameInput.text + "'s Room", new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "No lobby. Carregando salas...";
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;
        UpdateRoomListUI();
    }

    private void UpdateRoomListUI()
    {
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var room in availableRooms)
        {
            if (!room.IsOpen || !room.IsVisible) continue;

            Button roomButton = Instantiate(roomButtonPrefab, roomListContainer);
            roomButton.GetComponentInChildren<TMP_Text>().text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
            roomButton.onClick.AddListener(() => JoinRoom(room.Name));
        }
    }

    private void JoinRoom(string roomName)
    {
        statusText.text = $"Entrando na sala {roomName}...";
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = $"Entrou na sala! Jogadores: {PhotonNetwork.CurrentRoom.PlayerCount}";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Falha ao entrar na sala: {message}";
    }
}
