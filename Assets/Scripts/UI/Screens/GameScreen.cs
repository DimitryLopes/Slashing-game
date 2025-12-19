using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScreen : UIScreen<GameScreenController>
{
    [SerializeField]
    private TextMeshProUGUI livesText;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        EventManager.OnPlayerDamaged.AddListener(UpdateLives);
    }

    public void UpdateLives(int lives)
    {
        livesText.text = lives.ToString();
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        EventManager.OnPlayerDamaged.RemoveListener(UpdateLives);
    }
}
