using UnityEngine;

public class SliceArea : Activateable
{
    [Header("Area Settings")]
    [SerializeField] private Color areaColor = Color.green;
    [SerializeField] private float areaAlpha = 0.2f;
    [SerializeField] private float borderThickness = 0.05f;
    [SerializeField] private float startingMovementDuration = 2f;

    private Vector3[] vertices = new Vector3[4];
    private Vector3[] startVertices = new Vector3[4];
    private Vector3[] targetVertices = new Vector3[4];

    private int moveTweenId = -1;
    public bool IsMoving => moveTweenId != -1;
    public bool IsAvailable => OwnerId == -1 && !IsMoving;
    public int OwnerId { get; private set; }
    public bool IsLocalPlayerArea { get; private set; }
    public Vector3[] Vertices => vertices;

    public void Initialize(int ownerId, Vector3[] initialVertices, bool isLocal)
    {
        OwnerId = ownerId;
        IsLocalPlayerArea = isLocal;

        for (int i = 0; i < 4; i++)
        {
            vertices[i] = Vector3.zero;
            targetVertices[i] = Vector3.zero;
        }

        if (moveTweenId != -1)
        {
            LeanTween.cancel(moveTweenId);
            moveTweenId = -1;
        }
    }

    public void MoveTo(Vector3[] newVertices, float duration)
    {
        if (newVertices == null || newVertices.Length != 4)
            return;

        if (moveTweenId != -1)
            LeanTween.cancel(moveTweenId);

        for (int i = 0; i < 4; i++)
        {
            startVertices[i] = vertices[i];
            targetVertices[i] = newVertices[i];
        }

        moveTweenId = LeanTween.value(gameObject, 0f, 1f, duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnUpdate((float t) =>
            {
                for (int i = 0; i < 4; i++)
                    vertices[i] = Vector3.Lerp(startVertices[i], targetVertices[i], t);
            })
            .setOnComplete(() =>
            {
                for (int i = 0; i < 4; i++)
                    vertices[i] = targetVertices[i];

                moveTweenId = -1;
            })
            .id;
    }

    public bool Contains(Vector2 point)
    {
        int j = 3;
        bool inside = false;

        for (int i = 0; i < 4; j = i++)
        {
            if (((vertices[i].y > point.y) != (vertices[j].y > point.y)) &&
                (point.x < (vertices[j].x - vertices[i].x) *
                 (point.y - vertices[i].y) /
                 (vertices[j].y - vertices[i].y) + vertices[i].x))
            {
                inside = !inside;
            }
        }

        return inside;
    }

    public bool IsSliceValid(Vector2 from, Vector2 to)
    {
        return Contains(from) && Contains(to);
    }

    public void Clear()
    {
        OwnerId = -1;
        IsLocalPlayerArea = false;
        vertices = new Vector3[4];
        targetVertices = new Vector3[4];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (vertices == null || vertices.Length != 4)
            return;

        Color drawColor = areaColor;
        drawColor.a = areaAlpha;
        Gizmos.color = drawColor;

        for (int i = 0; i < 4; i++)
            Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 4]);

        if (IsLocalPlayerArea)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < 4; i++)
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 4]);
        }
    }
#endif
}
