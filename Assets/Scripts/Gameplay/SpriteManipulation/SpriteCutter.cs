using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SpriteCutter : MonoBehaviour
{
    [SerializeField]
    private SpriteFragmentHolder holderPrefab;
    [SerializeField] 
    private float normalForce = 6f;
    [SerializeField]
    private float alongCutForce = 2f;
    [SerializeField] 
    private float rotationForce = 5f;

    public static SpriteCutter Instance { get; private set; }

    public List<SpriteFragmentHolder> holders = new();

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

    public void CutSprite(SpriteRenderer renderer, Transform worldPosition, int sliceCount)
    {
        Sprite sprite = renderer.sprite;
        Vector2 center = renderer.bounds.center;

        List<Vector2> directions = new List<Vector2>();
        for (int i = 0; i < sliceCount; i++)
        {
            float angle = (360f / sliceCount) * i;
            float rad = angle * Mathf.Deg2Rad;
            directions.Add(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)));
        }

        for (int i = 0; i < sliceCount; i++)
        {
            Vector2 dirA = directions[i];
            Vector2 dirB = directions[(i + 1) % sliceCount];

            float maxExtent = renderer.bounds.extents.magnitude * 2f;
            Vector2 pA = center + dirA * maxExtent;
            Vector2 pB = center + dirB * maxExtent;

            Vector2 cutDir = (pB - pA).normalized;
            Vector2 cutNormal = new Vector2(-cutDir.y, cutDir.x);

            Sprite sectorFragment = SliceSpriteSector(sprite, worldPosition, center, pA, pB);
            SpawnFragment(sectorFragment, worldPosition.position, cutNormal, cutDir);
        }
    }

    public void CutSprite(Sprite sprite, Transform worldPosition, Vector2 startPoint, Vector2 endPoint)
    {
        Sprite[] slicedSprites = SliceSprite(sprite, worldPosition, startPoint, endPoint);

        Vector2 cutDir = (endPoint - startPoint).normalized;
        Vector2 cutNormal = new Vector2(-cutDir.y, cutDir.x);

        SpawnFragment(slicedSprites[0], worldPosition.position, cutNormal, cutDir);
        SpawnFragment(slicedSprites[1], worldPosition.position, -cutNormal, cutDir);
    }

    private Sprite SliceSpriteSector(Sprite sprite, Transform worldPosition, Vector2 center, Vector2 pA, Vector2 pB)
    {
        Texture2D originalTexture = sprite.texture;
        Rect rect = sprite.rect;
        Texture2D fragmentTexture = new Texture2D((int)rect.width, (int)rect.height);

        Color[] originalPixels = originalTexture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        );

        Vector2 texCenter = new Vector2(sprite.pivot.x, sprite.pivot.y);

        for (int y = 0; y < rect.height; y++)
        {
            for (int x = 0; x < rect.width; x++)
            {
                Vector2 p = new Vector2(x, y);
                Vector2 v = p - texCenter;

                if (IsPointInSector(v, Vector2.zero, pA - center, pB - center))
                {
                    int index = x + y * (int)rect.width;
                    fragmentTexture.SetPixel(x, y, originalPixels[index]);
                }
                else
                {
                    fragmentTexture.SetPixel(x, y, Color.clear);
                }
            }
        }

        fragmentTexture.Apply();
        return Sprite.Create(fragmentTexture, new Rect(0, 0, fragmentTexture.width, fragmentTexture.height), sprite.pivot / rect.size, sprite.pixelsPerUnit);
    }

    private bool IsPointInSector(Vector2 point, Vector2 center, Vector2 a, Vector2 b)
    {
        float angleA = Mathf.Atan2(a.y, a.x);
        float angleB = Mathf.Atan2(b.y, b.x);
        float angleP = Mathf.Atan2(point.y, point.x);

        float delta = Mathf.DeltaAngle(angleA * Mathf.Rad2Deg, angleB * Mathf.Rad2Deg);
        if (delta < 0) delta += 360f;

        float deltaP = Mathf.DeltaAngle(angleA * Mathf.Rad2Deg, angleP * Mathf.Rad2Deg);
        if (deltaP < 0) deltaP += 360f;

        return deltaP <= delta;
    }

    private SpriteFragmentHolder GetAvailableHolder()
    {
        foreach (var holder in holders)
        {
            if (!holder.IsActive)
            {
                return holder;
            }
        }

        SpriteFragmentHolder newHolder = Instantiate(holderPrefab, transform.position, Quaternion.identity);
        holders.Add(newHolder);
        return newHolder;
    }

    private void SpawnFragment(Sprite sprite, Vector3 position, Vector2 normal, Vector2 cutDir)
    {
        SpriteFragmentHolder holder = GetAvailableHolder();

        holder.transform.position = position;
        holder.SetSprite(sprite);

        Vector2 force = normal * normalForce + cutDir * alongCutForce;

        holder.Activate(true);
        holder.ApplyForce(force);
        holder.ApplyTorque(Random.Range(-rotationForce, rotationForce));
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
