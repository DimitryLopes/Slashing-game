using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField]
    private FloatingText floatingTextPrefab;
    [SerializeField]
    private Transform canvasTransform;
    
    private List<FloatingText> floatingTextPool = new List<FloatingText>();

    public static FloatingTextManager Instance { get; private set; }

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

    public void ShowFloatingText(string message, Vector3 worldPosition)
    {
        FloatingText floatingText = GetAvailableFloatingText();
        floatingText.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        floatingText.ShowText(message);
    }

    private FloatingText GetAvailableFloatingText()
    {
        foreach(FloatingText ft in floatingTextPool)
        {
            if(!ft.IsActive)
            {
                return ft;
            }
        }

        FloatingText newFloatingText = Instantiate(floatingTextPrefab, canvasTransform);
        floatingTextPool.Add(newFloatingText);
        newFloatingText.Activate();
        return newFloatingText;
    }
}
