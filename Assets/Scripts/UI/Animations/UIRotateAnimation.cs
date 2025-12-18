using UnityEngine;

public class UIRotationAnimation : UIAnimation
{
    public enum RotationType
    {
        Continuous,
        Set,
    }

    [SerializeField] private RotationType rotationType;

    [SerializeField]
    [ShowIf("rotationType", RotationType.Continuous)]
    private float degrees;

    [SerializeField]
    [ShowIf("rotationType", RotationType.Set)]
    private Vector3 endAngle = new Vector3(0f, 0f, 90f);

    protected override void DoAnimation(GameObject target)
    {
        switch (rotationType)
        {
            case RotationType.Continuous:
                tween = LeanTween.rotateAroundLocal(target, Vector3.forward, degrees, duration).setEase(easeType);
                break;
            case RotationType.Set:
                tween = LeanTween.rotate(target, endAngle, duration).setEase(easeType);
                break;
        }
    }
}