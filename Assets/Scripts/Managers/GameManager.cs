using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //InGame state
    [SerializeField]
    private int initialLives = 3;
    [SerializeField]
    private TargetSpawner targetSpawner;

    public static GameManager Instance { get; private set; }

    public int Lives { get; private set; }

    public enum GameState
    {
        Menu,
        Lobby,
        Room,
        InGame,
        EndGame
    }

    private GameState currentState;

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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
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
        Debug.Log("Entered Menu state.");
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
        Debug.Log("Entered EndGame state.");
    }

    #region InGame
    private IEnumerator HandleInGameState()
    {
        Debug.Log("Entered InGame state. Starting 3-second countdown...");
        Lives = initialLives;
        yield return new WaitForSeconds(3f);
        if(PhotonNetwork.IsMasterClient)
            targetSpawner.EnableSpawn(OnTargetMiss);
        Debug.Log("Players can now play!");
    }
    
    private void OnTargetMiss()
    {
        LoseLife();
    }

    private void LoseLife()
    {
        if(Lives <= 0) return;
        Lives--;
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
