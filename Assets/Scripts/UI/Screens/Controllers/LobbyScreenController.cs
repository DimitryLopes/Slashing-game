using System;

public class LobbyScreenController : ScreenController
{
    public readonly Action<string> OnJoinClicked;
    public readonly Action OnLeaveClicked;
    public readonly Action OnCreateClickedRoom;
  
    public LobbyScreenController(
        Action<string> onJoinClicked,
        Action onLeaveClicked,
        Action onCreateClickedRoom)
    {
        OnJoinClicked = onJoinClicked;
        OnLeaveClicked = onLeaveClicked;
        OnCreateClickedRoom = onCreateClickedRoom;
    }
}