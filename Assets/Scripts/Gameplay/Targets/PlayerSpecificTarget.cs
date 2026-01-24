using Photon.Pun;
using UnityEngine;

public class PlayerSpecificTarget : Target
{
    private int player;

    public int Player => player;
    protected override void ExecuteHit(HitInfo info)
    {
        SpriteSlicer.Instance.Slice(spriteRenderer, info.EntryPoint, info.ExitPoint);

        if (player == info.Player)
        {
            FloatingTextManager.Instance.ShowFloatingText($"+ {info.Score}", transform.position);
        }
        else
        {
            FloatingTextManager.Instance.ShowFloatingText("Ih rapaz, player errado ó", transform.position);
        }

        if(photonView.IsMine)
            EventManager.OnTargetHit.Invoke(this, info);

        gameObject.SetActive(!IsCutted);
    }

    protected override void OnHit(HitInfo hitInfo)
    {
    }

    public void Setup(TargetData data, int targetPlayer)
    {
        photonView.RPC(nameof(RPCSpecialSetup), RpcTarget.All, targetPlayer);
        Setup(data);
    }

    [PunRPC]
    private void RPCSpecialSetup(int targetPlayer)
    {
        bool isTargetForLocalPlayer = targetPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.LogError("TARGET IS FOR ME?: " + isTargetForLocalPlayer);
        spriteRenderer.color = isTargetForLocalPlayer ? Color.green : Color.red;
        player = targetPlayer;
    }
}
