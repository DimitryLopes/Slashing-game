using Photon.Realtime;
using UnityEngine.Events;

public class RoomScreenController : ScreenController
{
    public readonly UnityAction OnLeaveRoomButtonClicked;
    public readonly UnityAction OnReadyButtonClicked;
    public readonly UnityAction OnStartGameButtonClicked;
    public readonly UnityAction<Player> OnKickPlayerButtonClicked;

    public float MaxPlayersInRoom => Constants.Networking.MAX_PLAYERS_IN_ROOM;

    public RoomScreenController(
        UnityAction onLeaveRoomButtonClicked,
        UnityAction onReadyButtonClicked,
        UnityAction onStartGameButtonClicked,
        UnityAction<Player> onKickPlayerButtonClicked
    )
    {
        OnLeaveRoomButtonClicked = onLeaveRoomButtonClicked;
        OnReadyButtonClicked = onReadyButtonClicked;
        OnStartGameButtonClicked = onStartGameButtonClicked;
        OnKickPlayerButtonClicked = onKickPlayerButtonClicked;
    }
}
