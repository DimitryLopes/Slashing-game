using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetGenerator : MonoBehaviour
{
    [SerializeField]
    private List<TargetTemplate> targetTemplates;
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

    [SerializeField]
    private Transform spawnPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnDefaultTarget();
        }
    }

    private void SpawnDefaultTarget()
    {
        TargetTemplate defaultTemplate = targetDatabase[TargetType.Default];

        if (defaultTemplate.target != null)
        {
            GameObject instantiatedTarget = PhotonNetwork.Instantiate(
                string.Format(Constants.Assets.TARGET_PREFAB_FORMAT, defaultTemplate.type),
                spawnPoint.position,
                Quaternion.identity
            );

            Target targetComponent = instantiatedTarget.GetComponent<Target>();

            if (targetComponent != null)
            {
                TargetData targetData = new TargetData(1.0f, 1, 5.0f, spawnPoint.position, TargetType.Default.ToString()); // Exemplo de TargetData
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
