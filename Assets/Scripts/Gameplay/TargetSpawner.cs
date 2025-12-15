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

    private DifficultyData currentDifficultyData;
    private float difficultyTimer;
    private float spawnTimer;

    private void Awake()
    {
        FillDictionary();
        StartGame();
    }

    private void StartGame()
    {
        currentDifficultyData = new DifficultyData(baseDifficultyData);
        spawnTimer = 0f;
        difficultyTimer = 0f;
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
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnTimer += Time.deltaTime;
            difficultyTimer += Time.deltaTime;
            if (spawnTimer >= currentDifficultyData.TargetSpawnInterval)
            {
                spawnTimer = 0f;
                SpawnDefaultTarget();
            }

            if(difficultyTimer >= currentDifficultyData.SpawnIntervalDecreaseRate)
            {
                difficultyTimer = 0f;
                currentDifficultyData.TargetSpawnInterval = Mathf.Max(currentDifficultyData.MinSpawnInterval, currentDifficultyData.TargetSpawnInterval - currentDifficultyData.SpawnIntervalDecreaseAmount);
            }
        }
    }

    private void SpawnDefaultTarget()
    {
        TargetTemplate defaultTemplate = targetDatabase[TargetType.Default];

        if (defaultTemplate.target != null)
        {
            GameObject instantiatedTarget = PhotonNetwork.Instantiate(
                string.Format(Constants.Assets.TARGET_PREFAB_FORMAT, defaultTemplate.type),
                transform.position,
                Quaternion.identity
            );

            Target targetComponent = instantiatedTarget.GetComponent<Target>();

            if (targetComponent != null)
            {
                var spawnPoint = spawnPoints.GetRandom();
                Vector2 launchDirection = spawnPoint.GetLaunchDirection();

                TargetData targetData = new TargetData(1.0f, 1, 10, spawnPoint.transform.position,
                    launchDirection, TargetType.Default.ToString());
                targetComponent.Setup(targetData);
            }
        }
        else
        {
            Debug.LogError("DefaultTarget not found in the target database!");
        }
    }

}

[Serializable]
public struct TargetTemplate
{
    public TargetType type;
    public Target target;
}
