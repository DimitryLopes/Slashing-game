using System;
using UnityEngine;


[Serializable]
public class SliceAreaPosition
{
    [SerializeField]
    private Vector3[] positions;

    public Vector3[] Positions => positions;
}
