using System;
using UnityEngine;

[Serializable]
public abstract class UIAnimation
{
    [Header("Basic Settings")]
    [SerializeReference] protected float duration = 1f;
    [SerializeReference] protected int priority = 0;
    [SerializeReference] protected LeanTweenType easeType = LeanTweenType.easeOutQuart;

    [Header("Custom Animation Curve")]
    [SerializeReference] private bool useCustomCurve = false;
    [SerializeReference] [ShowIf("useCustomCurve")] protected AnimationCurve customCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Loop Settings")]
    [SerializeReference, Tooltip("-1 for infinite loop, 0 for no loop")]
    protected int loopCount = 0;

    [Header("Timing Settings")]
    [SerializeReference, Tooltip("Delay before animation starts")]
    protected float startDelay = 0f;
    [SerializeReference] protected bool ignoreTimeScale = false;

    protected LTDescr tween;
    protected Action callback;
    protected GameObject animationTarget;
    private bool isPlaying;

    public GameObject AnimationTarget => animationTarget;
    public LTDescr Tween => tween;
    public bool IsPlaying => isPlaying;
    public int Priority => priority;

    private bool isFirstPlayDone = false;
    private UIAnimationManager animationManager;

    public float Duration => duration;


    /// <summary>
    /// Called once, before the first time the animation plays. Override this method in child classes to fetch necessary components.
    /// </summary>
    protected virtual void FirstShowSetup()
    {
    }

    /// <summary>
    /// Start the animation on the provided GameObject with an optional callback.
    /// </summary>
    /// <param name="target">GameObject to animate</param>
    /// <param name="callback">Callback function on completion</param>
    public virtual void Animate(GameObject target, Action callback = null, bool debug = false)
    {
        if (target == null)
        {
            Debug.LogError("Target GameObject is null! Animation cannot be performed.");
            return;
        }

        animationTarget = target;
        this.callback = callback;
        // Call FirstPlay once, before the first time the animation runs
        if (!isFirstPlayDone)
        {
            animationManager = UIAnimationManager.Instance;
            FirstShowSetup();
            isFirstPlayDone = true;
        }

        if (!debug)
        {
            animationManager.Cancel(this, priority);
        }

        // Start the actual animation
        DoAnimation(target);
        if (!debug)
        {
            animationManager.AddAnimation(this);
        }

        tween.setDelay(startDelay)
             .setIgnoreTimeScale(ignoreTimeScale);

        // Apply either the custom curve or the predefined ease type
        if (useCustomCurve)
        {
            tween.setEase(customCurve);
        }
        else
        {
            tween.setEase(easeType);
        }

        // Handle looping
        if (loopCount != 0)
        {
            tween.setLoopCount(loopCount);
        }

        // Set the completion logic
        tween.setOnComplete(() =>
        {
            isPlaying = false;

            if(!debug)
                animationManager.RemoveAnimation(this);
            tween = null;
            this.callback?.Invoke();
        });
        isPlaying = true;
    }

    public void SetDynamicDelay(float delay)
    {
        startDelay = delay;
    }

    /// <summary>
    /// Must be implemented by derived classes to perform the actual animation.
    /// </summary>
    protected abstract void DoAnimation(GameObject target);
}
