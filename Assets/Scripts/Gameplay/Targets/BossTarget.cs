using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class BossTarget : Target
{
    [SerializeField] private int maxHp = 50;
    [SerializeField] private float slowDownFactor = 0.1f;

    [SerializeField, Header("Hit Feedback")]
    private float endScale = 1.1f;
    [SerializeField] 
    private float animationDuration;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Image healthBar;

    private bool isSlowed = false;
    private int hp;

    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }

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
        photonView.RPC(nameof(RPCHit), RpcTarget.Others, info.Player, info.EntryPoint, info.ExitPoint, info.Score);
    }

    protected override void ExecuteHit(HitInfo info)
    {
        SlowDown();
        OnHit(info);

        if (hp == 0)
        {
            EventManager.OnTargetHit.Invoke(this, info);
            FloatingTextManager.Instance.ShowFloatingText($"+ {info.Score}", transform.position);
            SpriteSlicer.Instance.Slice(spriteRenderer, info.EntryPoint, info.ExitPoint);
            gameObject.SetActive(!IsCutted);            
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
        UpdateHealthBar();
        spriteRenderer.gameObject.SetActive(true);
        isSlowed = false;
        rb.gravityScale = Constants.Targets.DEFAULT_GRAVITY_SCALE;
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)hp / maxHp;
    }

    protected override void OnHit(HitInfo hitInfo)
    {
        DoInHitAnimation();
        UpdateHealthBar();
    }

    private void DoInHitAnimation()
    {
        spriteRenderer.transform.LeanScale(Vector3.one * endScale, animationDuration).setOnComplete(DoOutHitAnimation);
    }

    private void DoOutHitAnimation()
    {
        spriteRenderer.transform.LeanScale(Vector3.one, animationDuration);
    }
}
