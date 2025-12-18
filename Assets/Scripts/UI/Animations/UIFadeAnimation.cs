using UnityEngine;

public class UIFadeAnimation : UIAnimation
{
    [SerializeField] private float startAlpha = 0f;
    [SerializeField] private float endAlpha = 1f;
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void DoAnimation(GameObject target)
    {
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
            Debug.LogWarning(target.name + "had no canvas group. Adding one as a fallback. please add one!");
        }

        canvasGroup.alpha = startAlpha;
        tween = LeanTween.alphaCanvas(canvasGroup, endAlpha, duration)
            .setEase(easeType);
    }
}