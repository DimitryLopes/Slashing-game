using UnityEngine;

public struct TargetData
{
    private float size;
    private float health;
    private float speed;
    private Vector3 start_position;
    private string spriteKey;

    public float Size => size;
    public float Health => health;
    public float Speed => speed;
    public Vector3 StartPosition => start_position;
    public string SpriteKey => spriteKey;

    public TargetData(float size, float health, float speed, Vector3 start_position, string spriteKey)
    {
        this.size = size;
        this.health = health;
        this.speed = speed;
        this.start_position = start_position;
        this.spriteKey = spriteKey;
    }
}
