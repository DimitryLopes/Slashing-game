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
    private float swapGlowStartLocalX = -85.4f;
    [SerializeField]
    private float swapGlowFinishLocaldX = -1;
    [SerializeField]
    private float swapAnimationDuration = 0.5f;

    private Player associatedPlayer;
    private UnityAction<Player> onPlayerKickButtonPressed;
    public Player Player => associatedPlayer;

    public bool IsOccupied => associatedPlayer != null;

    public void Setup(UnityAction<Player> kickPlayerActionm)
    {
        onPlayerKickButtonPressed = kickPlayerActionm;
     
        kickPlayerButton.onClick.RemoveAllListeners();

        kickPlayerButton.onClick.AddListener(OnPlayerKickButtonClick);
    }

    public void SetPlayer(Player player, bool isMasterClient)
    {
        bool shouldShowKickButton = !player.IsLocal && isMasterClient;
        kickPlayerButton.gameObject.SetActive(shouldShowKickButton);
        masterClientIcon.SetActive(player.IsMasterClient);

        associatedPlayer = player;
        float ping = player.GetPing();
        latency.UpdateLatency(ping);
        UpdatePlayerView();
        playerNameText.text = player.NickName;
        ChangePlayerStatus();
    }

    #region Swap animation

    public void ChangePlayerStatus()
    {
        bool isReady = (bool)associatedPlayer.CustomProperties[Constants.Networking.PLAYER_READY];
        StartSwapAnimation(isReady);
    }

    private void StartSwapAnimation(bool isReady)
    {
        Transform containerTo = isReady ? readyContainer.transform : notReadyContainer.transform;
        Transform containerFrom = isReady ? notReadyContainer.transform : readyContainer.transform;

        containerTo.transform.SetParent(readyInfoContainer.transform, true);
        containerTo.transform.SetAsFirstSibling();
        containerTo.gameObject.SetActive(true);

        containerFrom.transform.SetParent(swapAnimationContainer.transform, true);
        swapAnimationGlow.anchoredPosition = new Vector2(swapGlowStartLocalX, 0);

        TweenAnimationData data = new TweenAnimationData(swapAnimationContainer.gameObject,
            1, 0, swapAnimationDuration, OnSwapAnimationUpdate, OnSwapAnimationFinish);
        TweenUtils.PlayTween(data);

        swapAnimationGlow.transform.LeanMoveLocalX(-swapGlowFinishLocaldX, swapAnimationDuration);
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
        notReadyContainer.SetActive(true);
        readyContainer.SetActive(false);
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
