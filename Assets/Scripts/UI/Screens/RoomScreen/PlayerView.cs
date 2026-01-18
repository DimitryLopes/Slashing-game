using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Events;

public class PlayerView : Activateable
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;
    [SerializeField]
    private Button kickPlayerButton;
    [SerializeField]
    private GameObject readyContainer;
    [SerializeField]
    private GameObject notReadyContainer;
    [SerializeField]
    private GameObject waitingForPlayerContainer;
    [SerializeField]
    private GameObject playerContainer;
    [SerializeField]
    private UILatency latency;

    private Player associatedPlayer;
    private UnityAction<Player> onPlayerKickButtonPressed;
    public Player Player => associatedPlayer;

    public bool IsOccupied => associatedPlayer != null;

    public override void OnDeactivate()
    {
        associatedPlayer = null;
        UpdatePlayerView();
    }

    public void Setup(bool isLocalPlayer, bool isMasterClient, UnityAction kickPlayerAction)
    {
        bool shouldShowButton = !isLocalPlayer && isMasterClient;
        kickPlayerButton.gameObject.SetActive(shouldShowButton);
        UpdatePlayerView();
        if (!shouldShowButton) return;

        kickPlayerButton.onClick.RemoveAllListeners();
        kickPlayerButton.onClick.AddListener(OnPlayerKickButtonClick);
    }

    public void SetPlayer(Player player)
    {
        associatedPlayer = player;
        float ping = player.GetPing();
        latency.UpdateLatency(ping);
        UpdatePlayerView();
        playerNameText.text = player.NickName;
        ChangePlayerStatus(player);
    }

    public void ChangePlayerStatus(Player player)
    {
        bool isReady = (bool)player.CustomProperties[Constants.Networking.PLAYER_READY];
        readyContainer.SetActive(isReady);
        notReadyContainer.SetActive(!isReady);
    }

    public void Clear()
    {
        associatedPlayer = null;
        UpdatePlayerView();
    }

    private void UpdatePlayerView()
    {
        bool hasPlayer = associatedPlayer != null;
        waitingForPlayerContainer.gameObject.SetActive(!hasPlayer);
        playerContainer.gameObject.SetActive(hasPlayer);
    }

    private void OnPlayerKickButtonClick()
    {
        onPlayerKickButtonPressed.Invoke(associatedPlayer);
    }

}
