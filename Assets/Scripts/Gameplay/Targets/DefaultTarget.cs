using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultTarget : Target
{
    public override void OnHit(HitInfo hitInfo)
    {
        
    }

    public override void OnSetup(TargetData info)
    {
        spriteRenderer.sprite = AssetService.GetTargetSprite(info.SpriteKey);
        transform.position = info.StartPosition;
        transform.localScale = Vector3.one * info.Size;
    }
}
