using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Base class for Unity authoring components that export to GameCore components.
///
/// Usage:
/// 1. Create a new class inheriting from GameComponentAuth&lt;T&gt; where T is your GameComponent type
/// 2. Add [Serializable] fields that match the GameComponent's properties
/// 3. Override CreateComponent() to create and populate the GameComponent
///
/// The LevelFileExporter will automatically detect these and export them properly.
/// </summary>
public abstract class GameComponentAuth : MonoBehaviour
{
    /// <summary>
    /// Creates the GameCore component from this Unity authoring component.
    /// Override this to populate the component with your serialized data.
    /// </summary>
    public abstract GCComponent ToGameComponent();
}

/// <summary>
/// Generic base class for type-safe component authoring.
/// </summary>
/// <typeparam name="T">The GameComponent type this auth component exports to</typeparam>
public abstract class GameComponentAuth<T> : GameComponentAuth
    where T : GCComponent, new()
{
    public override GCComponent ToGameComponent()
    {
        var component = CreateComponent();
        component.Enabled = enabled;
        return component;
    }

    /// <summary>
    /// Override to create and populate your specific component type.
    /// Default implementation creates an empty component.
    /// </summary>
    protected virtual T CreateComponent() => new T();
}

public interface IComponentReferenceAuthoring
{
    void ResolveReferences(GCComponent component, ComponentExportContext context);
}

public class ComponentExportContext
{
    private ulong nextComponentId = 1;

    private readonly Dictionary<Component, GCComponent> unityToGameCore = new();

    private readonly Dictionary<ulong, GCComponent> idToComponent = new();

    private readonly List<(IComponentReferenceAuthoring resolver, GCComponent component)> deferredResolutions =
        new();

    public T RegisterComponent<T>(Component unityComponent, T gameComponent)
        where T : GCComponent
    {
        if (gameComponent == null)
            return null;

        gameComponent.ComponentId = nextComponentId++;
        idToComponent[gameComponent.ComponentId] = gameComponent;

        if (unityComponent != null)
        {
            unityToGameCore[unityComponent] = gameComponent;
        }

        return gameComponent;
    }

    public bool TryGetComponentId(Component unityComponent, out ulong componentId)
    {
        if (unityComponent != null && unityToGameCore.TryGetValue(unityComponent, out var component))
        {
            componentId = component.ComponentId;
            return true;
        }

        componentId = 0;
        return false;
    }

    public bool TryGetComponent(ulong componentId, out GCComponent component) =>
        idToComponent.TryGetValue(componentId, out component);

    public void RegisterDeferredResolver(IComponentReferenceAuthoring resolver, GCComponent component)
    {
        if (resolver == null || component == null)
            return;

        deferredResolutions.Add((resolver, component));
    }

    public void ResolveDeferredReferences()
    {
        foreach (var (resolver, component) in deferredResolutions)
        {
            resolver.ResolveReferences(component, this);
        }
    }
}
