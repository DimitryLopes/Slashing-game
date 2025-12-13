using UnityEngine;

public struct TargetData
{
    private float size;
    private float health;
    private float speed;
    private Vector3 startPosition;
    private Vector2 launchDirection;
    private string spriteKey;

    public float Size => size;
    public float Health => health;
    public float Speed => speed;
    public Vector3 StartPosition => startPosition;
    public string SpriteKey => spriteKey;
    public Vector2 LaunchDirection => launchDirection;

    public TargetData(float size, float health, float speed, Vector3 start_position, Vector2 launchDirection, string spriteKey)
    {
        this.size = size;
        this.health = health;
        this.speed = speed;
        this.startPosition = start_position;
        this.launchDirection = launchDirection;
        this.spriteKey = spriteKey;
    }
}
