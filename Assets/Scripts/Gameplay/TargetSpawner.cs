using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField]
    private List<TargetSpawnPoint> spawnPoints;
    [SerializeField]
    private List<TargetTemplate> targetTemplates;
    [SerializeField]
    private DifficultyData baseDifficultyData;

    private Dictionary<TargetType, TargetTemplate> targetDatabase;
    private Dictionary<TargetType, List<Target>> intantiatedTargets;

    private DifficultyData currentDifficultyData;
    private float difficultyTimer;
    private float spawnTimer;
    private bool canSpawn;


    private void Awake()
    {
        FillDictionary();
    }

    public void EnableSpawn()
    {
        currentDifficultyData = new DifficultyData(baseDifficultyData);
        spawnTimer = 0f;
        difficultyTimer = 0f;
        canSpawn = true;
    }

    public void DisableSpawn()
    {
        canSpawn = false;
        DeactivateAllTargets();
    }

    private void FillDictionary()
    {
        targetDatabase = new Dictionary<TargetType, TargetTemplate>();
        foreach (var template in targetTemplates)
        {
            if (!targetDatabase.ContainsKey(template.type))
            {
                targetDatabase.Add(template.type, template);
            }
            else
            {
                Debug.LogWarning($"Duplicate target type found in database: {template.type}");
            }
        }

        intantiatedTargets = new Dictionary<TargetType, List<Target>>();
        foreach (TargetType type in Enum.GetValues(typeof(TargetType)))
        {
            intantiatedTargets[type] = new List<Target>();
        }
    }

    private void Update()
    {
        /*
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
        */

        if (!canSpawn) return;

        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;
        if (spawnTimer >= currentDifficultyData.TargetSpawnInterval)
        {
            spawnTimer = 0f;
            
            SpawnTarget(TargetType.Default);
        }

        if (difficultyTimer < currentDifficultyData.SpawnIntervalDecreaseRate) return;

        difficultyTimer = 0f;
        currentDifficultyData.TargetSpawnInterval = Mathf.Max(currentDifficultyData.MinSpawnInterval,
            currentDifficultyData.TargetSpawnInterval - currentDifficultyData.SpawnIntervalDecreaseAmount);
    }

    private void SpawnTarget(TargetType type)
    {
        TargetTemplate template = targetDatabase[type];

        if (template.target != null)
        {
            Target targetComponent = GetAvailableTarget<Target>(type);
            if (targetComponent != null)
            {
                var spawnPoint = spawnPoints.GetRandom();
                Vector2 launchDirection = spawnPoint.GetLaunchDirection();
                TargetData targetData = new TargetData(1.0f, 1, 10, spawnPoint.transform.position,
                    launchDirection, type.ToString(), template.minScore, template.maxScore, type);
                targetComponent.Setup(targetData);
            }
        }
        else
        {
            Debug.LogError($"{type} not found in the target database!");
        }
    }

    private void SpawnPlayerTarget(TargetType type, byte player)
    {
        TargetTemplate template = targetDatabase[type];
        if (template.target != null)
        {
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
        else
        {
            Debug.LogError($"{type} not found in the target database!");
        }
    }


    private T GetAvailableTarget<T>(TargetType targetType) where T : Target
    {
        foreach (T target in intantiatedTargets[targetType])
        {
            if (!target.gameObject.activeInHierarchy)
            {
                return target;
            }
        }

        T newTarget = PhotonNetwork.Instantiate(
                string.Format(Constants.Assets.TARGET_PREFAB_FORMAT, targetType),
                transform.position,
                Quaternion.identity
            ).GetComponent<T>();
        intantiatedTargets[targetType].Add(newTarget);
        return newTarget;
    }

    private void DeactivateAllTargets()
    {
        foreach (var kvp in intantiatedTargets)
        {
            foreach (Target target in intantiatedTargets[kvp.Key])
            {
                target.gameObject.SetActive(false);
            }
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
