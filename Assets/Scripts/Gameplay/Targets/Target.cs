using UnityEngine;
using Photon.Pun;

public abstract class Target : MonoBehaviourPun, IPunObservable
{
    public bool IsCutted { get; protected set; } = false;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected Rigidbody2D rigidbody2D;

    public void Hit(HitInfo info)
    {
        if (!photonView.IsMine) return;

        SpriteCutter.Instance.CutSprite(spriteRenderer.sprite, transform, info.EntryPoint, info.ExitPoint);
        IsCutted = true;
        gameObject.SetActive(!IsCutted);
        OnHit(info);

        photonView.RPC(nameof(Sync), RpcTarget.Others, IsCutted);
    }

    public void Setup(TargetData data)
    {
        if (!photonView.IsMine) return; 

        IsCutted = false;
        gameObject.SetActive(!IsCutted);
        OnSetup(data);

        photonView.RPC(nameof(Sync), RpcTarget.Others, IsCutted);
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
    
    [PunRPC]
    public void Sync(bool isCutted)
    {
        IsCutted = isCutted;
        gameObject.SetActive(!IsCutted);
    }

}
