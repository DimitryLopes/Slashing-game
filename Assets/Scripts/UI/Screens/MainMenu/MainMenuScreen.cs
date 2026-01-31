using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : UIScreen<MainMenuScreenController>
{
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button exitButton;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        Controller?.OnPlayButtonClicked();
    }
    private void OnSettingsButtonClicked()
    {
        Controller?.OnSettingsButtonClicked();
    }
    private void OnExitButtonClicked()
    {
        Controller?.OnExitButtonClicked();
    }

}
