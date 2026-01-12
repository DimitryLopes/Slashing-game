using UnityEngine;

public class LoadingScreen : UIScreen<LoadingScreenController>
{
    [SerializeField]
    private UIAnimationComponent connectingIconAnimation;


    override protected void OnBeforeShow()
    {
        base.OnBeforeShow();
        EventManager.OnLobbyJoinedEvent += Hide;
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        connectingIconAnimation.PlayInAnimations();
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        connectingIconAnimation.PlayOutAnimations();
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        EventManager.OnLobbyJoinedEvent -= Hide;
    }
    
}
