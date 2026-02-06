using System;
using UnityEngine;

[Serializable]
public class SliceAreaPositionData
{
    [SerializeField]
    private string name; //for identification in inspector
    [SerializeField]
    public SliceAreaPosition[] sliceAreaPosition;
}
