using UnityEngine;

public struct HitInfo
{
    private Vector2 entryPoint;
    private byte player;

    public Vector2 EntryPoint => entryPoint;
    public Vector2 ExitPoint { get; set; }
    public float Score { get; set; }
    public byte Player => player;

    public HitInfo(Vector2 entry, byte player)
    {
        entryPoint = entry;
        ExitPoint = Vector2.zero;
        Score = 0f;
        this.player = player;
    }
}
