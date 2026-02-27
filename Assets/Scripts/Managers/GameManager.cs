using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
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
    [SerializeField]
    private GameObject playerControllerPrefab;

    public static GameManager Instance { get; private set; }
    public int Lives { get; private set; }
    public bool IsInitialized { get; private set; }
    public SliceAreaPositionData CurrentSliceAreaData { get; private set; }
    private PlayerController playerController;

    private List<SliceArea> sliceAreas = new List<SliceArea>();

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

        foreach(var preset in sliceAreaPresets)
        {
            foreach(var area in preset.SliceAreaPositions)
            {
                area.Setup();
            }
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
        if (playerController == null)
        {
            playerController = PhotonNetwork.Instantiate(
                playerControllerPrefab.name,
                Vector3.zero,
                Quaternion.identity).GetComponent<PlayerController>();

        }

        ClearGameStats();
        AssignSliceAreas();        

        if (PhotonNetwork.IsMasterClient)
        {
            targetSpawner.EnableSpawn();
            EventManager.OnSliceAreaMoveTimerEnded.AddListener(ChangeSliceArea);
        }     

        isPlaying = true;
    }

    public void EndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            EventManager.OnSliceAreaMoveTimerEnded.RemoveListener(ChangeSliceArea);
        }

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

        foreach(SliceArea sliceArea in sliceAreas)
        {
            sliceArea.Clear();
        }

        playerController.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying) return;
        timeElapsed += Time.deltaTime;
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

    private void SetSliceArea(SliceAreaPositionData data)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        CurrentSliceAreaData = data;

        var players = PhotonNetwork.PlayerList;
        int playerCount = players.Length;

        List<int> areaIndices = new List<int>();
        for (int i = 0; i < playerCount; i++)
            areaIndices.Add(i);

        areaIndices.Shuffle();

        List<int> actorNumbers = new List<int>();
        for (int i = 0; i < playerCount; i++)
            actorNumbers.Add(players[i].ActorNumber);

        float[] serializedPositions = SerializeAreas(data);

        photonView.RPC(
            nameof(RPCApplySliceAreas),
            RpcTarget.All,
            actorNumbers.ToArray(),
            areaIndices.ToArray(),
            serializedPositions
        );
    }

    private SliceAreaData GetSliceAreaPreset()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        SliceAreaData preset = sliceAreaPresets[playerCount - 1];
        return preset;
    }

    private SliceArea GetSliceArea(int playerId)
    {
        bool isLocal = playerId == PhotonNetwork.LocalPlayer.ActorNumber;

        foreach (var area in sliceAreas)
        {
            if (area.IsAvailable || area.OwnerId == playerId)  return area;
        }

        SliceArea sliceArea = Instantiate(sliceAreaPrefab, sliceAreaContainer);
        sliceAreas.Add(sliceArea);
        sliceArea.Initialize(playerId, new Vector3[4], isLocal);
        return sliceArea;
    }

    #region RPC
    private float[] SerializeAreas(SliceAreaPositionData data)
    {
        List<float> serialized = new List<float>();

        foreach (var area in data.sliceAreaPosition)
        {
            foreach (var pos in area.Positions)
            {
                serialized.Add(pos.x);
                serialized.Add(pos.y);
            }
        }

        return serialized.ToArray();
    }

    [PunRPC]
    private void RPCApplySliceAreas(int[] actorNumbers, int[] areaIndices, float[] serializedPositions)
    {
        int areaCount = areaIndices.Length;
        int verticesPerArea = serializedPositions.Length / (areaCount * 2);

        Vector3[][] deserializedAreas = new Vector3[areaCount][];

        int index = 0;

        for (int i = 0; i < areaCount; i++)
        {
            Vector3[] positions = new Vector3[verticesPerArea];

            for (int j = 0; j < verticesPerArea; j++)
            {
                positions[j] = new Vector3(
                    serializedPositions[index],
                    serializedPositions[index + 1]
                );

                index += 2;
            }

            deserializedAreas[i] = positions;
        }

        for (int i = 0; i < actorNumbers.Length; i++)
        {
            int actorNumber = actorNumbers[i];
            int areaIndex = areaIndices[i];

            var area = GetSliceArea(actorNumber);
            if(area.OwnerId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerController.Setup(area);
            }
            area.MoveTo(deserializedAreas[areaIndex], 2f);
        }

        if (ScreenManager.Instance.ActiveScreen is GameScreen) return;

        var controller = new GameScreenController(Lives, sliceAreas);
        ScreenManager.Instance.Show<GameScreen>(controller);
    }

    #endregion

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
