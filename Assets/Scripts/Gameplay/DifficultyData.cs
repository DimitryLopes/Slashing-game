using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Difficulty/Difficulty Profile")]
public class DifficultyProfile : ScriptableObject
{
    public List<DifficultyPhase> phases;

    public DifficultyPhase GetPhase(float elapsedTime)
    {
        DifficultyPhase current = phases[0];

        foreach (var phase in phases)
        {
            if (elapsedTime >= phase.startTime)
                current = phase;
            else
                break;
        }

        return current;
    }
}
