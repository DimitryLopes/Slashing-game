using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliceAreaView : MonoBehaviour
{
    [SerializeField]
    private Image areaSidePrefab;
    
    private List<Image> areaSides = new List<Image>();

    private int areaOwnerID;

    public void Setup(SliceArea area)
    {
        areaOwnerID = area.OwnerId;
        EventManager.OnSliceAreaMoved.AddListener(UpdateView);
    }

    public void Clear()
    {
        areaOwnerID = -1;
        EventManager.OnSliceAreaMoved.RemoveListener(UpdateView);
        foreach(var side in areaSides)
        {
            Destroy(side.gameObject);
        }
        areaSides.Clear();
    }

    private void UpdateView(SliceArea sliceArea)
    {
        if(sliceArea.OwnerId != areaOwnerID) return;

        for (int i = 0; i < sliceArea.Vertices.Length; i++)
        {
            var vertice = sliceArea.Vertices[i];
            var verticePosition = Camera.main.WorldToScreenPoint(vertice);
            
            if (i >= areaSides.Count)
            {
                var newSide = Instantiate(areaSidePrefab, transform);
                areaSides.Add(newSide);
            }

            areaSides[i].rectTransform.anchorMax = verticePosition;
            if(i + 1 < sliceArea.Vertices.Length)
                areaSides[i + 1].rectTransform.anchorMin = verticePosition;
            else
                areaSides[0].rectTransform.anchorMin = verticePosition;
        }
    }
}
