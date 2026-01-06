using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class TextMeshProExtensions
{
    private static readonly Dictionary<TMP_Text, Coroutine> activeAnimations = new();

    public static void AnimateToValue(this TMP_Text text, int targetValue, int from, string format, float duration = 0.5f)
    {
        if (activeAnimations.TryGetValue(text, out Coroutine existingAnimation))
        {
            if (text.gameObject.activeInHierarchy)
            {
                text.StopCoroutine(existingAnimation);
            }
            activeAnimations.Remove(text);
        }

        Coroutine newAnimation = text.StartCoroutine(AnimateNumber(text, targetValue, from, format, duration));
        activeAnimations[text] = newAnimation;
    }

    private static IEnumerator AnimateNumber(TMP_Text text, int targetValue, int from , string format, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(from, targetValue, t));
            text.text = string.Format(format, currentValue);
            yield return null;
        }

        text.text = string.Format(format, targetValue);

        activeAnimations.Remove(text);
    }
}