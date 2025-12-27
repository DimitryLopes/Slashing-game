using Photon.Pun;
using System;
using UnityEngine;

public class PlayerSpecificTarget : Target
{
    private new Action<HitInfo, byte, float> OnTargetHit;
    private byte player;
    
    protected override void OnHit(HitInfo hitInfo)
    {
    }

    [PunRPC]
    protected override void ExecuteHit(HitInfo info)
    {
        IsCutted = true;
        SpriteCutter.Instance.CutSprite(spriteRenderer.sprite, transform, info.EntryPoint, info.ExitPoint);
        gameObject.SetActive(!IsCutted);
        float score = CalculateScore(info);
        OnTargetHit?.Invoke(info, player, score);
        OnHit(info);
    }

    public void Setup(TargetData data, byte targetPlayer, Action<HitInfo, byte, float> onHit, Action onMiss)
    {
        photonView.RPC(nameof(RPCSpecialSetup), RpcTarget.All, targetPlayer, onHit);
        Setup(data, null, onMiss);
    }

    [PunRPC]
    private void RPCSpecialSetup(byte targetPlayer, Action<HitInfo, byte, float> onHit)
    {
        bool isTargetForLocalPlayer = targetPlayer == Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber;
        spriteRenderer.color = isTargetForLocalPlayer ? Color.green : Color.red;
        player = targetPlayer;
        OnTargetHit = onHit;
    }
}
