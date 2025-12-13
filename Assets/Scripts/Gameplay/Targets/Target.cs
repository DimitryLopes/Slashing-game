using UnityEngine;
using Photon.Pun;

public abstract class Target : MonoBehaviourPun, IPunObservable
{
    public bool IsCutted { get; protected set; } = false;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected Rigidbody2D rigidbody2D;
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            rigidbody2D.isKinematic = true;
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

    private void ExecuteHit(HitInfo info)
    {
        IsCutted = true;
        SpriteCutter.Instance.CutSprite(spriteRenderer.sprite, transform, info.EntryPoint, info.ExitPoint);
        gameObject.SetActive(!IsCutted);
        OnHit(info);
    }

    public void Setup(TargetData data)
    {
        ExecuteSetup(data); 
        
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
