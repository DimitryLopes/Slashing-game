using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun, IManager
{
    [SerializeField]
    private int initialLives = 3;
    [SerializeField]
    private TargetSpawner targetSpawner;
    [SerializeField]
    private SliceAreaPosition[] sliceAreaPresets;

    public static GameManager Instance { get; private set; }
    public int Lives { get; private set; }
    public bool IsInitialized { get; private set; }

    private float timeElapsed;
    private int bossesDefeated;
    private int targetsHit;
    private float currentSessionScore;
    private float currentPlayerScore;

    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsInitialized = true;
    }

    private void Start()
    {
        EventManager.OnTargetHit.AddListener(OnTargetHit);
        EventManager.OnTargetMiss.AddListener(OnTargetMiss);
    }

    public void StartGame()
    {
        ClearGameStats();

        if (PhotonNetwork.IsMasterClient)
            targetSpawner.EnableSpawn();

        isPlaying = true;
        ScreenManager.Instance.Show<GameScreen>(new GameScreenController(Lives));
    }

    public void EndGame()
    {
        targetSpawner.DisableSpawn();
        GameOverScreenController controller = new GameOverScreenController
            (
                currentSessionScore, timeElapsed, targetsHit, bossesDefeated,
                OnPlayAgain, OnMainMenu, OnLobby
            );

        ScreenManager.Instance.HideAll();
        ScreenManager.Instance.Show<GameOverScreen>(controller);
        ClearGameStats();
    }

    private void ClearGameStats()
    {
        Lives = initialLives;
        currentSessionScore = 0;
        timeElapsed = 0;
        targetsHit = 0;
        bossesDefeated = 0;
        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying) return;
        timeElapsed += Time.deltaTime;
    }

    #region Slice Area
    private void AssignSliceAreas()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // Find the preset for the current player count
        SliceAreaPosition preset = sliceAreaPresets[playerCount -1];

        // Assign positions to each player
        for (int i = 0; i < playerCount; i++)
        {
            int playerId = PhotonNetwork.PlayerList[i].ActorNumber;
            Vector3 position = preset.positions[i];

            // Instantiate the slice area for the local player
            if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                CreateSliceArea(position, true);
            }

            // Notify other clients to create their slice areas
            photonView.RPC(nameof(RPCUpdateSliceArea), PhotonNetwork.PlayerList[i], position);
        }
    }
    #endregion

    #region End Game Screen Callbacks
    private void OnPlayAgain()
    {
        StateManager.Instance.ChangeState(GameState.Room);
    }

    private void OnLobby()
    {
        StateManager.Instance.ChangeState(GameState.Lobby);
    }

    private void OnMainMenu()
    {
        StateManager.Instance.ChangeState(GameState.Menu);
    }
    #endregion

    #region Target Events
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
        if(info.Player == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            targetsHit++;
            currentPlayerScore =+ info.Score;
            if (target.Data.Type == TargetType.Boss)
            {
                bossesDefeated++;
            }
        }
        currentSessionScore += info.Score;
        EventManager.OnScoreUpdated.Invoke(currentSessionScore);
    }
    #endregion

    #region Lives

    [PunRPC]
    private void RPCLoseLife()
    {
        LoseLife();
    }

    private void LoseLife()
    {
        if (Lives <= 0) return;
        Lives--;

        photonView.RPC(nameof(RPCUpdateLives), RpcTarget.All, Lives);
    }

    [PunRPC]
    private void RPCUpdateLives(int lives)
    {
        Lives = lives;
        EventManager.OnPlayerDamaged.Invoke(Lives);
        if (Lives == 0)
        {
            Debug.Log("No lives left. Game Over!");
            StateManager.Instance.ChangeState(GameState.EndGame);
        }
        else
        {
            Debug.Log($"Life lost! Remaining lives: {Lives}");
        }

    }
    #endregion
}
