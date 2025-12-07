using GameCoreLib;
using UnityEngine;

/// <summary>
/// Component that identifies which render prefab this is in the RenderPrefabRegistry.
/// Add this to prefabs that should be renderable, and set the ID to match their index in the registry.
/// Derives from GameComponentAuth so it gets stripped from rendered prefabs at runtime.
/// </summary>
public class RenderPrefabIdentifier : GameComponentAuth
{
    [Tooltip(
        "The ID of this prefab in the RenderPrefabRegistry (0-based index). Set to -1 for no rendering."
    )]
    [SerializeField]
    private int renderPrefabID = -1;

    /// <summary>
    /// The render prefab ID for this GameObject.
    /// </summary>
    public int RenderPrefabID => renderPrefabID;

    /// <summary>
    /// Set the render prefab ID (useful for editor tools).
    /// </summary>
    public void SetRenderPrefabID(int id)
    {
        renderPrefabID = id;
    }

    /// <summary>
    /// Returns null since this component is purely for authoring and doesn't export a runtime component.
    /// The prefab ID is read directly by the converter via RenderPrefabRegistry.GetPrefabID().
    /// </summary>
    public override GCComponent ToGameComponent() => null;
}
