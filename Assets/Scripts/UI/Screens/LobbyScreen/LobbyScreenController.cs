using System;
using UnityEngine.Events;

public class LobbyScreenController : ScreenController
{
    public readonly UnityAction<string> OnJoinClicked;
    public readonly Action OnLeaveClicked;
    public readonly Action OnCreateRoomButtonClicked;
    public readonly Action OnRefreshButtonClicked;
  
    public LobbyScreenController(
        UnityAction<string> onJoinClicked,
        Action onLeaveClicked,
        Action onCreateClickedRoom,
        Action onRefreshButtonClicked)
    {
        OnJoinClicked = onJoinClicked;
        OnLeaveClicked = onLeaveClicked;
        OnCreateRoomButtonClicked = onCreateClickedRoom;
        OnRefreshButtonClicked = onRefreshButtonClicked;
    }
}