using Photon.Pun;
using System;
using UnityEngine;

public class PlayerSpecificTarget : Target
{
    private byte player;

    public byte Player => player;

    protected override void OnHit(HitInfo hitInfo)
    {
    }

    public void Setup(TargetData data, byte targetPlayer)
    {
        photonView.RPC(nameof(RPCSpecialSetup), RpcTarget.All, targetPlayer);
        Setup(data);
    }

    [PunRPC]
    private void RPCSpecialSetup(byte targetPlayer)
    {
        bool isTargetForLocalPlayer = targetPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
        spriteRenderer.color = isTargetForLocalPlayer ? Color.green : Color.red;
        player = targetPlayer;
    }
}
