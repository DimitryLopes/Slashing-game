using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DifficultyPhase
{
    public float startTime;

    [Header("Spawn Pace")]
    public float spawnInterval;

    [Header("Spawn Weights")]
    public List<TargetSpawnWeight> spawnWeights;

    [Header("Active Caps")]
    public List<TargetActiveCap> activeCaps;

    public float areaSwapInterval;
}

[Serializable]
public class TargetSpawnWeight
{
    public TargetType type;
    public float weight;
}

[Serializable]
public class TargetActiveCap
{
    public TargetType type;
    public int maxActive;
}
