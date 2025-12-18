using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private ScreenDataBase screenDataBase;

    public static ScreenManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private readonly Dictionary<Type, IScreen> instantiatedScreens = new();
    private readonly Stack<IScreen> screenStack = new();
    private (IScreen, ScreenController) enqueuedScreen = new();

    public IScreen ActiveScreen => screenStack.Peek();
    public bool IsShowingScreen => screenStack.Count > 0;

    private void Start()
    {
        EventManager.OnScreenBeforeShowEvent.AddListener(OnScreenShown);
        EventManager.OnScreenAfterHideEvent.AddListener(OnScreenHidden);
    }

    private void OnScreenShown(IScreen screen)
    {
        if (enqueuedScreen.Item1 != null)
        {
            if (enqueuedScreen.Item1 == screen)
            {
                screenStack.Push(enqueuedScreen.Item1);
                enqueuedScreen.Item2 = null;
                enqueuedScreen.Item1 = null;
            }
        }
    }

    private void OnScreenHidden(IScreen screen)
    {
        if (ActiveScreen == screen)
        {
            if (enqueuedScreen.Item1 != null)
            {
                enqueuedScreen.Item1.Show(enqueuedScreen.Item2);
                //Debug.Log($"[ScreenManager] Showing enqueued screen {screen}");
            }
            else if (screenStack.Count > 1)
            {
                screenStack.Pop();
                ActiveScreen.Show();
                //Debug.Log($"[ScreenManager] No screens enqueued, showing first in stack {screen}");
            }
            else
            {
                screenStack.Pop();
            }
            return;
        }
        //Debug.LogError($"[ScreenManager] Hid {screen} but it was not the first in stack.");
    }

    public void Show<TScreen>(ScreenController controller) where TScreen : IScreen
    {
        var newScreen = GetScreen<TScreen>();
        if (newScreen == null || newScreen.IsShown) return;

        //Debug.Log($"[ScreenManager] Attempting to show {typeof(TScreen)}");

        if (screenStack.Count > 0)
        {
            //Debug.Log($"[ScreenManager] Hiding active screen {ActiveScreen.GetType()}");
            enqueuedScreen = (newScreen, controller);
            //Debug.Log($"[ScreenManager] Enqueing requestd screen {typeof(TScreen)}");
            ActiveScreen.Hide();
            return;
        }

        ShowScreen(newScreen, controller);
    }

    private void ShowScreen(IScreen screen, ScreenController controller)
    {
        //Debug.Log($"[ScreenManager] Showing {screen.GetType().Name}");
        screen.Show(controller);
        screenStack.Push(screen);

    }

    public void HideAll()
    {
        IScreen screen = ActiveScreen;
        screenStack.Clear();
        screenStack.Push(screen);
        screen.Hide();
    }

    public void ClearStack()
    {
        IScreen screen = ActiveScreen;
        screenStack.Clear();
        screenStack.Push(screen);
    }

    #region Screen Retrieving
    public TScreen GetScreen<TScreen>() where TScreen : IScreen
    {
        Type type = typeof(TScreen);
        if (instantiatedScreens.TryGetValue(type, out var screen))
        {
            return (TScreen)screen;
        }

        return InstantiateScreen<TScreen>();
    }

    private TScreen InstantiateScreen<TScreen>() where TScreen : IScreen
    {
        Type type = typeof(TScreen);
        TScreen newScreen = Instantiate(screenDataBase.UIScreens[typeof(TScreen)]).GetComponent<TScreen>();
        if (newScreen != null)
            instantiatedScreens.Add(type, newScreen);
        return newScreen;
    }
    #endregion
}