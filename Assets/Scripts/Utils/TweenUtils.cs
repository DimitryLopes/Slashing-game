using System;
using UnityEngine;

public class TweenUtils
{
    public static void CancelTween(GameObject gameObject, bool finishCurrent)
    {
        bool isTweening = gameObject.LeanIsTweening();
        if (!isTweening) return;

        LeanTween.cancel(gameObject, finishCurrent);
    }

    /// <summary>
    /// tweens a float variable using animation data
    /// </summary>
    /// <param name="data">Animation Data</param>
    /// <param name="overrideCurrent">whether or not the current tween should be overriden</param>
    /// <param name="finishCurrent">whether or not the current tween should call OnComplete. Only works if overrideCurrent is true</param>
    public static void PlayTween(TweenAnimationData data, bool overrideCurrent = true, bool finishCurrent = true)
    {
        if (!overrideCurrent) return;
        CancelTween(data.GameObject, finishCurrent);

        LeanTween.value(data.GameObject, data.From, data.To, data.Duration).setOnUpdate(data.OnUpdate).setOnComplete(data.OnComplete);
    }
}

public struct TweenAnimationData
{
    public TweenAnimationData(GameObject gameObject, float from, float to, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        GameObject = gameObject;
        From = from;
        To = to;
        Duration = duration;
        OnUpdate = onUpdate;
        OnComplete = onComplete;
    }

    public GameObject GameObject { get; private set; }
    public float From { get; private set; }
    public float To { get; private set; }
    public float Duration { get; private set; }
    public Action<float> OnUpdate { get; private set; }
    public Action OnComplete { get; private set; }

}
