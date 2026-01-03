using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : UIScreen<GameOverScreenController>
{
    [SerializeField]
    private TextMeshProUGUI finalScoreText;
    [SerializeField]
    private Button roomScreenButton;
    [SerializeField]
    private Button lobbyScreenButton;
    [SerializeField]
    private Button mainMenuButton;
    [SerializeField]
    private Button replayButton;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        finalScoreText.text = $"Pontuação Final: {Controller.Score}";
        roomScreenButton.onClick.AddListener(OnRoomScreenButtonClick);
        lobbyScreenButton.onClick.AddListener(OnLobbyScreenButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuGameButtonClick);
        replayButton.onClick.AddListener(OnPlayAgainButtonClicked);
    }

    private void OnRoomScreenButtonClick()
    {

    }

    private void OnLobbyScreenButtonClick()
    {

    }

    private void OnPlayAgainButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.InGame);
    }

    private void OnMainMenuGameButtonClick()
    {
        GameManager.Instance.ChangeState(GameState.Menu);
    }
}
