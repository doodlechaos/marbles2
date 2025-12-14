using GameCoreLib;
using UnityEngine;

/// <summary>
/// Non-generic interface for GCBinding to allow TileBinding to work with any binding type.
/// </summary>
public interface IGCBinding
{
    /// <summary>
    /// Attempts to bind this Unity component to a GameCoreObj.
    /// The implementation should find the appropriate GCComponent on the object.
    /// </summary>
    /// <param name="gcObj">The GameCoreObj to search for the component.</param>
    /// <returns>True if binding was successful, false otherwise.</returns>
    bool TryBindToObject(GameCoreObj gcObj);
}

/// <summary>
/// Abstract base class for Unity components that bind to GameCore components.
/// Derive from this to create visual bindings that automatically sync with
/// GameCore component state.
///
/// Simply add this component to a prefab, and TileBinding will automatically
/// bind it when the prefab is instantiated.
/// </summary>
/// <typeparam name="T">The GCComponent type this binding displays.</typeparam>
public abstract class GCBinding<T> : MonoBehaviour, IGCBinding
    where T : GCComponent
{
    /// <summary>
    /// The bound GameCore component. Set automatically by TileBinding when
    /// a prefab containing this binding is instantiated.
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
    /// Attempts to bind to a GameCoreObj by finding the first component of type T.
    /// Called automatically by TileBinding.
    /// </summary>
    public virtual bool TryBindToObject(GameCoreObj gcObj)
    {
        if (gcObj == null)
            return false;

        T component = gcObj.GetComponent<T>();
        if (component != null)
        {
            Bind(component);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Binds this Unity component to a GameCore component.
    /// </summary>
    /// <param name="component">The GameCore component to bind to.</param>
    public void Bind(T component)
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
    /// </summary>
    /// <param name="previousComponent">The previously bound component (may be null).</param>
    /// <param name="newComponent">The newly bound component (may be null).</param>
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
