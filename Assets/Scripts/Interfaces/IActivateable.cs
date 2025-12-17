public interface IActivateable
{
    bool IsActive { get; }

    void Activate() { }

    void Deactivate() { }

    void OnActivate() { }

    void OnDeactivate() { }
}


