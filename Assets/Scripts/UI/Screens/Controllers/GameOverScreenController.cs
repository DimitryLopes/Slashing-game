using UnityEngine.Events;

public class GameOverScreenController : ScreenController
{
    public readonly float Score;
    public readonly float ElapsedTime;
    public readonly int TargetsHit;
    public readonly int BossesDefeated;

    public readonly UnityAction OnPlayAgainButtonClicked;
    public readonly UnityAction OnMainMenuButtonClicked;
    public readonly UnityAction OnLobbyButtonClicked;

    public GameOverScreenController(float score, float elapsedTime, int targetsHit, 
        int bossesDefeated, UnityAction onPLayAgain, UnityAction onMainMenu, UnityAction onLobby)
    {
        Score = score;
        ElapsedTime = elapsedTime;
        TargetsHit = targetsHit;
        BossesDefeated = bossesDefeated;
        OnPlayAgainButtonClicked = onPLayAgain;
        OnMainMenuButtonClicked = onMainMenu;
        OnLobbyButtonClicked = onLobby;
    }
}
