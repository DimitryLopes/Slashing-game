using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour, IManager
{
    [SerializeField]
    private FloatingText floatingTextPrefab;
    [SerializeField]
    private Transform floatingTextContainer;
    
    private List<FloatingText> floatingTextPool = new List<FloatingText>();

    public static FloatingTextManager Instance { get; private set; }
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsInitialized = true;
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

        FloatingText newFloatingText = Instantiate(floatingTextPrefab, floatingTextContainer);
        floatingTextPool.Add(newFloatingText);
        newFloatingText.Activate();
        return newFloatingText;
    }
}
