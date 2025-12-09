using UnityEngine;
using UnityEngine.Tilemaps;

public class AssetService
{
    public static Sprite GetTargetSprite(string spriteKey)
    {
        string path = string.Format(Constants.Assets.TARGET_SPRITE_PATH, spriteKey);
        return LoadSprite(path);
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogError($"Failed to load sprite at path: {path}");
        }
        return sprite;
    }
}
