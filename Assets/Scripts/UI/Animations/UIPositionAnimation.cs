using UnityEngine;

public class UIPositionAnimation : UIAnimation
{
    public enum MovementType
    {
        Linear,
        Arch
    }

    [Header("Position Settings")]
    [SerializeField] private bool startFromSetPosition = true;
    [SerializeField, ShowIf("startFromSetPosition")] private Vector3 startPosition = Vector3.zero;
    [SerializeField] private Vector3 endPosition = Vector3.zero;
    [SerializeField] private MovementType movementType = MovementType.Linear;
    [SerializeField][ShowIf("movementType", MovementType.Arch)] private AnimationCurve movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Animation curve for arch movement
    
    private RectTransform rectTransform;

    protected override void DoAnimation(GameObject target)
    {
        Vector3 endPos = endPosition;
        if (!startFromSetPosition)
        {
            startPosition =  rectTransform.anchoredPosition;
            endPos = new Vector3(rectTransform.localPosition.x + endPosition.x,
                rectTransform.localPosition.y + endPosition.y);
        }

        if (movementType == MovementType.Linear)
        {
            rectTransform.anchoredPosition = startPosition;
            if (!startFromSetPosition)
                tween = target.LeanMoveLocal(endPos, duration);
            else
                tween = target.LeanMoveLocal(endPosition, duration);
                return;
        }

        tween = LeanTween.value(0f, 1f, duration).setOnUpdate(ArchMovement);
    }

    private void ArchMovement(float t)
    {
        float curveValue = movementCurve.Evaluate(t);
        Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);

        float archOffset = curveValue * Mathf.Abs(endPosition.y - startPosition.y);

        rectTransform.anchoredPosition = new Vector3(currentPosition.x, currentPosition.y + archOffset, currentPosition.z);
    }

    protected override void FirstShowSetup()
    {
        if (rectTransform == null)
        {
            rectTransform = animationTarget.GetComponent<RectTransform>();
            return;
        }

        if (rectTransform == null)
        {
            Debug.LogError("UIPositionAnimation: The target GameObject "+ animationTarget.name + " does not have a RectTransform component.");
        }
    }
}