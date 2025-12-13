using UnityEngine;
using System;

[Serializable]
public class SpawnDeviationProfile
{
    public float minAtEdge = 45f;
    public float maxAtEdge = 45f;

    public float minAtCenter = 45f;
    public float maxAtCenter = 45f;

    public AnimationCurve interpolation = AnimationCurve.Linear(0, 0, 1, 1);

    public (float min, float max) Evaluate(float t)
    {
        float k = interpolation.Evaluate(t);
        float min = Mathf.Lerp(minAtEdge, minAtCenter, k);
        float max = Mathf.Lerp(maxAtEdge, maxAtCenter, k);
        return (min, max);
    }
}
