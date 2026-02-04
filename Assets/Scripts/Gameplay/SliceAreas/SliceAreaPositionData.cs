using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceAreaData : MonoBehaviour
{
    [SerializeField]
    private int playerCount;
    [SerializeField]
    private List<SliceAreaPositionData> sliceAreaPositions;
    [SerializeField]
    private SliceAreaPosition startingSliceAreaPosition;

    public int PlayerCount => playerCount;
}

public class SliceAreaPositionData
{
    [SerializeField]
    public SliceAreaPosition[] sliceAreaPosition;
}
