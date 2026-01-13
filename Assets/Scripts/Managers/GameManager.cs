using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPun
{
    //InGame state
    [SerializeField]
    private int initialLives = 3;
    [SerializeField]
    private TargetSpawner targetSpawner;

    private float currentScore;

    public static GameManager Instance { get; private set; }

    public int Lives { get; private set; }


    private GameState currentState = GameState.Initializing;

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
    }

    private void Start()
    {
        SceneManager.sceneLoaded += ChangeStateToInGame;
        EventManager.OnTargetHit.AddListener(OnTargetHit);
        EventManager.OnTargetMiss.AddListener(OnTargetMiss);
        ChangeState(GameState.Menu);
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
                StartCoroutine(HandleInGameState());
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
        Debug.Log("Entered Lobby state.");
    }

    private void HandleRoomState()
    {
        Debug.Log("Entered Room state.");
    }

    private void HandleEndGameState()
    {
        targetSpawner.DisableSpawn();
        ScreenManager.Instance.Show<GameOverScreen>(new GameOverScreenController(currentScore));
    }

    private void ChangeStateToInGame(Scene scene, LoadSceneMode mode)
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

    #region InGame
    private IEnumerator HandleInGameState()
    {
        Lives = initialLives;
        currentScore = 0;
        yield return new WaitForSeconds(3f);
        if(PhotonNetwork.IsMasterClient)
            targetSpawner.EnableSpawn();
        ScreenManager.Instance.Show<GameScreen>(new GameScreenController(Lives));
     }
    
    private void OnTargetMiss(Target target)
    {
        switch (target.Data.Type)
        {
            case TargetType.Explosive:
                return;
        }
        photonView.RPC(nameof(RPCLoseLife), RpcTarget.All);
    }

    private void OnTargetHit(Target target, HitInfo info)
    {
        switch (target.Data.Type)
        {
            case TargetType.Explosive:
                photonView.RPC(nameof(RPCLoseLife), RpcTarget.All);
                return;
            case TargetType.SpecificPlayer:
                var specificTarget = target as PlayerSpecificTarget;
                if (info.Player != specificTarget.Player)
                {
                    photonView.RPC(nameof(RPCLoseLife), RpcTarget.All);
                    return;
                }
                break;

        }
        currentScore += info.Score;
        EventManager.OnScoreUpdated.Invoke(currentScore);
    }

    [PunRPC]
    private void RPCLoseLife()
    {
        LoseLife();
    }

    private void LoseLife()
    {
        if(Lives <= 0) return;
        Lives--;
        EventManager.OnPlayerDamaged.Invoke(Lives);
        if(Lives == 0)
        {
            Debug.Log("No lives left. Game Over!");
            ChangeState(GameState.EndGame);
        }
        else
        {
            Debug.Log($"Life lost! Remaining lives: {Lives}");
        }
    }
    #endregion
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