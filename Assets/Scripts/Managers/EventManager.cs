using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public static UnityEvent<IScreen> OnScreenAfterHideEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenAfterShowEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenBeforeHideEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenBeforeShowEvent = new UnityEvent<IScreen>();
    public static UnityEvent<int> OnPlayerDamaged = new UnityEvent<int>();
}
