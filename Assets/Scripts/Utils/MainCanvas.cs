using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField]
    public RectTransform CanvasRect;
    [SerializeField]
    public Canvas Canvas;

    public static MainCanvas Instance { get; private set; }

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
}