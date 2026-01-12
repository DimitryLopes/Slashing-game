using System;

public class MainMenuScreenController : ScreenController
{
    public readonly Action OnPlayButtonClicked;
    public readonly Action OnSettingsButtonClicked;
    public readonly Action OnExitButtonClicked;

    public MainMenuScreenController(Action play, Action showSettings, Action exitGame)
    {
        OnPlayButtonClicked = play;
        OnSettingsButtonClicked = showSettings;
        OnExitButtonClicked = exitGame;
    }
}
