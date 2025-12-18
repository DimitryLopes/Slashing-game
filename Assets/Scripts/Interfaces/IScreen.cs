public interface IScreen
{
    bool IsShown { get; }
    void Show<T>(T controller) where T : ScreenController;
    void Show();
    void Hide();
    void HideImmediately(bool sendEvent);
}