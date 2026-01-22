using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class RoomView : Activateable
{
    public const string PLAYER_COUNT_TEXT_FORMAT = "{0}/{1} Players";

    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private TextMeshProUGUI roomNameText;
    [SerializeField]
    private TextMeshProUGUI playerCountText;
    [SerializeField]
    private TextMeshProUGUI gameModeName;
    [SerializeField]
    private UILatency uiLatency;
    [SerializeField]
    private Image roomIcon;
    [SerializeField]
    private Image gameModeIcon;

    private UnityAction<string> onJoinRoomClicked;
    private string roomName;

    public override void OnActivate()
    {
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
    }

    public override void OnDeactivate()
    {
        joinRoomButton.onClick.RemoveListener(OnJoinRoomClicked);
    }

    public void Initialize(string roomName, int latency, int playerCount, int maxPlayerCount,
        UnityAction<string> onJoinRoomClicked)
    {
        uiLatency.UpdateLatency(latency);
        this.roomName = roomName;
        roomNameText.text = roomName;
        playerCountText.text = string.Format(PLAYER_COUNT_TEXT_FORMAT, playerCount, maxPlayerCount);
        this.onJoinRoomClicked = onJoinRoomClicked;
    }

    private void OnJoinRoomClicked()
    {
        onJoinRoomClicked?.Invoke(roomName);
    }
}
