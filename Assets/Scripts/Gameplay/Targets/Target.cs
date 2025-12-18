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

    private Action onMiss;

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

            if (PhotonNetwork.IsMasterClient)
            {
                onMiss?.Invoke();
            }
        }
    }

    public void Hit(HitInfo info)
    {
        ExecuteHit(info);
        photonView.RPC(nameof(RPCHit), RpcTarget.Others, info.Player, info.EntryPoint, info.ExitPoint);
    }

    [PunRPC]
    public void RPCHit(byte player, Vector2 entryPoint, Vector2 exitPoint)
    {
        var info = new HitInfo(entryPoint, player);
        info.ExitPoint = exitPoint;
        ExecuteHit(info);
    }

    [PunRPC]
    public void RPCDeactivate()
    {
        gameObject.SetActive(false);
    }

    private void ExecuteHit(HitInfo info)
    {
        IsCutted = true;
        SpriteCutter.Instance.CutSprite(spriteRenderer.sprite, transform, info.EntryPoint, info.ExitPoint);
        gameObject.SetActive(!IsCutted);

        OnHit(info);
    }

    public void Setup(TargetData data, Action onMiss)
    {
        ExecuteSetup(data);
        this.onMiss = onMiss;
        
        if (photonView.IsMine)
            rigidbody2D.velocity = data.LaunchDirection * data.Speed;

        photonView.RPC(nameof(RPCSetup), RpcTarget.Others, data.Size, data.Health,
            data.Speed, data.SpriteKey, data.LaunchDirection, data.StartPosition);
    }

    [PunRPC]
    protected void RPCSetup(float size, float hp, float speed, string spriteKey, Vector2 launchDirection, Vector3 startPosition)
    {
        transform.position = startPosition;
        TargetData data = new TargetData(size, hp, speed, startPosition, launchDirection, spriteKey);
        ExecuteSetup(data);
    }

    private void ExecuteSetup(TargetData data)
    {
        IsCutted = false;
        gameObject.SetActive(true);
        OnSetup(data);
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

    public abstract void OnHit(HitInfo hitInfo);

    public abstract void OnSetup(TargetData data);
}
