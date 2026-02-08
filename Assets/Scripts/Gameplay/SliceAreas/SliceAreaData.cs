using System.Collections.Generic;
using UnityEngine;

public class SliceAreaData : MonoBehaviour
{
    [SerializeField]
    private int playerCount;
    [SerializeField]
    private List<SliceAreaPositionData> sliceAreaPositions;
    [SerializeField]
    private SliceAreaPositionData startingSliceAreaPosition;

    public SliceAreaPositionData StartingSliceArea => startingSliceAreaPosition;
    public List<SliceAreaPositionData> SliceAreaPositions => sliceAreaPositions;
    public int PlayerCount => playerCount;

    public SliceAreaPositionData GetRandomArea()
    {
        return sliceAreaPositions[Random.Range(0, sliceAreaPositions.Count)];
    }
}