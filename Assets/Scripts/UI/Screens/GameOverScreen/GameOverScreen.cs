using UnityEngine;
using UnityEngine.UI;
using System;

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
    private Button leaveButton; //takes back to main menu
    [SerializeField]
    private Button playAgainButton; //takes back to game screen


    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        roomScreenButton.onClick.AddListener(OnRoomScreenButtonClick);
        leaveButton.onClick.AddListener(OnMainMenuGameButtonClick);
        playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);

        scoreStatInfo.SetStatValue(Controller.Score.ToString());
        targetStatInfo.SetStatValue(Controller.TargetsHit.ToString());
        bossStatInfo.SetStatValue(Controller.BossesDefeated.ToString());

        TimeSpan time = TimeSpan.FromSeconds(Controller.ElapsedTime);
        string minutesAndSeconds = time.ToString(@"m\:ss");
        timeStatInfo.SetStatValue(minutesAndSeconds);
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
