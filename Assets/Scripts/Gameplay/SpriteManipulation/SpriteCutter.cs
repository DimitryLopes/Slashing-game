using System.Collections.Generic;
using UnityEngine;

public class SpriteCutter : MonoBehaviour
{
    [SerializeField]
    private SpriteFragmentHolder holderPrefab;

    public static SpriteCutter Instance { get; private set; }

    public List<SpriteFragmentHolder> holders;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CutSprite(Sprite sprite, Transform worldPosition, Vector2 startPoint, Vector2 endPoint)
    {
        Sprite[] slicedSprites = SliceSprite(sprite, worldPosition, startPoint, endPoint);

        for (int i = 0; i < slicedSprites.Length; i++)
        {
            SpriteFragmentHolder holder = Instantiate(holderPrefab, startPoint, Quaternion.identity);
            holder.SetSprite(slicedSprites[i]);
            holder.ApplyForce(endPoint - startPoint);
            holder.Activate(true);
            holders.Add(holder);
        }
    }

    private Sprite[] SliceSprite(Sprite sprite, Transform worldPosition, Vector2 worldStart, Vector2 worldEnd)
    {
        Texture2D originalTexture = sprite.texture;
        Rect rect = sprite.rect;

        Texture2D t1 = new Texture2D((int)rect.width, (int)rect.height);
        Texture2D t2 = new Texture2D((int)rect.width, (int)rect.height);

        Color[] originalPixels = originalTexture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        );

        Vector2 pA = WorldToTexture(sprite, worldPosition, worldStart);
        Vector2 pB = WorldToTexture(sprite, worldPosition, worldEnd);

        for (int y = 0; y < rect.height; y++)
        {
            for (int x = 0; x < rect.width; x++)
            {
                Vector2 p = new Vector2(x, y);

                int index = x + y * (int)rect.width;

                if (IsPointAboveLine(p, pA, pB))
                {
                    t1.SetPixel(x, y, originalPixels[index]);
                    t2.SetPixel(x, y, Color.clear);
                }
                else
                {
                    t2.SetPixel(x, y, originalPixels[index]);
                    t1.SetPixel(x, y, Color.clear);
                }
            }
        }

        t1.Apply();
        t2.Apply();

        Sprite s1 = Sprite.Create(t1, new Rect(0, 0, t1.width, t1.height), sprite.pivot / rect.size, sprite.pixelsPerUnit);
        Sprite s2 = Sprite.Create(t2, new Rect(0, 0, t2.width, t2.height), sprite.pivot / rect.size, sprite.pixelsPerUnit);

        return new Sprite[] { s1, s2 };
    }

    private bool IsPointAboveLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        return (lineEnd.x - lineStart.x) * (point.y - lineStart.y) - (lineEnd.y - lineStart.y) * (point.x - lineStart.x) > 0;
    }

    private Vector2 WorldToTexture(Sprite sprite, Transform worldPosition, Vector2 world)
    {
        Vector2 local = worldPosition.InverseTransformPoint(world);
        Vector2 pivot = sprite.pivot;
        float ppu = sprite.pixelsPerUnit;

        return new Vector2(
            pivot.x + local.x * ppu,
            pivot.y + local.y * ppu
        );
    }
}
