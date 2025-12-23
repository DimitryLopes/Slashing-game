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

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        finalScoreText.text = $"Pontuação Final: {Controller.Score}";
        roomScreenButton.onClick.AddListener(OnRoomScreenButtonClick);
        lobbyScreenButton.onClick.AddListener(OnLobbyScreenButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuGameButtonClick);
    }

    private void OnRoomScreenButtonClick()
    {

    }

    private void OnLobbyScreenButtonClick()
    {

    }

    private void OnMainMenuGameButtonClick()
    {

    }
}
