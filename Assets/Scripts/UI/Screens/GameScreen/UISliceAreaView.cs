using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliceAreaView : MonoBehaviour
{
    [SerializeField]
    private Image areaSidePrefab;

    private MainCanvas canvas;

    private readonly List<Image> areaSides = new List<Image>();
    private int areaOwnerID = -1;

    private void Awake()
    {
        canvas = MainCanvas.Instance;
    }

    public void Setup(SliceArea area)
    {
        areaOwnerID = area.OwnerId;
        EventManager.OnSliceAreaMoved.AddListener(UpdateView);
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        areaOwnerID = -1;
        EventManager.OnSliceAreaMoved.RemoveListener(UpdateView);

        foreach (var side in areaSides)
        {
            Destroy(side.gameObject);
        }

        areaSides.Clear();
        gameObject.SetActive(false);
    }

    private void UpdateView(SliceArea sliceArea)
    {
        if (sliceArea.OwnerId != areaOwnerID)
            return;

        int vertexCount = sliceArea.Vertices.Length;

        EnsureSideCount(vertexCount);

        Vector2[] uiPoints = ConvertWorldToCanvasPoints(sliceArea.Vertices);

        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 start = uiPoints[i];
            Vector2 end = uiPoints[(i + 1) % vertexCount];

            UpdateSideTransform(areaSides[i].rectTransform, start, end);
        }
    }

    private void EnsureSideCount(int requiredCount)
    {
        while (areaSides.Count < requiredCount)
        {
            var side = Instantiate(areaSidePrefab, transform);
            side.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            areaSides.Add(side);
        }

        while (areaSides.Count > requiredCount)
        {
            Destroy(areaSides[areaSides.Count - 1].gameObject);
            areaSides.RemoveAt(areaSides.Count - 1);
        }
    }

    private Vector2[] ConvertWorldToCanvasPoints(Vector3[] worldVertices)
    {
        Vector2[] points = new Vector2[worldVertices.Length];

        for (int i = 0; i < worldVertices.Length; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldVertices[i]);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.CanvasRect,
                screenPoint,
                canvas.Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                out Vector2 localPoint
            );

            points[i] = localPoint;
        }

        return points;
    }

    private void UpdateSideTransform(RectTransform rect, Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        rect.anchoredPosition = (start + end) * 0.5f;
        rect.sizeDelta = new Vector2(distance, rect.sizeDelta.y);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rect.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
