using UnityEngine;

public class UIWobbleAnimation : UIAnimation
{
    [Header("Wobble Settings")]
    [SerializeField] private Vector2 minDeviation = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxDeviation = new Vector2(10f, 10f);
    [SerializeField] private Vector2 wobbleSpeed = new Vector2(1f, 1f);

    private float randomSeedX;
    private float randomSeedY;
    private Vector2 originalAnchoredPosition;
    private RectTransform rectTransform;

    protected override void FirstShowSetup()
    {
        rectTransform = animationTarget.GetComponent<RectTransform>();
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    protected override void DoAnimation(GameObject target)
    {
        if (rectTransform == null)
        {
            Debug.LogError("UIWobbleAnimation: The target GameObject does not have a RectTransform component.");
            return;
        }

        callback += ResetPosition;

        randomSeedX = Random.Range(0f, 1000f);
        randomSeedY = Random.Range(0f, 1000f);

        // Start the wobble and stop after 'duration'
        StartWobble(rectTransform);
    }

    private void StartWobble(RectTransform rectTransform)
    {
        // Run the wobble for the duration, then stop and reset the position
        tween = LeanTween.value(animationTarget, 0f, 1f, duration).setEase(LeanTweenType.linear).setOnUpdate((float time) =>
        {
            float perlinX = Mathf.PerlinNoise(randomSeedX, Time.time * wobbleSpeed.x) * 2f - 1f;
            float perlinY = Mathf.PerlinNoise(randomSeedY, Time.time * wobbleSpeed.y) * 2f - 1f;
            float deviationX = Mathf.Lerp(minDeviation.x, maxDeviation.x, (perlinX + 1f) / 2f);
            float deviationY = Mathf.Lerp(minDeviation.y, maxDeviation.y, (perlinY + 1f) / 2f);
    
            rectTransform.anchoredPosition = originalAnchoredPosition + new Vector2(deviationX, deviationY);
        });
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}
