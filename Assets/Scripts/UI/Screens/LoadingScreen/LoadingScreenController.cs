using UnityEngine.Events;

public class LoadingScreenController : ScreenController
{
    public readonly UnityAction OnScreenAfterShow;

    public LoadingScreenController(UnityAction onBeforeShow)
    {
        OnScreenAfterShow = onBeforeShow;
    }
}
