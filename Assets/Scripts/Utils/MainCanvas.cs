using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField]
    public Transform ScreenContainer;

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