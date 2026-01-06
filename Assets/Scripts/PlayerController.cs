using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private GameObject bladePrefab;
    [SerializeField] private LayerMask targetMask;

    private GameObject playerBlade;
    private Camera mainCamera;

    private Vector2 lastMousePos;
    private bool isCutting;

    private readonly HashSet<Collider2D> hitsThisFrame = new();
    private readonly RaycastHit2D[] raycastBuffer = new RaycastHit2D[1];
    private readonly Dictionary<Collider2D, Vector2> lastHitPoint = new();
    private readonly Dictionary<Collider2D, HitInfo> activeStrikes = new();

    private byte PlayerId => (byte)photonView.OwnerActorNr;

    void Start()
    {
        mainCamera = Camera.main;

        if (photonView.IsMine)
            playerBlade = Instantiate(bladePrefab, transform);
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        Vector2 mouseWorld = GetMouseWorld();

        if (Input.GetMouseButtonDown(0))
        {
            isCutting = true;
            lastMousePos = mouseWorld;
        }

        if (Input.GetMouseButton(0) && isCutting)
        {
            PerformLinecast(lastMousePos, mouseWorld);
            MoveBlade(mouseWorld);
            photonView.RPC(nameof(UpdateBladePosition), RpcTarget.Others, mouseWorld);
            lastMousePos = mouseWorld;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isCutting = false;
        }
    }

    private void PerformLinecast(Vector2 from, Vector2 to)
    {
        hitsThisFrame.Clear();

        RaycastHit2D[] hits = Physics2D.LinecastAll(from, to, targetMask);

        foreach (var hit in hits)
        {
            Collider2D col = hit.collider;
            hitsThisFrame.Add(col);

            if (!activeStrikes.ContainsKey(col))
            {
                activeStrikes[col] = new HitInfo(hit.point, PlayerId);
            }

            lastHitPoint[col] = hit.point;
        }

        EndExitedStrikes(to);
    }

    private void EndExitedStrikes(Vector2 exitPoint)
    {
        List<Collider2D> finished = new();

        foreach (var kvp in activeStrikes)
        {
            if (!hitsThisFrame.Contains(kvp.Key))
            {
                finished.Add(kvp.Key);
            }
        }

        foreach (var col in finished)
        {
            if (!col) continue;

            Target target = col.GetComponent<Target>();
            if (!target || target.IsCutted) continue;

            HitInfo info = activeStrikes[col];

            Vector2 entry = info.EntryPoint;

            Vector2 dir = (GetMouseWorld() - lastMousePos).normalized;
            if (dir.sqrMagnitude < 0.0001f)
                dir = (lastHitPoint[col] - entry).normalized;

            if (col is CircleCollider2D circle)
            {
                info.ExitPoint = GetCircleExitPoint(circle, entry, dir);
            }
            else
            {
                info.ExitPoint = col.ClosestPoint(entry + dir * 100f);
            }

            target.Hit(info);

            activeStrikes.Remove(col);
            lastHitPoint.Remove(col);
        }
    }

    private static Vector2 GetCircleExitPoint(CircleCollider2D circle, Vector2 entryPoint, Vector2 direction)
    {
        Vector2 center = circle.bounds.center;
        float radius = circle.radius * circle.transform.lossyScale.x;

        direction.Normalize();

        // Solve: |(entry + t*dir) - center| = radius
        Vector2 oc = entryPoint - center;

        float b = 2f * Vector2.Dot(oc, direction);
        float c = Vector2.Dot(oc, oc) - radius * radius;

        float discriminant = b * b - 4f * c;

        if (discriminant < 0f)
            return entryPoint;

        float sqrt = Mathf.Sqrt(discriminant);

        float t = (-b + sqrt) / 2f;

        return entryPoint + direction * t;
    }

    private Vector2 GetMouseWorld()
    {
        Vector3 p = Input.mousePosition;
        p.z = 10f;
        return mainCamera.ScreenToWorldPoint(p);
    }

    private void MoveBlade(Vector2 position)
    {
        transform.position = position;
    }

    [PunRPC]
    private void UpdateBladePosition(Vector2 position)
    {
        if (!playerBlade)
            playerBlade = Instantiate(bladePrefab, transform);

        playerBlade.transform.position = position;
    }
}
