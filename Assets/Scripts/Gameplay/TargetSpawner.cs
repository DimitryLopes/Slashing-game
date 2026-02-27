using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private List<TargetSpawnPoint> spawnPoints;
    [SerializeField] private List<TargetTemplate> targetTemplates;
    [SerializeField] private DifficultyProfile difficultyProfile;
    [SerializeField] private Transform spawnContainer;

    private Dictionary<TargetType, TargetTemplate> targetDatabase;
    private Dictionary<TargetType, List<Target>> instantiatedTargets;

    private float elapsedTime;
    private float spawnTimer;
    private float areaSwapTimer;

    private bool isPlaying;
    private bool isAreaSwapping;
    private bool CanSpawn => isPlaying && !isAreaSwapping;

    private DifficultyPhase currentPhase;

    private void Awake()
    {
        InitializeDatabases();
    }

    public void EnableSpawn()
    {
        elapsedTime = 0f;
        spawnTimer = 0f;
        isPlaying = true;
        currentPhase = difficultyProfile.phases[0];
        EventManager.OnSliceAreaMoved.AddListener(OnSliceAreaMoved);
    }

    public void DisableSpawn()
    {
        isPlaying = false;
        DeactivateAllTargets();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Q))
        //{
        //    SpawnTarget(TargetType.Default);
        //}

        //if(Input.GetKeyDown(KeyCode.W))
        //{
        //    SpawnTarget(TargetType.Explosive);
        //}

        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    SpawnPlayerTarget(TargetType.SpecificPlayer, 1);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SpawnPlayerTarget(TargetType.SpecificPlayer, 2);
        //}

        //if(Input.GetKeyDown(KeyCode.T))
        //{
        //    SpawnTarget(TargetType.Boss);
        //}

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    SpawnTestTarget();
        //}

        if (!isPlaying) return;

        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        areaSwapTimer += Time.deltaTime;

        currentPhase = difficultyProfile.GetPhase(elapsedTime);

        if(areaSwapTimer >= currentPhase.areaSwapInterval)
        {
            areaSwapTimer = 0f;
            isAreaSwapping = true;
            EventManager.OnSliceAreaMoveTimerEnded.Invoke();
        }

        if (spawnTimer < currentPhase.spawnInterval) return;

        spawnTimer = 0f;

        TargetType rolledType = RollSpawnType(currentPhase.spawnWeights);

        if (!CanSpawnType(rolledType))
            return;

        switch (rolledType)
        {
            case TargetType.SpecificPlayer:
                int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
                SpawnPlayerTarget(rolledType, UnityEngine.Random.Range(0, playerCount));
                break;
            default:
                SpawnTarget(rolledType);
                break;

        }
    }

    #region Target Management
    private void InitializeDatabases()
    {
        targetDatabase = new Dictionary<TargetType, TargetTemplate>();
        instantiatedTargets = new Dictionary<TargetType, List<Target>>();

        foreach (var template in targetTemplates)
            targetDatabase[template.type] = template;

        foreach (TargetType type in Enum.GetValues(typeof(TargetType)))
            instantiatedTargets[type] = new List<Target>();
    }

    private TargetType RollSpawnType(List<TargetSpawnWeight> weights)
    {
        float totalWeight = 0f;
        foreach (var entry in weights)
            totalWeight += entry.weight;

        float roll = UnityEngine.Random.Range(0f, totalWeight);

        float cumulative = 0f;
        foreach (var entry in weights)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.type;
        }

        return weights[0].type;
    }

    private bool CanSpawnType(TargetType type)
    {
        int activeCount = 0;
        foreach (var target in instantiatedTargets[type])
        {
            if (target.gameObject.activeInHierarchy)
                activeCount++;
        }

        foreach (var cap in currentPhase.activeCaps)
        {
            if (cap.type == type)
                return activeCount < cap.maxActive;
        }

        return true;
    }

    private void SpawnTarget(TargetType type)
    {
        if (!targetDatabase.TryGetValue(type, out var template))
            return;

        Target target = GetAvailableTarget<Target>(type);
        if (target == null)
            return;

        SliceAreaPositionData areaData = GameManager.Instance.CurrentSliceAreaData;
        var targetPoint = areaData.EntireArea.GetRandomPointInArea(Constants.Targets.TARGET_AREA_TRESHOLD);

        Vector2 startPoint = spawnPoints.GetRandom().transform.position;

        float timeToTarget = 1.2f;
        Vector2 gravity = Physics2D.gravity;

        Vector2 initialVelocity =
            (targetPoint - startPoint - 0.5f * gravity * timeToTarget * timeToTarget)
            / timeToTarget;

        TargetData data = new TargetData(
            1f,                     // size
            1,                      // health
            initialVelocity.magnitude, // speed (optional, if used elsewhere)
            startPoint,             // start position
            initialVelocity.normalized, // launch direction
            type.ToString(),
            template.minScore,
            template.maxScore,
            type
        );

        target.Setup(data);
    }

    private void SpawnTestTarget()
    {
        Target target = GetAvailableTarget<Target>(TargetType.Default);
        if (target == null)
            return;
        TargetSpawnPoint spawnPoint = spawnPoints.GetRandom();
        Vector2 launchDirection = spawnPoint.GetLaunchDirection();
        TargetData data = new TargetData(
            1f,
            1,
            0,
            spawnPoint.transform.position,
            launchDirection,
            TargetType.Default.ToString(),
            0,
            0,
            TargetType.Default
        );
        target.Setup(data);
    }

    private void SpawnPlayerTarget(TargetType type, int player)
    {
        TargetTemplate template = targetDatabase[type];

        PlayerSpecificTarget targetComponent = GetAvailableTarget<PlayerSpecificTarget>(type);
        if (targetComponent != null)
        {
            var spawnPoint = spawnPoints.GetRandom();
            Vector2 launchDirection = spawnPoint.GetLaunchDirection();
            TargetData targetData = new TargetData(1.0f, 1, 10, spawnPoint.transform.position,
                launchDirection, type.ToString(), template.minScore, template.maxScore, type);
            targetComponent.Setup(targetData, player);
        }
    }

    private T GetAvailableTarget<T>(TargetType type) where T : Target
    {
        foreach (T target in instantiatedTargets[type])
        {
            if (!target.gameObject.activeInHierarchy)
                return target;
        }

        T newTarget = PhotonNetwork
            .Instantiate(
                string.Format(Constants.Assets.TARGET_PREFAB_FORMAT, type),
                transform.position,
                Quaternion.identity
            ).GetComponent<T>();

        newTarget.transform.SetParent(spawnContainer);
        instantiatedTargets[type].Add(newTarget);
        return newTarget;
    }

    private void DeactivateAllTargets()
    {
        foreach (var list in instantiatedTargets.Values)
        {
            foreach (var target in list)
                target.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Area Swap Handling
    private void OnSliceAreaMoved(SliceArea area)
    {
        isAreaSwapping = false;
    }
    #endregion
}


[Serializable]
public struct TargetTemplate
{
    public TargetType type;
    public Target target;
    public float minScore;
    public float maxScore;
}
