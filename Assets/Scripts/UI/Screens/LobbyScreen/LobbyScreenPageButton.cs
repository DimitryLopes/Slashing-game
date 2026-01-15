using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyScreenPageButton : Activateable
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI buttonText;
    [SerializeField]
    private GameObject selectionIndicator;

    private UnityAction onButtonClicked;

    public void Initialize(string text, UnityAction onClick)
    {
        buttonText.text = text;
        onButtonClicked = onClick;
        button.onClick.AddListener(onButtonClicked);
    }
}
