using UnityEngine;
using Photon.Pun;

public class BossTarget : Target
{
    [SerializeField] private int maxHp = 50;
    [SerializeField] private float slowDownFactor = 0.1f;
    private bool isSlowed = false;

    private int hp;
    public override void Hit(HitInfo info)
    {
        if (IsCutted) return;

        hp--;
        if (hp == 0)
        {
            float score = Data.MinScore;
            EventManager.OnTargetHit.Invoke(this, info);
            IsCutted = true;
        }
            
        ExecuteHit(info);
        photonView.RPC(nameof(RPCHit), RpcTarget.Others, info.Player, info.EntryPoint, info.ExitPoint);
    }

    protected override void ExecuteHit(HitInfo info)
    {
        SlowDown();
        OnHit(info);

        if (hp == 0)
        {
            SpriteSlicer.Instance.Slice(spriteRenderer, info.EntryPoint, info.ExitPoint);
            gameObject.SetActive(false);
        }
    }

    private void SlowDown()
    {
        if (isSlowed) return;

        rb.velocity *= slowDownFactor;
        rb.gravityScale *= slowDownFactor;
        isSlowed = true;
    }

    protected override void OnSetup(TargetData data)
    {
        hp = maxHp;
        IsCutted = false;
    }

    protected override void OnHit(HitInfo hitInfo)
    {
    }
}
