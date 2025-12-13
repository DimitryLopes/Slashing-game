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
    private Transform middle;

    private Dictionary<TargetType, TargetTemplate> targetDatabase;

    private void Awake()
    {
        FillDictionary();
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SpawnDefaultTarget();
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
                middle.position,
                Quaternion.identity
            );

            Target targetComponent = instantiatedTarget.GetComponent<Target>();

            if (targetComponent != null)
            {
                var spawnPoint = spawnPoints.GetRandom();
                Vector2 launchDirection = spawnPoint.GetLaunchDirection(middle.position);

                TargetData targetData = new TargetData(1.0f, 1, 5.0f, spawnPoint.transform.position,
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
