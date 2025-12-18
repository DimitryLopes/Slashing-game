using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour
{

    public static UIAnimationManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private List<UIAnimation> activeAnimations = new();

    /// <summary>
    /// Adds an animation to be managed by the manager.
    /// </summary>
    /// <param name="animation">The animation to be added.</param>
    public void AddAnimation(UIAnimation animation)
    {
        if (animation.AnimationTarget == null)
        {
            Debug.LogError("UIAnimationManager: Attempted to add an animation with no target.");
            return;
        }

        // Add the animation to the list
        activeAnimations.Add(animation);
    }

    public void RemoveAnimation(UIAnimation animation)
    {
        if (animation.AnimationTarget == null)
        {
            Debug.LogError("UIAnimationManager: Attempted to remove an animation with no target.");
            return;
        }

        if (!activeAnimations.Contains(animation)) return;

        activeAnimations.Remove(animation);

        LeanTween.cancel(animation.Tween.id);
    }

    /// <summary>
    /// Cancels a specific animation by LeanTween ID (tween.id)
    /// </summary>
    /// <param name="id">LeanTween ID</param>
    public void Cancel(int id, int priority)
    {
        for (int i = 0; i < activeAnimations.Count; i++)
        {

            if (activeAnimations[i].Tween.id == id)
            {
                Cancel(activeAnimations[i], priority);
                return;
            }
        }

        Debug.LogWarning($"UIAnimationManager: No animation found with \n ID {id}");
    }

    /// <summary>
    /// Cancels all animations on a specific GameObject.
    /// </summary>
    /// <param name="target">The GameObject whose animations to cancel.</param>
    public void Cancel(GameObject target, int priority)
    {
        if (target == null) return;

        for (int i = 0; i < activeAnimations.Count; i++)
        {
            Cancel(activeAnimations[i], priority);
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

    public void CancelAll()
    {
        for (int i = 0; i < activeAnimations.Count; i++)
        {
            if (activeAnimations[i].Tween != null)
            {
                LeanTween.cancel(activeAnimations[i].Tween.id, true);
            }
        }
        activeAnimations.Clear();
    }
}
