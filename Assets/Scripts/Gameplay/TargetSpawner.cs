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

    private Action OnTargetMissed;
    private Action<float> OnTargetHit;
    private Action<HitInfo, byte, float> OnPlayerSpecificTargetHit;
    private Action<float> OnBombHit;


    private void Awake()
    {
        FillDictionary();
    }

    public void EnableSpawn(Action<float> onTargetHit, Action onTargetMiss,
        Action<HitInfo,byte,float> onPlayerSpecificTargetHit, Action<float> onBombHit)
    {
        OnTargetHit = onTargetHit;
        OnTargetMissed = onTargetMiss;
        OnPlayerSpecificTargetHit = onPlayerSpecificTargetHit;
        OnBombHit = onBombHit;
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
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SpawnTarget(TargetType.Default, OnTargetHit, OnTargetMissed);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            SpawnTarget(TargetType.Explosive, OnBombHit, OnTargetMissed);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            SpawnPlayerTarget(TargetType.SpecificPlayer, OnPlayerSpecificTargetHit, OnTargetMissed, 1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnPlayerTarget(TargetType.SpecificPlayer, OnPlayerSpecificTargetHit, OnTargetMissed, 2);
        }



        if (true) return; //!can spawn

        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;
        if (spawnTimer >= currentDifficultyData.TargetSpawnInterval)
        {
            spawnTimer = 0f;
            
            SpawnTarget(TargetType.Default, OnTargetHit, OnTargetMissed);
        }

        if (difficultyTimer < currentDifficultyData.SpawnIntervalDecreaseRate) return;

        difficultyTimer = 0f;
        currentDifficultyData.TargetSpawnInterval = Mathf.Max(currentDifficultyData.MinSpawnInterval,
            currentDifficultyData.TargetSpawnInterval - currentDifficultyData.SpawnIntervalDecreaseAmount);
    }

    private void SpawnTarget(TargetType type, Action<float> onHit, Action onMiss)
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
                    launchDirection, type.ToString(), template.minScore, template.maxScore);
                targetComponent.Setup(targetData, onHit, onMiss);
            }
        }
        else
        {
            Debug.LogError($"{type} not found in the target database!");
        }
    }

    private void SpawnPlayerTarget(TargetType type, Action<HitInfo, byte, float> onHit, Action onMiss, byte player)
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
                    launchDirection, type.ToString(), template.minScore, template.maxScore);
                targetComponent.Setup(targetData, player, onHit, onMiss);
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
