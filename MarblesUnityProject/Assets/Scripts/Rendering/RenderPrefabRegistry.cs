using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds the centralized list of render prefabs.
/// Create one instance via Assets > Create > Marbles > Render Prefab Registry.
/// Reference this from all TileRenderer instances to share the same prefab configuration.
/// </summary>
[CreateAssetMenu(fileName = "RenderPrefabRegistry", menuName = "Marbles/Render Prefab Registry")]
public class RenderPrefabRegistry : ScriptableObject
{
    [Tooltip(
        "List of prefabs for rendering. RenderPrefabID uses 0-based indexing (0 = first prefab, 1 = second, etc). ID -1 = no prefab."
    )]
    [SerializeField]
    private List<GameObject> renderPrefabs = new List<GameObject>();

    /// <summary>
    /// Get prefab by RenderPrefabID. Uses 0-based indexing.
    /// ID -1 = no prefab, ID 0 = first prefab in list, etc.
    /// </summary>
    public GameObject GetPrefabByID(int prefabID)
    {
        if (prefabID < 0)
            return null;

        if (prefabID < renderPrefabs.Count)
        {
            return renderPrefabs[prefabID];
        }

        return null;
    }

    /// <summary>
    /// Get the render prefab ID from a GameObject.
    /// Looks for a RenderPrefabIdentifier component on the GameObject.
    /// Returns -1 if not found or if the GameObject has no identifier.
    /// </summary>
    public int GetPrefabID(GameObject go)
    {
        if (go == null)
        {
            return -1;
        }

        // Look for the identifier component
        RenderPrefabIdentifier identifier = go.GetComponent<RenderPrefabIdentifier>();
        if (identifier != null)
        {
            return identifier.RenderPrefabID;
        }

        // No identifier found - this GameObject doesn't specify a render prefab
        return -1;
    }

    /// <summary>
    /// Number of prefabs in the registry.
    /// </summary>
    public int Count => renderPrefabs.Count;

    /// <summary>
    /// Access the prefab list (for editor tools like GameTileExporter).
    /// </summary>
    public IReadOnlyList<GameObject> Prefabs => renderPrefabs;
}
