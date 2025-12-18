using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIColorAnimation : UIAnimation
{
    public enum TargetType { Image, Text }

    [SerializeField]
    private TargetType targetType = TargetType.Image;

    [SerializeField, ShowIf(nameof(targetType), TargetType.Image)]
    private Image targetImage;

    [SerializeField, ShowIf(nameof(targetType), TargetType.Text)]
    private TextMeshProUGUI targetText;

    [SerializeField]
    private Color targetColor = Color.white;

    protected override void DoAnimation(GameObject target)
    {
        if (targetType == TargetType.Image && targetImage != null)
        {
            tween = LeanTween.value(target, targetImage.color, targetColor, duration)
                     .setEase(easeType)
                     .setOnUpdate((Color value) => targetImage.color = value);
        }
        else if (targetType == TargetType.Text && targetText != null)
        {
            tween = LeanTween.value(target, targetText.color, targetColor, duration)
                     .setEase(easeType)
                     .setOnUpdate((Color value) => targetText.color = value);
        }
        else
        {
            Debug.LogError($"UIColorAnimation: {target.name} is missing a {targetType} component");
        }
    }

    protected override void FirstShowSetup()
    {
        if (targetType == TargetType.Image && !targetImage)
        {
            targetImage = animationTarget.GetComponent<Image>();
        }
        else if (targetType == TargetType.Text && !targetText)
        {
            targetText = animationTarget.GetComponent<TextMeshProUGUI>();
        }
    }
}