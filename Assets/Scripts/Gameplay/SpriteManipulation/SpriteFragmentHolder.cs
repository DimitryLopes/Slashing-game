using Photon.Pun;
using UnityEngine;

public class SpriteFragmentHolder : Activateable
{
    [SerializeField]
    private SpriteRenderer renderer;
    [SerializeField]
    private Rigidbody2D rb;

    private const float DisableDelay = 1f;

    private float disableTimer = 0f;

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
    }

    public void ApplyForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ApplyTorque(float torque)
    {
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }

    private void Update()
    {
        disableTimer += Time.deltaTime;
        if (disableTimer >= DisableDelay)
        {
            Deactivate();
            disableTimer = 0f;
        }
    }

    public override void OnActivate()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.WakeUp();
        disableTimer = 0f;
        transform.localScale = Vector3.one;
        transform.LeanScale(Vector3.zero, DisableDelay).setEase(LeanTweenType.easeInQuint);
    }
}