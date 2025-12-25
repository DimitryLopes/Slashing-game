using System;

public class PlayerSpecificTarget : Target
{
    private new Action<HitInfo, byte, float> OnTargetHit;
    private byte player;
    protected override void OnHit(HitInfo hitInfo)
    {
        throw new System.NotImplementedException();
    }

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
        player = targetPlayer;
        OnTargetHit = onHit;
        Setup(data, null, onMiss);
    }

}
