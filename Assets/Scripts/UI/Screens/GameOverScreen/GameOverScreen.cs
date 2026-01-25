using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : UIScreen<GameOverScreenController>
{
    [SerializeField]
    private UIStatInfo scoreStatInfo;
    [SerializeField]
    private UIStatInfo targetStatInfo;
    [SerializeField]
    private UIStatInfo bossStatInfo;
    [SerializeField]
    private UIStatInfo timeStatInfo;

    [SerializeField]
    private Button roomScreenButton; //takes back to lobby screen
    [SerializeField]
    private Button mainMenuButton; //takes back to main menu
    [SerializeField]
    private Button playAgainButton; //takes back to game screen


    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        roomScreenButton.onClick.AddListener(OnRoomScreenButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuGameButtonClick);
        playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);

        scoreStatInfo.SetStatValue(Controller.ScoreStatValue);
        targetStatInfo.SetStatValue(Controller.TargetStatValue);
        bossStatInfo.SetStatValue(Controller.BossStatValue);
        timeStatInfo.SetStatValue(Controller.TimeStatValue);
    }

    private void OnRoomScreenButtonClick()
    {
        Controller.OnLobbyButtonClicked.Invoke();
    }
    private void OnPlayAgainButtonClicked()
    {
        Controller.OnPlayAgainButtonClicked.Invoke();
    }

    private void OnMainMenuGameButtonClick()
    {
        Controller.OnMainMenuButtonClicked.Invoke();
    }
}
