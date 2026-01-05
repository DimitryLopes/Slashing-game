using UnityEngine;
using TMPro;

public class FloatingText : Activateable
{

    [SerializeField]
    private TextMeshProUGUI textComponent;
    [SerializeField]
    private UIAnimationComponent textAnimation;

    public void ShowText(string message)
    {
        textComponent.text = message;
        Activate();
        textAnimation.PlayInAnimations(OnInAnimationFinish);
    }

    private void OnInAnimationFinish()
    {
        textAnimation.PlayOutAnimations();
    }
}


