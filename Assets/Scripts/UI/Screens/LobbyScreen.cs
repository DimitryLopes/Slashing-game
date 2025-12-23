using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : UIScreen<LobbyScreenController>
{
    [SerializeField]
    private Button createRoomButton;
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField]
    private TextMeshProUGUI statusText;
    [SerializeField]
    private Transform roomListContainer;
    [SerializeField]
    private Button roomButtonPrefab;
    [SerializeField]
    private GameObject loginContainer;
    [SerializeField]
    private GameObject lobbyContainer;

    private List<RoomInfo> availableRooms = new List<RoomInfo>();

    private void Start()
    {
        statusText.text = "Digite seu nome e entre em uma sala";
    }

    protected override void OnBeforeShow()
    {
        EventManager.OnRoomListUpdateEvent += OnRoomListUpdate;
        EventManager.OnPlayerJoinedRoomEvent += OnPlayerJoinedRoom;
        EventManager.OnJoinedRoomEvent += OnJoinedRoom;
        EventManager.OnConnectedToMasterEvent += OnConnectedToMaster;

        connectButton.onClick.AddListener(OnConnectButtonClicked);
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
    }

    protected override void OnBeforeHide()
    {
        EventManager.OnRoomListUpdateEvent -= OnRoomListUpdate;
        EventManager.OnPlayerJoinedRoomEvent -= OnPlayerJoinedRoom;
        EventManager.OnJoinedRoomEvent -= OnJoinedRoom;
        EventManager.OnConnectedToMasterEvent -= OnConnectedToMaster;
    }

    private void OnConnectedToMaster()
    {
        lobbyContainer.SetActive(true);
        loginContainer.SetActive(false);
    }

    public void OnConnectButtonClicked()
    {
        Controller.OnConnectClicked.Invoke(statusText.text);
    }

    private void OnCreateRoomButtonClicked()
    {
        Controller.OnCreateClickedRoom.Invoke();
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"Atualização da lista de salas recebida. Total de salas: {roomList.Count}");
        availableRooms = roomList;
        UpdateRoomListUI();
    }

    private void JoinRoom(string roomName)
    {
        statusText.text = $"Entrando na sala {roomName}...";
        Controller.OnJoinClicked.Invoke(roomName);
    }

    //when another player joins
    private void OnPlayerJoinedRoom(RoomInfo roomInfo, Player player)
    {
        statusText.text = $"{player.NickName} entrou na sala! Jogadores: {roomInfo.PlayerCount}";
    }

    //when this player joins
    public void OnJoinedRoom(RoomInfo room)
    {
        statusText.text = $"Entrou na sala! Jogadores: {room.PlayerCount}";
        if (room.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(Constants.Scenes.GAME);
        }
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
}
