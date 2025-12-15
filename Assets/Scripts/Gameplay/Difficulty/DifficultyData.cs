using System;
using UnityEngine;

[Serializable]
public class DifficultyData
{
    [SerializeField]
    public float TargetSpawnInterval = 1.0f;
    [SerializeField]
    public float SpawnIntervalDecreaseRate;
    [SerializeField]
    public float SpawnIntervalDecreaseAmount;
    [SerializeField]
    public float MinSpawnInterval;
    [SerializeField]
    public float ScoreMultiplier;

    public DifficultyData(DifficultyData other)
    {
        TargetSpawnInterval = other.TargetSpawnInterval;
        SpawnIntervalDecreaseRate = other.SpawnIntervalDecreaseRate;
        SpawnIntervalDecreaseAmount = other.SpawnIntervalDecreaseAmount;
        MinSpawnInterval = other.MinSpawnInterval;
        ScoreMultiplier = other.ScoreMultiplier;
    }
}
