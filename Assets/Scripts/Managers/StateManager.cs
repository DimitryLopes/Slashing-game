using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour, IManager
{
    public static StateManager Instance { get; private set; }

    private GameState currentState = GameState.Initializing;

    public bool IsInitialized { get; private set; }

    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            currentState = value;
            OnGameStateChanged?.Invoke(currentState);
        }
    }

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        IsInitialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState) return;

        Debug.Log($"Changing state from {CurrentState} to {newState}");
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Menu:
                HandleMenuState();
                break;
            case GameState.Lobby:
                HandleLobbyState();
                break;
            case GameState.Room:
                HandleRoomState();
                break;
            case GameState.InGame:
                HandleInGameState();
                break;
            case GameState.EndGame:
                HandleEndGameState();
                break;
        }
    }

    private void HandleMenuState()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != Constants.Scenes.MENU)
            SceneManager.LoadScene(Constants.Scenes.MENU);
        else
            ShowMainMenu();
    }

    private void HandleLobbyState()
    {
        NetworkManager.Instance.JoinLobby();
    }

    private void HandleRoomState()
    {
        NetworkManager.Instance.ShowRoom();
    }

    private void HandleEndGameState()
    {
        GameManager.Instance.EndGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == Constants.Scenes.GAME)
            ChangeState(GameState.InGame);
        if (scene.name == Constants.Scenes.MENU)
            ShowMainMenu();
    }

    #region Menu
    private void ShowMainMenu()
    {
        var controller = new MainMenuScreenController(ShowLobbyScreen,ShowSettingsScreen,Quit);
        ScreenManager.Instance.Show<MainMenuScreen>(controller);
    }

    private void ShowLobbyScreen()
    {
        ScreenManager.Instance.Show<LoadingScreen>(new LoadingScreenController());
        NetworkManager.Instance.Connect("a");
    }

    private void ShowSettingsScreen()
    {

    }

    private void Quit()
    {
        Application.Quit();
    }
    #endregion

    private void HandleInGameState()
    {
        GameManager.Instance.StartGame();
    }
    
}

public enum GameState
{
    Menu,
    Lobby,
    Room,
    InGame,
    EndGame,
    Initializing
}