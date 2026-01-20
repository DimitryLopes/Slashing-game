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

    private UnityAction<int> onButtonClicked;

    private int pageIndex;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public void Initialize(int index, UnityAction<int> onClick)
    {
        pageIndex = index;
        buttonText.text = (index+1).ToString();
        onButtonClicked = onClick;
    }

    private void OnButtonClick()
    {
        onButtonClicked.Invoke(pageIndex);
    }
}
