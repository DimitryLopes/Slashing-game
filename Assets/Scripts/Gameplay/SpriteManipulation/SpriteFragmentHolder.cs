using UnityEngine;

public class SpriteFragmentHolder : Activateable
{
    [SerializeField]
    private SpriteRenderer renderer;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float minForce = 100f;
    [SerializeField]
    private float maxForce = 300f;

    private const float DisableDelay = 5f;

    private float disableTimer = 0f;

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
    }

    public void ApplyForce(Vector2 direction)
    {
        rb.AddForce(direction.normalized * Random.Range(minForce, maxForce));
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
}