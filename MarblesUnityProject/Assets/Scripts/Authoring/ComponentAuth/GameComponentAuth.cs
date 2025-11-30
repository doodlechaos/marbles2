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
    public abstract GameComponent ToGameComponent();
}

/// <summary>
/// Generic base class for type-safe component authoring.
/// </summary>
/// <typeparam name="T">The GameComponent type this auth component exports to</typeparam>
public abstract class GameComponentAuth<T> : GameComponentAuth
    where T : GameComponent, new()
{
    public override GameComponent ToGameComponent()
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
