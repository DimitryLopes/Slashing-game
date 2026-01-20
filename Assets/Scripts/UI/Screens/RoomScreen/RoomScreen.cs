using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
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

    private byte playerCount;

    override protected void OnBeforeShow()
    {
        base.OnBeforeShow();
        roomNameText.text = Controller.ScreenName;
        EventManager.OnPlayerJoinedRoomEvent += OnPlayerJoined;
        EventManager.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;
        leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);

        if (IsFirstShow)
        {
            foreach (PlayerView view in playerViews)
            {
                view.Setup(Controller.OnKickPlayerButtonClicked);
            }
        }

        foreach (PlayerView view in playerViews)
        {
            view.Clear();
        }

        foreach (Player player in Controller.PlayersInRoom)
        {
            UpdateAvailableView(player);
        }
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        EventManager.OnPlayerJoinedRoomEvent -= OnPlayerJoined;
        EventManager.OnPlayerLeftRoomEvent -= OnPlayerLeft;
        EventManager.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;
        leaveRoomButton.onClick.RemoveListener(OnLeaveRoomButtonClicked);
        startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        readyButton.onClick.RemoveListener(OnReadyButtonClicked);
    }

    private void UpdatePlayersCountText()
    {
        if(playerCount == Controller.MaxPlayersInRoom)
        {
            playersReadyText.text = string.Format(MAX_PLAYERS_COUNT_FORMAT, playerCount, Controller.MaxPlayersInRoom);
        }
        else
        {
            playersReadyText.text = string.Format(PLAYERS_COUNT_FORMAT, playerCount, Controller.MaxPlayersInRoom);
        }
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
        bool isMaster = Controller.MasterClient == Controller.LocalPlayer;
        startGameButton.gameObject.SetActive(isMaster);
        readyButton.gameObject.SetActive(isMaster);
    }

    #region Player Management

    private void UpdateAvailableView(Player player)
    {
        var playerView = GetAvailablePlayerView();
        UpdatePlayerView(playerView, player);
    }

    private void UpdatePlayerView(PlayerView view, Player player)
    {
        if (player == null)
        {
            if (view.Player.UserId == player.UserId)
            {
                view.Clear();
                return;
            }
            else if (view.Player != null)
            {
                Debug.LogError($"Attempted to clear {view.name}. It doesn't  belong to {player.NickName}. It belongs to {view.Player.NickName}");
            }
            else
            {
                Debug.LogError($"Attempted to clear {view.name}. It is already empty.");
            }
            return;
        }

        view.SetPlayer(player, player.IsLocal, player.IsMasterClient);
    }

    private void UpdatePlayerView(Player player)
    {
        foreach(PlayerView pv in playerViews)
        {
            if (pv.IsOccupied && pv.Player == player)
            {
                pv.SetPlayer(player, player == Controller.LocalPlayer, player.IsMasterClient);
                break;
            }
        }
    }

    private void OnPlayerReadyStatusChanged(Player player)
    {
        bool isEveryoneReady = true;
        foreach(PlayerView pv in playerViews)
        {
            isEveryoneReady &= pv.IsOccupied && (bool)pv.Player.CustomProperties[Constants.Networking.PLAYER_READY];
            if (pv.IsOccupied && pv.Player == player)
            {
                pv.ChangePlayerStatus(player);
                continue;
            }
        }

        UpdatePlayersCountText();
        startGameButton.interactable = isEveryoneReady;
    }

    private void OnMasterClientChanged(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            Controller.MasterClient = Controller.LocalPlayer;
            UpdateBottomButtons();
        }

        UpdatePlayerView(newMasterClient);
    }

    private void OnPlayerJoined(RoomInfo info, Player player)
    {
        UpdateAvailableView(player);
        Controller.PlayersInRoom.Add(player);
        UpdatePlayersCountText();
    }

    private void OnPlayerLeft(RoomInfo info, Player player)
    {
        foreach (PlayerView pv in playerViews)
        {
            if (pv.IsOccupied && pv.Player == player)
            {
                pv.Clear();
                break;
            }
        }
        Controller.PlayersInRoom.Remove(player);

        if(Controller.MasterClient.UserId != player.UserId)
        {
            OnMasterClientChanged(player);
        }

        UpdatePlayersCountText();
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
