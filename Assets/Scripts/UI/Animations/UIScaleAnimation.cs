using UnityEngine;

public class UIScaleAnimation : UIAnimation
{
    [SerializeField] private bool startWithSetScale = true;
    [SerializeField, ShowIf("startWithSetScale")] private Vector3 startScale = Vector3.zero;
    [SerializeField] private Vector3 endScale = Vector3.one;

    protected override void DoAnimation(GameObject target)
    {
        if (startWithSetScale)
            target.transform.localScale = startScale;
        else
            startScale = target.transform.localScale;
        tween = LeanTween.scale(target, endScale, duration)
            .setEase(easeType);
    }
}