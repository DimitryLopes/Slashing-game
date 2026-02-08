using System;
using UnityEngine;


[Serializable]
public class SliceAreaPosition
{
    [SerializeField]
    private Vector3[] positions;

    public Vector3[] Positions => positions;

    public SliceAreaPosition(Vector3[] positions)
    {
        this.positions = positions;
    }

    public Vector2 GetRandomPointInArea(float inset01)
    {
        if (positions == null || positions.Length < 3)
            throw new InvalidOperationException("Polygon must have at least 3 vertices.");

        inset01 = Mathf.Clamp01(inset01);

        Vector2 center = GetCentroid();

        int triangleCount = positions.Length - 2;
        int triIndex = UnityEngine.Random.Range(0, triangleCount);

        Vector2 a = Vector2.Lerp(center, positions[0], inset01);
        Vector2 b = Vector2.Lerp(center, positions[triIndex + 1], inset01);
        Vector2 c = Vector2.Lerp(center, positions[triIndex + 2], inset01);

        float r1 = Mathf.Sqrt(UnityEngine.Random.value);
        float r2 = UnityEngine.Random.value;

        return (1 - r1) * a + r1 * (1 - r2) * b + r1 * r2 * c;
    }

    private Vector2 GetCentroid()
    {
        Vector2 sum = Vector2.zero;
        foreach (var p in positions)
            sum += (Vector2)p;
        return sum / positions.Length;
    }
}

