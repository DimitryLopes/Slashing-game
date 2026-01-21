using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : UIScreen<RoomScreenController>
{
    private const string PLAYERS_COUNT_FORMAT = "Players Ready: <color=#FFFFFF>{0}/{1}</color>";
    private const string MAX_PLAYERS_COUNT_FORMAT = "Players Ready: <color=#00FFFF>{0}/{1}</color>";

    [SerializeField]
    private TextMeshProUGUI roomNameText;
    [SerializeField]
    private TextMeshProUGUI playersReadyText;
    [SerializeField]
    private Button readyButton;
    [SerializeField]
    private Button leaveRoomButton;
    [SerializeField]
    private Button startGameButton;

    [SerializeField]
    private List<PlayerView> playerViews;

    private bool IsLocalPlayerMaster => PhotonNetwork.CurrentRoom.masterClientId == PhotonNetwork.LocalPlayer.ActorNumber;

    override protected void OnBeforeShow()
    {
        base.OnBeforeShow();
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        EventManager.OnPlayerJoinedRoomEvent.AddListener(OnPlayerJoined);
        EventManager.OnPlayerReadyStatusChanged.AddListener(OnPlayerReadyStatusChanged);
        EventManager.OnPlayerLeftRoomEvent.AddListener(OnPlayerLeft);
        leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);

        UpdateBottomButtons();

        if (IsFirstShow)
        {
            foreach (PlayerView view in playerViews)
            {
                view.Setup(Controller.OnKickPlayerButtonClicked);
            }
        }

        ClearPlayerViews();

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            UpdateAvailableView(player);
        }
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        EventManager.OnPlayerJoinedRoomEvent.RemoveListener(OnPlayerJoined);
        EventManager.OnPlayerLeftRoomEvent.RemoveListener(OnPlayerLeft);
        EventManager.OnPlayerReadyStatusChanged.RemoveListener(OnPlayerReadyStatusChanged);
        leaveRoomButton.onClick.RemoveListener(OnLeaveRoomButtonClicked);
        startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        readyButton.onClick.RemoveListener(OnReadyButtonClicked);
    }

    private void UpdatePlayersReadyText()
    {
        int playersReadyCount = 0;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if ((bool)player.CustomProperties[Constants.Networking.PLAYER_READY])
            {
                playersReadyCount++;
            }
        }
        string format = playersReadyCount == PhotonNetwork.CurrentRoom.PlayerCount ? MAX_PLAYERS_COUNT_FORMAT : PLAYERS_COUNT_FORMAT;

        playersReadyText.text = string.Format(format, playersReadyCount, PhotonNetwork.CurrentRoom.PlayerCount);

    }

    #region Button Callbacks
    private void OnLeaveRoomButtonClicked()
    {
        Controller.OnLeaveRoomButtonClicked.Invoke();
    }

    private void OnReadyButtonClicked()
    {
        Controller.OnReadyButtonClicked.Invoke();
    }

    private void OnStartGameButtonClicked()
    {
        Controller.OnStartGameButtonClicked.Invoke();
    }
    #endregion

    private void UpdateBottomButtons()
    {
        startGameButton.gameObject.SetActive(IsLocalPlayerMaster);
    }

    #region Player Management

    private void UpdateAvailableView(Player player)
    {
        var playerView = GetAvailablePlayerView();
        UpdatePlayerView(playerView, player);
    }

    private void UpdatePlayerView(PlayerView view, Player player)
    {
        view.SetPlayer(player, IsLocalPlayerMaster);
    }

    private void OnPlayerReadyStatusChanged(Player changedPlayer)
    {
        bool isEveryoneReady = true;

        foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            isEveryoneReady &= (bool)player.CustomProperties[Constants.Networking.PLAYER_READY];
        }

        foreach(PlayerView pv in playerViews)
        {
            if (pv.IsOccupied && pv.Player == changedPlayer)
            {
                pv.ChangePlayerStatus();
                continue;
            }
        }

        UpdatePlayersReadyText();
        startGameButton.interactable = isEveryoneReady;
    }

    private void OnPlayerJoined(RoomInfo info, Player player)
    {
        UpdateAvailableView(player);
        UpdatePlayersReadyText();
    }

    private void OnPlayerLeft(Player playerWhoLeft)
    {
        ClearPlayerViews();

        foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            UpdateAvailableView(player);
        }

        UpdateBottomButtons();

        UpdatePlayersReadyText();
    }

    private void ClearPlayerViews()
    {
        foreach (PlayerView view in playerViews)
        {
            view.Clear();
        }
    }

    private PlayerView GetAvailablePlayerView()
    {
        foreach (PlayerView pv in playerViews)
        {
            if (!pv.IsOccupied)
            {
                return pv;
            }
        }
        return null;
    }
    #endregion
}
