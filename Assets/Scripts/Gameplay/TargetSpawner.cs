using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private List<TargetSpawnPoint> spawnPoints;
    [SerializeField] private List<TargetTemplate> targetTemplates;
    [SerializeField] private DifficultyProfile difficultyProfile;

    private Dictionary<TargetType, TargetTemplate> targetDatabase;
    private Dictionary<TargetType, List<Target>> instantiatedTargets;

    private float elapsedTime;
    private float spawnTimer;
    private bool canSpawn;

    private DifficultyPhase currentPhase;

    private void Awake()
    {
        InitializeDatabases();
    }

    public void EnableSpawn()
    {
        elapsedTime = 0f;
        spawnTimer = 0f;
        canSpawn = true;
        currentPhase = difficultyProfile.phases[0];
    }

    public void DisableSpawn()
    {
        canSpawn = false;
        DeactivateAllTargets();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SpawnTarget(TargetType.Default);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            SpawnTarget(TargetType.Explosive);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            SpawnPlayerTarget(TargetType.SpecificPlayer, 1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnPlayerTarget(TargetType.SpecificPlayer, 2);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            SpawnTarget(TargetType.Boss);
        }

        return;

        if (!canSpawn) return;

        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        currentPhase = difficultyProfile.GetPhase(elapsedTime);

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

        TargetSpawnPoint spawnPoint = spawnPoints.GetRandom();
        Vector2 launchDirection = spawnPoint.GetLaunchDirection();

        TargetData data = new TargetData(
            1f,
            1,
            10,
            spawnPoint.transform.position,
            launchDirection,
            type.ToString(),
            template.minScore,
            template.maxScore,
            type
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
            )
            .GetComponent<T>();

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
}


[Serializable]
public struct TargetTemplate
{
    public TargetType type;
    public Target target;
    public float minScore;
    public float maxScore;
}
