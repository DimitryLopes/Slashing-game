using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomScreenController : ScreenController
{
    public readonly string ScreenName;
    public readonly UnityAction OnLeaveRoomButtonClicked;
    public readonly UnityAction OnReadyButtonClicked;
    public readonly UnityAction OnStartGameButtonClicked;
    public readonly UnityAction<Player> OnKickPlayerButtonClicked;

    public float MaxPlayersInRoom => Constants.Networking.MAX_PLAYERS_IN_ROOM;

    public RoomScreenController(string screenName,
        UnityAction onLeaveRoomButtonClicked,
        UnityAction onReadyButtonClicked,
        UnityAction onStartGameButtonClicked,
        UnityAction<Player> onKickPlayerButtonClicked
    )
    {
        ScreenName = screenName;
        OnLeaveRoomButtonClicked = onLeaveRoomButtonClicked;
        OnReadyButtonClicked = onReadyButtonClicked;
        OnStartGameButtonClicked = onStartGameButtonClicked;
        OnKickPlayerButtonClicked = onKickPlayerButtonClicked;
    }
}
