using GameCoreLib;
using UnityEngine;

/// <summary>
/// Interface for Unity components that bind to GameCore objects.
/// Implement this on any MonoBehaviour that needs lifecycle callbacks when
/// the prefab is acquired from or released to the object pool.
/// </summary>
public interface IGCBinding
{
    /// <summary>
    /// Binds this component to a GameCoreObj.
    /// Called when the prefab is acquired from the pool.
    /// </summary>
    void Bind(GameCoreObj gcObj);

    /// <summary>
    /// Unbinds from the current GameCoreObj.
    /// Called when the prefab is released back to the pool.
    /// </summary>
    void Unbind();
}

/// <summary>
/// Abstract base class for Unity components that bind to GameCore components.
/// Derive from this to create visual bindings that automatically sync with
/// GameCore component state.
/// </summary>
/// <typeparam name="T">The GCComponent type this binding displays.</typeparam>
public abstract class GCBinding<T> : MonoBehaviour, IGCBinding
    where T : GCComponent
{
    /// <summary>
    /// The bound GameCore component.
    /// </summary>
    public T BoundComponent { get; private set; }

    /// <summary>
    /// The GameCoreObj that owns the bound component.
    /// </summary>
    public GameCoreObj BoundObject => BoundComponent?.GCObj;

    /// <summary>
    /// Whether this binding has a valid component reference.
    /// </summary>
    public bool IsBound => BoundComponent != null;

    /// <summary>
    /// Binds to a GameCoreObj by finding the first component of type T.
    /// Called by RenderPrefabRoot when acquired from the pool.
    /// </summary>
    public void Bind(GameCoreObj gcObj)
    {
        if (gcObj == null)
        {
            Unbind();
            return;
        }

        T component = gcObj.GetComponent<T>();
        if (component != null)
        {
            BindComponent(component);
        }
    }

    /// <summary>
    /// Binds directly to a specific component.
    /// </summary>
    public void BindComponent(T component)
    {
        T previousComponent = BoundComponent;
        BoundComponent = component;

        if (previousComponent != component)
        {
            OnBindingChanged(previousComponent, component);
        }
    }

    /// <summary>
    /// Unbinds the current component reference.
    /// Called by RenderPrefabRoot when released to the pool.
    /// </summary>
    public void Unbind()
    {
        if (BoundComponent != null)
        {
            T previous = BoundComponent;
            BoundComponent = null;
            OnBindingChanged(previous, null);
        }
    }

    /// <summary>
    /// Called when the binding changes. Override to respond to binding updates.
    /// Use this for registration/unregistration with external systems.
    /// </summary>
    protected virtual void OnBindingChanged(T previousComponent, T newComponent) { }

    /// <summary>
    /// Called every frame when bound. Override to update visuals based on component state.
    /// </summary>
    protected virtual void UpdateVisuals() { }

    protected virtual void Update()
    {
        if (IsBound)
        {
            UpdateVisuals();
        }
    }
}
