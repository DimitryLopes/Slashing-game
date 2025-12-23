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

    private void Awake()
    {
        FillDictionary();
    }

    public void EnableSpawn(Action<float> onTargetHit, Action onTargetMiss)
    {
        OnTargetHit = onTargetHit;
        OnTargetMissed = onTargetMiss;
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
        if (!canSpawn) return;

        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;
        if (spawnTimer >= currentDifficultyData.TargetSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnDefaultTarget();
        }

        if (difficultyTimer < currentDifficultyData.SpawnIntervalDecreaseRate) return;

        difficultyTimer = 0f;
        currentDifficultyData.TargetSpawnInterval = Mathf.Max(currentDifficultyData.MinSpawnInterval,
            currentDifficultyData.TargetSpawnInterval - currentDifficultyData.SpawnIntervalDecreaseAmount);
    }

    private void SpawnDefaultTarget()
    {
        TargetTemplate defaultTemplate = targetDatabase[TargetType.Default];

        if (defaultTemplate.target != null)
        {
            DefaultTarget targetComponent = GetAvailableTarget<DefaultTarget>(TargetType.Default);

            if (targetComponent != null)
            {
                var spawnPoint = spawnPoints.GetRandom();
                Vector2 launchDirection = spawnPoint.GetLaunchDirection();

                TargetData targetData = new TargetData(1.0f, 1, 10, spawnPoint.transform.position,
                    launchDirection, TargetType.Default.ToString(), defaultTemplate.minScore, defaultTemplate.maxScore);
                targetComponent.Setup(targetData, OnTargetHit, OnTargetMissed);
            }
        }
        else
        {
            Debug.LogError("DefaultTarget not found in the target database!");
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
