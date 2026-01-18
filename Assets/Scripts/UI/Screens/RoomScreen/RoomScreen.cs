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
    private Button leaveRoomButton;
    [SerializeField]
    private Button readyButton;
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
        readyButton.onClick.AddListener(OnReadyButtonClicked);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        EventManager.OnPlayerJoinedRoomEvent -= OnPlayerJoined;
        EventManager.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;
        leaveRoomButton.onClick.RemoveListener(OnLeaveRoomButtonClicked);
        readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
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

    #region Player Views Management
    private void OnPlayerJoined(RoomInfo info, Player player)
    {
        for (int i = 0; i < playerViews.Count; i++)
        {
            PlayerView pv = playerViews[i];
            if (!pv.IsOccupied)
            {
                pv.SetPlayer(player);
                break;
            }
        }
        UpdatePlayersCountText();
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

        readyButton.interactable = isEveryoneReady;
    }

    private void OnPlayerLeft(RoomInfo info, Player player)
    {
        for (int i = 0; i < playerViews.Count; i++)
        {
            PlayerView pv = playerViews[i];
            if (pv.IsOccupied && pv.Player == player)
            {
                pv.Clear();
                break;
            }
        }
        UpdatePlayersCountText();
    }
    #endregion
}
