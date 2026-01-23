using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public abstract class Target : MonoBehaviourPun, IPunObservable
{
    public bool IsCutted { get; protected set; } = false;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected Rigidbody2D rb;

    private TargetData targetData;

    public TargetData Data => targetData;

    public float Size => spriteRenderer.bounds.extents.magnitude;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
            CheckOutOfBounds();        
    }

    private void CheckOutOfBounds()
    {
        Vector3 p = transform.position;

        if (p.y < -13f || p.y > 13f || p.x < -13f || p.x > 13f)
        {
            gameObject.SetActive(false);
            OnMiss();
        }
    }

    public virtual void Hit(HitInfo info)
    {
        if (IsCutted) return;

        IsCutted = true;
        float score = CalculateScore(info);
        info.Score = score;
        ExecuteHit(info);
        photonView.RPC(nameof(RPCHit), RpcTarget.Others, info.Player, info.EntryPoint, info.ExitPoint, info.Score);
    }

    [PunRPC]
    public void RPCHit(byte player, Vector2 entryPoint, Vector2 exitPoint, float score)
    {
        IsCutted = true;
        var info = new HitInfo(entryPoint, player);
        info.ExitPoint = exitPoint;
        info.Score = score;
        ExecuteHit(info);
    }

    protected virtual void ExecuteHit(HitInfo info)
    {
        SpriteSlicer.Instance.Slice(spriteRenderer, info.EntryPoint, info.ExitPoint);
        
        EventManager.OnTargetHit.Invoke(this, info);
        FloatingTextManager.Instance.ShowFloatingText($"+ {info.Score}", transform.position);
        gameObject.SetActive(!IsCutted);
        OnHit(info);
    }

    protected float CalculateScore(HitInfo info)
    {
        Vector3 entry = info.EntryPoint;
        Vector3 exit = info.ExitPoint;
        Vector2 center = spriteRenderer.bounds.center;
        Vector2 cutMid = (entry + exit) / 2f;
        float distance = Vector2.Distance(center, cutMid);

        gizmoList.Add(new DebugGizmo
        {
            entry = entry,
            exit = exit,
            mid = cutMid,
            center = center,
            expireTime = Time.time + 5f
        });

        float radius = Mathf.Min(
            spriteRenderer.bounds.extents.x,
            spriteRenderer.bounds.extents.y
        );

        float normalized = Mathf.Clamp01(distance / radius);

        float score = Mathf.Lerp(targetData.MaxScore, targetData.MinScore, normalized);
        score = Mathf.Floor(score);
        return score; 
    }

    public void Setup(TargetData data)
    {
        ExecuteSetup(data);
        if (photonView.IsMine) { }
            rb.velocity = data.LaunchDirection * data.Speed;

        photonView.RPC(nameof(RPCSetup), RpcTarget.Others, data.Size, data.Health,
            data.Speed, data.SpriteKey, data.LaunchDirection, data.StartPosition, data.MinScore,
            data.MaxScore, data.Type);
    }

    [PunRPC]
    protected void RPCSetup(float size, float hp, float speed, string spriteKey,
        Vector2 launchDirection, Vector3 startPosition, float minScore, float maxScore,
        TargetType targetType)
    {
        transform.position = startPosition;
        TargetData data = new TargetData(size, hp, speed, startPosition,
            launchDirection, spriteKey, minScore, maxScore, targetType);
        ExecuteSetup(data);
    }

    private void ExecuteSetup(TargetData data)
    {
        targetData = data;
        IsCutted = false;
        OnSetup(data);
        spriteRenderer.sprite = AssetService.GetTargetSprite(data.SpriteKey);
        transform.position = data.StartPosition;
        transform.localScale = Vector3.one * data.Size;
        gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsCutted);
        }
        else
        {
            IsCutted = (bool)stream.ReceiveNext();
        }
    }

    protected virtual void OnMiss()
    {
        EventManager.OnTargetMiss.Invoke(this);
    }
    
    protected abstract void OnHit(HitInfo hitInfo);

    protected virtual void OnSetup(TargetData data) { }

    private struct DebugGizmo
    {
        public Vector3 entry;
        public Vector3 exit;
        public Vector3 mid;
        public Vector3 center;
        public float expireTime;
    }

    private List<DebugGizmo> gizmoList = new List<DebugGizmo>();


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (gizmoList == null) return;

        float now = Application.isPlaying ? Time.time : 0f;
        gizmoList.RemoveAll(g => g.expireTime < now);

        foreach (var g in gizmoList)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(g.entry, 0.15f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(g.exit, 0.15f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(g.mid, 0.15f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(g.center, 0.15f);
        }
    }
#endif
}
