using GameCoreLib;
using UnityEngine;

/// <summary>
/// Component that binds a Unity GameObject to a GameCoreObj.
/// Implements IGCBinding to integrate with the prefab binding lifecycle.
///
/// Every GameObject in the visual hierarchy should have this component
/// so the GameCoreObj can be inspected at runtime via the custom inspector.
/// </summary>
public sealed class GCObjBinding : MonoBehaviour, IGCBinding
{
    /// <summary>
    /// The GameCoreObj this GameObject is bound to.
    /// </summary>
    public GameCoreObj GameCoreObj { get; private set; }

    /// <summary>
    /// Binds this component to a GameCoreObj.
    /// Called when the prefab is acquired from the pool or when a visual is created.
    /// </summary>
    public void Bind(GameCoreObj gcObj)
    {
        GameCoreObj = gcObj;
    }

    /// <summary>
    /// Unbinds from the current GameCoreObj.
    /// Called when the prefab is released back to the pool.
    /// </summary>
    public void Unbind()
    {
        GameCoreObj = null;
    }
}
