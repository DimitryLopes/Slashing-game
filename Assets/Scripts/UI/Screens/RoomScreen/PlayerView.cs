using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Events;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;
    [SerializeField]
    private GameObject masterClientIcon;
    [SerializeField]
    private Button kickPlayerButton;
    [SerializeField]
    private GameObject readyContainer;
    [SerializeField]
    private GameObject notReadyContainer;
    [SerializeField]
    private GameObject waitingForPlayerContainer;
    [SerializeField]
    private GameObject readyInfoContainer;
    [SerializeField]
    private GameObject playerContainer;
    [SerializeField]
    private UILatency latency;
    [SerializeField, Header("Swap Animation")]
    private Image swapAnimationContainer;
    [SerializeField]
    private RectTransform swapAnimationGlow;
    [SerializeField]
    private float swapGlowStartAnhchoredX = -6;
    [SerializeField]
    private float swapGlowFinishAnhchoredX = 78;
    [SerializeField]
    private float swapAnimationDuration = 0.5f;

    private Player associatedPlayer;
    private UnityAction<Player> onPlayerKickButtonPressed;
    public Player Player => associatedPlayer;

    public bool IsOccupied => associatedPlayer != null;

    public void Setup(UnityAction<Player> kickPlayerActionm)
    {
        onPlayerKickButtonPressed = kickPlayerActionm;

        UpdatePlayerView();
     
        kickPlayerButton.onClick.RemoveAllListeners();

        kickPlayerButton.onClick.AddListener(OnPlayerKickButtonClick);
    }

    public void SetPlayer(Player player, bool isLocalPlayer, bool isMasterClient)
    {
        bool shouldShowKickButton = !isLocalPlayer && isMasterClient;
        kickPlayerButton.gameObject.SetActive(shouldShowKickButton);
        masterClientIcon.SetActive(isMasterClient);

        associatedPlayer = player;
        float ping = player.GetPing();
        latency.UpdateLatency(ping);
        UpdatePlayerView();
        playerNameText.text = player.NickName;
        ChangePlayerStatus(player);
    }

    #region Swap animation

    public void ChangePlayerStatus(Player player)
    {
        bool isReady = (bool)player.CustomProperties[Constants.Networking.PLAYER_READY];
        StartSwapAnimation(isReady);
    }

    private void StartSwapAnimation(bool isReady)
    {
        Transform containerTo = isReady ? readyContainer.transform : notReadyContainer.transform;
        Transform containerFrom = isReady ? notReadyContainer.transform : readyContainer.transform;

        containerTo.transform.SetParent(readyInfoContainer.transform, true);
        containerTo.gameObject.SetActive(true);

        containerFrom.transform.SetParent(swapAnimationContainer.transform, true);
        swapAnimationGlow.anchoredPosition = new Vector2(swapGlowStartAnhchoredX, 0);

        TweenAnimationData data = new TweenAnimationData(swapAnimationContainer.gameObject,
            1, 0, swapAnimationDuration, OnSwapAnimationUpdate, OnSwapAnimationFinish);
    }

    private void OnSwapAnimationUpdate(float t)
    {
        swapAnimationContainer.fillAmount = t;
    }

    private void OnSwapAnimationFinish()
    {
        if (associatedPlayer == null) return;

        bool isReady = (bool)associatedPlayer.CustomProperties[Constants.Networking.PLAYER_READY];

        Transform containerTo = isReady ? readyContainer.transform : notReadyContainer.transform;
        Transform containerFrom = isReady ? notReadyContainer.transform : readyContainer.transform;

        containerFrom.gameObject.SetActive(false);
        containerFrom.transform.SetParent(readyInfoContainer.transform, true);
        swapAnimationContainer.fillAmount = 1;
    }

    #endregion

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
