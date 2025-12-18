using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UIAnimationComponent : MonoBehaviour
{
    [Header("In Animations")]
    [SerializeReference] public List<UIAnimation> inAnimations = new List<UIAnimation>();

    [Header("Out Animations")]
    [SerializeReference] public List<UIAnimation> outAnimations = new List<UIAnimation>();

    [Header("Additional Settings")]
    [SerializeField] private int componentPriority = 0;
    [SerializeField] private GameObject animationTarget;
    [SerializeField] private bool deactivateOnHide = true; 
    [SerializeField] private bool activateOnShow = true;
    [SerializeField] private bool playAllAtOnce = false; 
    [SerializeField] private bool affectedByGetComponent = true;
    [SerializeField] private bool overrideOtherAnimationsWhenPlayed = true;

    public bool AffectedByGetComponent => affectedByGetComponent;
    public GameObject AnimationTarget => animationTarget;
    private UIAnimationManager animationManager;
    private UIAnimationManager AnimationManager
    {
        get
        {
            if(animationManager == null)
            {
                animationManager = UIAnimationManager.Instance;
            }
            return animationManager;
        }
    }

    private void Awake()
    {
        if (!animationTarget)
            animationTarget = gameObject;
    }

    
    public void PlayInAnimations(Action onComplete, bool debug = false)
    {
        if (activateOnShow && !animationTarget.activeSelf)
        {
            animationTarget.SetActive(true); 
        }

        PlayAnimations(inAnimations, onComplete, debug);
    }

    public void PlayInAnimations()
    {
        PlayInAnimations(null);
    }


    public void PlayOutAnimations(Action onComplete, bool debug = false)
    {
        onComplete += OnHideAnimationsFinished;
        PlayAnimations(outAnimations, onComplete, debug);
    }

    private void OnHideAnimationsFinished()
    {
        if (deactivateOnHide)
        {
            animationTarget.SetActive(false);
        }
    }

    public void PlayOutAnimations()
    {
        PlayOutAnimations(null);
    }

    /// <summary>
    /// Play a sequence of animations on the GameObject.
    /// </summary>
    /// <param name="animations">List of animations to play</param>
    /// <param name="onComplete">Callback when all animations are done</param>
    private void PlayAnimations(List<UIAnimation> animations, Action onComplete = null, bool debug = false)
    {
        if (!debug)
        {
            StopCurrentAnimations();
        }

        if (animations == null || animations.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (playAllAtOnce)
        {
            var sortedAnimations = animations.OrderByDescending(a => a.Duration).ToList();
            for (int i = 1; i < animations.Count; i++)
            {
                animations[i].Animate(animationTarget, debug: debug);
            }
            sortedAnimations[0].Animate(animationTarget, onComplete, debug);
        }
        else
        {
            int currentAnimationIndex = 0;

            void PlayNextAnimation()
            {
                if (currentAnimationIndex >= animations.Count)
                {
                    onComplete?.Invoke();
                    return;
                }

                var animation = animations[currentAnimationIndex];
                animation.Animate(animationTarget, () =>
                {
                    currentAnimationIndex++;
                    PlayNextAnimation();
                }, debug);
            }

            PlayNextAnimation();
        }
    }

    public void SetInDelay(float delay)
    {
        if (playAllAtOnce)
        {
            foreach (UIAnimation anim in inAnimations)
            {
                anim.SetDynamicDelay(delay);
            }
        }
        else if (inAnimations.Count > 0)
        {
            inAnimations[0].SetDynamicDelay(delay);
        }
    }

    public void SetOutDelay(float delay)
    {
        if (playAllAtOnce)
        {
            foreach (UIAnimation anim in outAnimations)
            {
                anim.SetDynamicDelay(delay);
            }
        }
        else if (outAnimations.Count > 0)
        {
            outAnimations[0].SetDynamicDelay(delay);
        }
    }

    public void StopCurrentAnimations()
    {
        if (overrideOtherAnimationsWhenPlayed)
        {
            AnimationManager.Cancel(AnimationTarget, componentPriority);
            return;
        }

        foreach (UIAnimation anim in outAnimations)
        {
            if(anim.IsPlaying)
            AnimationManager.Cancel(anim, anim.Priority);
        }

        foreach(UIAnimation anim in inAnimations)
        {
            if (anim.IsPlaying)
                AnimationManager.Cancel(anim, anim.Priority);
        }
    }
}
