using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScreen : UIScreen<GameScreenController>
{
    public const string LIVES_TEXT_FORMAT = "Lives: {0}";

    [SerializeField]
    private TextMeshProUGUI livesText;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        UpdateLives(Controller.PlayerMaxLives);
        EventManager.OnPlayerDamaged.AddListener(UpdateLives);
    }

    public void UpdateLives(int lives)
    {
        livesText.text = string.Format(LIVES_TEXT_FORMAT, lives);
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        EventManager.OnPlayerDamaged.RemoveListener(UpdateLives);
    }
}
