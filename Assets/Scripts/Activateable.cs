using UnityEngine;
using UnityEngine.Events;

public class Activateable : MonoBehaviour
{
    protected bool active;

    public bool IsActive => active;
    [HideInInspector]
    public UnityEvent<Activateable> onDeactivate;
    [HideInInspector]
    public UnityEvent<Activateable> onActivate;

    public virtual void Activate(bool forced = false)
    {
        if (active && !forced) return;

        active = true;
        gameObject.SetActive(true);
        OnActivate();
    }

    public virtual void Deactivate()
    {
        if (!active) return;

        active = false;
        gameObject.SetActive(false);
        OnDeactivate();
    }

    public virtual void RawDeactivate()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public virtual void OnActivate()
    {
        onActivate.Invoke(this);
    }

    public virtual void OnDeactivate()
    {
        onDeactivate.Invoke(this);
    }
}
