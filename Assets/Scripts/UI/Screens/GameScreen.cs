using TMPro;
using UnityEngine;

public class GameScreen : UIScreen<GameScreenController>
{
    public const string LIVES_TEXT_FORMAT = "Lives: {0}";
    public const string SCORE_TEXT_FORMAT = "Score: {0}";

    [SerializeField]
    private TextMeshProUGUI livesText;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private float previousScore = 0;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();        
        UpdateLives(Controller.PlayerMaxLives); 
        ClearScore();
        EventManager.OnPlayerDamaged.AddListener(UpdateLives);
        EventManager.OnScoreUpdated.AddListener(UpdateScore);
    }

    private void UpdateLives(int lives)
    {
        livesText.text = string.Format(LIVES_TEXT_FORMAT, lives);
    }

    private void ClearScore()
    {
        previousScore = 0;
        scoreText.text = string.Format(SCORE_TEXT_FORMAT, 0);
    }

    private void UpdateScore(float score)
    {
        scoreText.AnimateToValue((int) score, (int)previousScore, SCORE_TEXT_FORMAT);
        previousScore = score;
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        EventManager.OnPlayerDamaged.RemoveListener(UpdateLives);
    }
}
