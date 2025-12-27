using UnityEngine;
using Photon.Pun;
using System;

public abstract class Target : MonoBehaviourPun, IPunObservable
{
    public bool IsCutted { get; protected set; } = false;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected Rigidbody2D rigidbody2D;

    private TargetData targetData;

    public TargetData Data => targetData;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            rigidbody2D.isKinematic = true;
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

        if (p.y < -12f || p.y > 12f || p.x < -10f || p.x > 12f)
        {
            gameObject.SetActive(false);
            OnMiss();
        }
    }

    public void Hit(HitInfo info)
    {
        if (IsCutted) return;

        ExecuteHit(info);
        float score = CalculateScore(info);
        EventManager.OnTargetHit.Invoke(this, info, score);
        photonView.RPC(nameof(RPCHit), RpcTarget.Others, info.Player, info.EntryPoint, info.ExitPoint);
    }

    [PunRPC]
    public void RPCHit(byte player, Vector2 entryPoint, Vector2 exitPoint)
    {
        var info = new HitInfo(entryPoint, player);
        info.ExitPoint = exitPoint;
        ExecuteHit(info);
    }

    protected virtual void ExecuteHit(HitInfo info)
    {
        IsCutted = true;
        SpriteCutter.Instance.CutSprite(spriteRenderer.sprite, transform, info.EntryPoint, info.ExitPoint);
        gameObject.SetActive(!IsCutted);
        OnHit(info);
    }

    protected float CalculateScore(HitInfo info)
    {
        Vector2 center = (Vector2)transform.position;
        Vector2 entry = info.EntryPoint;
        float distance = Vector2.Distance(center, entry);
        float maxDistance = spriteRenderer.bounds.extents.magnitude;
        float normalized = Mathf.Clamp01(distance / maxDistance);
        float score = Mathf.Lerp(targetData.MaxScore, targetData.MinScore, normalized);
        
        return score;
    }

    public void Setup(TargetData data)
    {
        ExecuteSetup(data);
        if (photonView.IsMine)
            rigidbody2D.velocity = data.LaunchDirection * data.Speed;

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
        gameObject.SetActive(true);

        spriteRenderer.sprite = AssetService.GetTargetSprite(data.SpriteKey);
        transform.position = data.StartPosition;
        transform.localScale = Vector3.one * data.Size;
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
    
    //will be used for sound effects
    protected abstract void OnHit(HitInfo hitInfo);
}
