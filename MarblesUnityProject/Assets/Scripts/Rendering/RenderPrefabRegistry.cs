using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds the centralized list of render prefabs.
/// Create via Assets > Create > Marbles > Render Prefab Registry.
/// </summary>
[CreateAssetMenu(fileName = "RenderPrefabRegistry", menuName = "Marbles/Render Prefab Registry")]
public class RenderPrefabRegistry : ScriptableObject
{
    [Tooltip("List of prefabs for rendering. Index = RenderPrefabID (0-based). ID -1 = no prefab.")]
    [SerializeField]
    private List<RenderPrefabRoot> renderPrefabs = new List<RenderPrefabRoot>();

    /// <summary>
    /// Get prefab by RenderPrefabID. Returns null if ID is invalid.
    /// </summary>
    public RenderPrefabRoot GetPrefabByID(int prefabID)
    {
        if (prefabID < 0 || prefabID >= renderPrefabs.Count)
            return null;

        return renderPrefabs[prefabID];
    }

    /// <summary>
    /// Get the render prefab ID from a GameObject.
    /// Looks for a RenderPrefabRoot component and returns its PrefabId.
    /// Returns -1 if not found.
    /// </summary>
    public short GetPrefabID(GameObject go)
    {
        if (go == null)
            return -1;

        var root = go.GetComponent<RenderPrefabRoot>();
        return root != null ? root.PrefabId : (short)-1;
    }

    /// <summary>
    /// Number of prefabs in the registry.
    /// </summary>
    public int Count => renderPrefabs.Count;

    /// <summary>
    /// Access the prefab list (for editor tools).
    /// </summary>
    public IReadOnlyList<RenderPrefabRoot> Prefabs => renderPrefabs;
}
