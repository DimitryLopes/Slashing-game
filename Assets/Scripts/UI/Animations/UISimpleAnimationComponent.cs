using UnityEngine;

public class UISimpleAnimationComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeReference] public UIAnimation uiAnimation;

    public void PlayAnimation()
    {
        if (uiAnimation != null)
        {
            if (LeanTween.isTweening(target))
            {
                LeanTween.cancel(target);
            }
            uiAnimation.Animate(target);
        }
    }
}
