using System.Collections.Generic;
using UnityEngine;

public class SpriteSlicer : MonoBehaviour
{
    private const string CUT_POINT_MATERIAL_PROPERTY = "_CutPoint";
    private const string CUT_NORMAL_MATERIAL_PROPERTY = "_CutNormal";
    private const string SIDE_MATERIAL_PROPERTY = "_Side";

    [SerializeField] 
    private Material sliceMaterial;
    [SerializeField]
    private SpriteFragmentHolder holderPrefab;

    public static SpriteSlicer Instance { get; private set; }

    [HideInInspector]
    public List<SpriteFragmentHolder> holders = new();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Slice(SpriteRenderer renderer, Vector2 entryPoint, Vector2 exitPoint)
    {
        Vector2 cutDir = (exitPoint - entryPoint).normalized;
        Vector2 cutNormal = new Vector2(-cutDir.y, cutDir.x);

        SpawnFragment(renderer, entryPoint, cutNormal, 1);
        SpawnFragment(renderer, entryPoint, cutNormal, -1);

        renderer.gameObject.SetActive(false);
    }

    private void SpawnFragment(SpriteRenderer sourceRenderer, Vector2 cutPoint, Vector2 cutNormal, float side)
    {
        var holder = GetAvailableHolder();
        holder.Activate();
        holder.transform.position = sourceRenderer.transform.position;
        holder.transform.rotation = sourceRenderer.transform.rotation;
        holder.transform.localScale = sourceRenderer.transform.localScale;

        SpriteRenderer holderRenderer = holder.Renderer;
        holderRenderer.sprite = sourceRenderer.sprite;
        holderRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        holderRenderer.sortingOrder = sourceRenderer.sortingOrder;

        Vector2 localCutPoint = holder.transform.InverseTransformPoint(cutPoint);
        Vector2 localCutNormal = holder.transform.InverseTransformDirection(cutNormal).normalized;

        var materialInstance = new Material(sliceMaterial);
        materialInstance.SetVector(CUT_POINT_MATERIAL_PROPERTY, localCutPoint);
        materialInstance.SetVector(CUT_NORMAL_MATERIAL_PROPERTY, localCutNormal);
        materialInstance.SetFloat(SIDE_MATERIAL_PROPERTY, side);
        holderRenderer.material = materialInstance;

        holder.ApplyForce(cutNormal * side);
        holder.ApplyTorque(Random.Range(-5f, 5f));
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
}