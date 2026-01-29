using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour, IManager
{
    public static UIAnimationManager Instance;

    public bool IsInitialized { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsInitialized = true;
    }

    private Dictionary<GameObject, List<UIAnimation>> activeAnimations = new();

    /// <summary>
    /// Adds an animation to be managed by the manager.
    /// </summary>
    /// <param name="animation">The animation to be added.</param>
    public void AddAnimation(UIAnimation animation)
    {        
        if (activeAnimations.ContainsKey(animation.AnimationTarget))
        {
            List<UIAnimation> animationsOnTarget = activeAnimations[animation.AnimationTarget];

            for(int i = 0; i < animationsOnTarget.Count;)
            {
                var existingAnimation = animationsOnTarget[i];
                bool isHigherPriority = animation.Priority > existingAnimation.Priority;
                bool isSameCoponent = animation.ComponentReference == existingAnimation.ComponentReference;
                if (isHigherPriority && !isSameCoponent && existingAnimation.IsPlaying)
                {
                    // Cancel lower or equal priority animations
                    LeanTween.cancel(existingAnimation.Tween.id, true);
                    // Animation removes itself onComplete
                }
                else
                {
                    i++;
                }
            }
            if (activeAnimations.ContainsKey(animation.AnimationTarget))
            {
                activeAnimations[animation.AnimationTarget].Add(animation);            
            }
            else // All animations were cancelled
            {
                activeAnimations.Add(animation.AnimationTarget, new List<UIAnimation>() { animation });
            }
        }
        else
        {
            activeAnimations.Add(animation.AnimationTarget, new List<UIAnimation>() { animation });
        }
    }

    public void RemoveAnimation(UIAnimation animation)
    {
        if (!activeAnimations.ContainsKey(animation.AnimationTarget)) return;

        var animationsOnTarget = activeAnimations[animation.AnimationTarget];
        animationsOnTarget.Remove(animation);

        if (animationsOnTarget.Count == 0)
            activeAnimations.Remove(animation.AnimationTarget);
        else
            activeAnimations[animation.AnimationTarget] = animationsOnTarget;

        LeanTween.cancel(animation.Tween.id);
    }

    /// <summary>
    /// Cancels all animations on a specific GameObject.
    /// </summary>
    /// <param name="target">The GameObject whose animations to cancel.</param>
    public void Cancel(GameObject target, int priority)
    {
        var animationsToCancel = new List<UIAnimation>();
        if (activeAnimations.ContainsKey(target))
        {
            foreach (var animation in activeAnimations[target])
            {
                if (animation.IsPlaying && priority >= animation.Priority)
                {
                    animationsToCancel.Add(animation);
                }
            }
            foreach (var animation in animationsToCancel)
            {
                LeanTween.cancel(animation.Tween.id, true);
                //Animation removes itself onComplete
            }
        }
    }

    public void Cancel(UIAnimation animation, int priority)
    {
        if (animation.IsPlaying && priority >= animation.Priority)
        {
            LeanTween.cancel(animation.Tween.id, true);
            //Animation removes itself onComplete
        }
    }
}
