using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPun, IManager
{
    [SerializeField]
    private int initialLives = 3;
    [SerializeField]
    private TargetSpawner targetSpawner;
    [SerializeField, Header("Slice Areas")]
    private SliceArea sliceAreaPrefab;
    [SerializeField]
    private Transform sliceAreaContainer;
    [SerializeField]
    private SliceAreaData[] sliceAreaPresets;

    public static GameManager Instance { get; private set; }
    public int Lives { get; private set; }
    public bool IsInitialized { get; private set; }

    private List<SliceArea> sliceAreas = new List<SliceArea>();

    private string currentSliceareaName;
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

        if(Input.GetKeyDown(KeyCode.L))
        {
            AssignSliceAreas();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeSliceArea();
        }
    }

    #region Slice Area
    private void AssignSliceAreas()
    {
        SliceAreaData preset = GetSliceAreaPreset();
        SliceAreaPositionData startingData = preset.StartingSliceArea;

        SetSliceArea(startingData);
    }

    private void ChangeSliceArea()
    {
        var preset = GetSliceAreaPreset();
        SliceAreaPositionData areaPositionData = preset.GetRandomArea();
        
        SetSliceArea(areaPositionData);
    }
    private void SetSliceArea(SliceAreaPositionData startingData)
    {
        var players = PhotonNetwork.CurrentRoom.Players.Values;

        for (int i = 0; i < players.Count; i++)
        {
            int playerId = i + 1;
            var areaData = startingData.sliceAreaPosition[i];
            var area = GetSliceArea();
            bool isLocal = playerId == PhotonNetwork.LocalPlayer.ActorNumber;
            area.Initialize(playerId, areaData.Positions, isLocal);
        }
    }


    private SliceAreaData GetSliceAreaPreset()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        SliceAreaData preset = sliceAreaPresets[playerCount - 1];
        return preset;
    }

    private SliceArea GetSliceArea()
    {
        foreach(var area in sliceAreas)
        {
            if (area.IsMoving) continue;

            return area;
        }

        SliceArea sliceArea = Instantiate(sliceAreaPrefab, sliceAreaContainer);
        sliceAreas.Add(sliceArea);
        return sliceArea;
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
