using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingScreen : UIScreen<LoadingScreenController>
{
    [SerializeField]
    private UIAnimationComponent connectingIconAnimation;


    override protected void OnBeforeShow()
    {
        base.OnBeforeShow();
        SceneManager.sceneLoaded += HideOnSceneLoad;
        connectingIconAnimation.PlayInAnimations();
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        Controller.OnScreenAfterShow.Invoke();
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        connectingIconAnimation.PlayOutAnimations();
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        EventManager.OnLobbyJoinedEvent.RemoveListener(Hide);
    }

    private void HideOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Hide();
        SceneManager.sceneLoaded -= HideOnSceneLoad;
    }

}
