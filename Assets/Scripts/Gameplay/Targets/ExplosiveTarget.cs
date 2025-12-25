using UnityEngine;

public class ExplosiveTarget : Target
{
    protected override void OnMiss()
    {
        //Explosive targets don't do anything on a miss
    }

    protected override void OnHit(HitInfo hitInfo)
    {
        OnTargetMiss?.Invoke(); 
    }
}
