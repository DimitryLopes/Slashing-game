using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomView : MonoBehaviour
{
    private const string ROOM_NAME_FORMAT = "{0}'s room";
    private const string PLAYER_COUNT_FORMAT = "{0}/{1}";

    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private TextMeshProUGUI roomNameText;
    [SerializeField]
    private TextMeshProUGUI playerCountText;

    public void Initialize(string roomName, int playerCount, int maxPlayers, UnityEngine.Events.UnityAction onJoinRoomClicked)
    {
        roomNameText.text = string.Format(ROOM_NAME_FORMAT, roomName);
        playerCountText.text = string.Format(PLAYER_COUNT_FORMAT, playerCount,maxPlayers);
        joinRoomButton.onClick.AddListener(onJoinRoomClicked);
    }
}
