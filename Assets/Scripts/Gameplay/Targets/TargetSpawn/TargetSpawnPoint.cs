using UnityEngine;

public class TargetSpawnPoint : MonoBehaviour
{
    [Header("Setup")]
    public Edge edge;
    [Range(0f, 1f)]
    public float normalizedPosition; // along the edge

    [Header("Deviation Profiles")]
    public SpawnDeviationProfile leftRightProfile;
    public SpawnDeviationProfile upDownProfile;

    public Vector2 GetLaunchDirection(Vector2 screenCenter)
    {
        Vector2 spawnPos = transform.position;
        Vector2 baseDir = (screenCenter - spawnPos).normalized;

        SpawnDeviationProfile profile = GetProfile();
        float t = GetInterpolationT();

        var (minDev, maxDev) = profile.Evaluate(t);

        float deviation = Random.Range(minDev, maxDev);
        deviation *= Random.value < 0.5f ? -1f : 1f;

        return Rotate(baseDir, deviation);
    }

    private SpawnDeviationProfile GetProfile()
    {
        return edge == Edge.Left || edge == Edge.Right
            ? leftRightProfile
            : upDownProfile;
    }

    private float GetInterpolationT()
    {
        // Distance from center of the edge
        float distFromCenter = Mathf.Abs(normalizedPosition - 0.5f) * 2f;
        return 1f - Mathf.Clamp01(distFromCenter);
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
