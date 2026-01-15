using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class RoomView : Activateable
{
    private const string ROOM_NAME_FORMAT = "{0}'s room";
    private const string LATENCY_FORMAT = "{0} ms";

    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private TextMeshProUGUI roomNameText;
    [SerializeField]
    private TextMeshProUGUI latencyText;
    [SerializeField]
    private TextMeshProUGUI gameModeName;
    [SerializeField]
    private Image latencyImageFill;
    [SerializeField]
    private Image roomIcon;
    [SerializeField]
    private Image gameModeIcon;

    public void Initialize(string roomName, int latency,
        UnityAction onJoinRoomClicked)
    {
        roomNameText.text = string.Format(ROOM_NAME_FORMAT, roomName);
        latencyText.text = string.Format(LATENCY_FORMAT, latency);
        joinRoomButton.onClick.AddListener(onJoinRoomClicked);
    }
}
