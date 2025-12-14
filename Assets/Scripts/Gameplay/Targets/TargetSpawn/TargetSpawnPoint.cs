using UnityEngine;

public class TargetSpawnPoint : MonoBehaviour
{
    [Header("Setup")]
    public Edge edge;
    [Range(0f, 1f)]
    public float normalizedPosition;

    [Header("Deviation Objects")]
    public Transform minObject;
    public Transform middleObject;
    public Transform maxObject;

    public Vector2 GetLaunchDirection()
    {
        Vector2 spawnPos = transform.position;

        Vector2 baseDir = ((Vector2)middleObject.position - spawnPos).normalized;

        float lowerAngle = CalculateAngle(spawnPos, minObject.position, baseDir);
        float upperAngle = CalculateAngle(spawnPos, maxObject.position, baseDir);

        float deviation = Random.Range(lowerAngle, upperAngle);

        return Rotate(baseDir, deviation);
    }

    private float CalculateAngle(Vector2 origin, Vector2 target, Vector2 baseDir)
    {
        Vector2 dir = (target - origin).normalized;
        return Vector2.SignedAngle(baseDir, dir);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 spawnPos = transform.position;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(spawnPos, middleObject.position);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(spawnPos, minObject.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnPos, maxObject.position);
    }
#endif
}
