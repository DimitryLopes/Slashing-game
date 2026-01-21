using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class RoomView : Activateable
{
    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private TextMeshProUGUI roomNameText;
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

    public void Initialize(string roomName, int latency,
        UnityAction<string> onJoinRoomClicked)
    {
        uiLatency.UpdateLatency(latency);
        this.roomName = roomName;
        roomNameText.text = roomName;
        this.onJoinRoomClicked = onJoinRoomClicked;
    }

    private void OnJoinRoomClicked()
    {
        onJoinRoomClicked?.Invoke(roomName);
    }
}
