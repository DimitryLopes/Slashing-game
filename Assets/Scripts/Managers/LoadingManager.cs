using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour, IManager
{
    [SerializeField]
    private List<GameObject> managers;

    public static LoadingManager Instance { get; private set; }

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForManagers());
    }

    private IEnumerator WaitForManagers()
    {
        List<IManager> managers = new List<IManager>();
        foreach (GameObject managerObject in this.managers)
        {
            IManager manager = managerObject.GetComponent<IManager>();
            if (manager != null)
            {
                managers.Add(manager);
            }
            else
            {
                Debug.LogError(managerObject.name + "did not have an IManager component attached to it.");
            }
        }

        foreach (IManager manager in managers)
        {
            while (!manager.IsInitialized)
            {
                yield return null;
            }
        }

        IsInitialized = true;
        StateManager.Instance.ChangeState(GameState.Menu);
        yield return null;
    }
}
