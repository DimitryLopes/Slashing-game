using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.Events;

public class RoomScreenController : ScreenController
{
    public readonly string ScreenName;
    public readonly UnityAction OnLeaveRoomButtonClicked;
    public readonly UnityAction OnReadyButtonClicked;
    public readonly UnityAction OnStartGameButtonClicked;
    public readonly UnityAction<Player> OnKickPlayerButtonClicked;
    public readonly Player LocalPlayer;

    public List<Player> PlayersInRoom { get; set; } = new List<Player>();
    public Player MasterClient { get; set; }
    public string UserID => LocalPlayer.UserId;
    public float MaxPlayersInRoom => Constants.Networking.MAX_PLAYERS_IN_ROOM;

    public RoomScreenController(
        Room roomInfo,
        UnityAction onLeaveRoomButtonClicked,
        UnityAction onReadyButtonClicked,
        UnityAction onStartGameButtonClicked,
        UnityAction<Player> onKickPlayerButtonClicked
    )
    {
        ScreenName = roomInfo.GetRoomName();
        MasterClient = roomInfo.Players[roomInfo.masterClientId];
        OnLeaveRoomButtonClicked = onLeaveRoomButtonClicked;
        OnReadyButtonClicked = onReadyButtonClicked;
        OnStartGameButtonClicked = onStartGameButtonClicked;
        OnKickPlayerButtonClicked = onKickPlayerButtonClicked;
        PlayersInRoom = new List<Player>(roomInfo.Players.Values);
    }
}
