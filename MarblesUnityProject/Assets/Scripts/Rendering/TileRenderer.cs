using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using LockSim;
using UnityEngine;

/// <summary>
/// Renders a single tile's RuntimeObj hierarchy. Completely generic - works with any tile type
/// (GameTiles, ThroneTile, etc.) as long as you provide a TileRoot.
///
/// Multiple instances can be used to render multiple tiles.
/// All instances share the same RenderPrefabRegistry for prefab configuration.
/// </summary>
public class TileRenderer : MonoBehaviour
{
    [Header("Prefab Configuration")]
    [Tooltip("Reference to the shared RenderPrefabRegistry asset")]
    [SerializeField]
    private RenderPrefabRegistry prefabRegistry;

    [Header("Debug")]
    public bool ShowDebugInfo = false;

    [Tooltip(
        "Optional: Physics simulation for gizmo drawing. Set this if you want to see physics debug visuals."
    )]
    public World PhysicsSim;

    // Track mapping between RuntimeId and their visual GameObject representations
    private Dictionary<ulong, GameObject> idToGameObject = new Dictionary<ulong, GameObject>();

    // Root GameObject for the visual hierarchy
    private GameObject renderRoot;

    // Track which IDs were seen during the last UpdateRendering pass
    private HashSet<ulong> seenIds = new HashSet<ulong>();

    // Track the currently rendered tile to detect when it changes
    private GameTileBase currentGameTile;

    /// <summary>
    /// Update rendering for the assigned GameTile.
    /// This method automatically syncs the visual representation with the RuntimeObj tree,
    /// even after deserialization. Adds a GameTileBinding component to the root for debugging.
    /// </summary>
    public void Render(GameTileBase gameTile)
    {
        // Detect if we're rendering a completely different tile
        bool tileChanged = currentGameTile != gameTile;

        if (gameTile == null || gameTile.TileRoot == null)
        {
            // If the game tile is null, clean up any existing rendering
            if (renderRoot != null)
            {
                DestroyGameObject(renderRoot);
                renderRoot = null;
                idToGameObject.Clear();
            }
            currentGameTile = null;
            return;
        }

        // If tile changed, clear everything and start fresh
        if (tileChanged)
        {
            if (renderRoot != null)
            {
                DestroyGameObject(renderRoot);
                renderRoot = null;
                idToGameObject.Clear();
            }
            currentGameTile = gameTile;
        }

        RuntimeObj tileRoot = gameTile.TileRoot;

        // Determine the root name
        string expectedRootName = tileRoot.Name;

        // Ensure we have a render root
        if (renderRoot == null)
        {
            renderRoot = new GameObject(expectedRootName);
            renderRoot.transform.SetParent(transform);
            renderRoot.transform.localPosition = Vector3.zero;
            renderRoot.transform.localRotation = Quaternion.identity;
            renderRoot.transform.localScale = Vector3.one;

            var gameTileBinding = renderRoot.AddComponent<GameTileBinding>();
            gameTileBinding.GameTile = gameTile;
        }
        else
        {
            // Update name if it changed
            if (renderRoot.name != expectedRootName)
            {
                renderRoot.name = expectedRootName;
            }

            // Always keep the binding up to date
            var gameTileBinding = renderRoot.GetComponent<GameTileBinding>();
            if (gameTileBinding != null)
            {
                gameTileBinding.GameTile = gameTile;
            }
        }

        // Clear the "seen" set from previous frame
        seenIds.Clear();

        // Traverse the RuntimeObj tree and create/update GameObjects as needed
        // isInsidePrefabHierarchy = false at root level
        UpdateRuntimeObjRecursive(tileRoot, renderRoot.transform, false);

        // Destroy any GameObjects whose RuntimeIds were not seen (they've been removed from the tree)
        List<ulong> idsToRemove = new List<ulong>();
        foreach (var kvp in idToGameObject)
        {
            ulong id = kvp.Key;
            if (!seenIds.Contains(id))
            {
                GameObject go = kvp.Value;
                if (go != null)
                {
                    DestroyGameObject(go);
                }
                idsToRemove.Add(id);
            }
        }

        // Remove destroyed objects from the mapping
        foreach (var id in idsToRemove)
        {
            idToGameObject.Remove(id);
        }
    }

    /// <summary>
    /// Recursively update or create GameObjects for RuntimeObjs in the hierarchy.
    /// </summary>
    private void UpdateRuntimeObjRecursive(
        RuntimeObj runtimeObj,
        Transform parentTransform,
        bool isInsidePrefabHierarchy
    )
    {
        // Skip level roots (they're just containers)
        if (IsLevelRoot(runtimeObj))
        {
            // Still process children, but don't render this object
            if (runtimeObj.Children != null)
            {
                foreach (var child in runtimeObj.Children)
                {
                    UpdateRuntimeObjRecursive(child, parentTransform, false);
                }
            }
            return;
        }

        // Mark this ID as seen
        seenIds.Add(runtimeObj.RuntimeId);

        GameObject visualObj;

        // Check if we already have a GameObject for this RuntimeId
        if (idToGameObject.TryGetValue(runtimeObj.RuntimeId, out visualObj))
        {
            // GameObject exists - just update its transform
            if (visualObj != null)
            {
                UpdateGameObjectTransform(visualObj, runtimeObj.Transform);

                // Process children with this GameObject as parent
                if (runtimeObj.Children != null)
                {
                    foreach (var child in runtimeObj.Children)
                    {
                        UpdateRuntimeObjRecursive(
                            child,
                            visualObj.transform,
                            isInsidePrefabHierarchy
                        );
                    }
                }
            }
            else
            {
                // GameObject was destroyed externally - recreate it
                CreateGameObjectForRuntimeObj(runtimeObj, parentTransform, isInsidePrefabHierarchy);
            }
        }
        else
        {
            // GameObject doesn't exist yet - create or find it
            CreateGameObjectForRuntimeObj(runtimeObj, parentTransform, isInsidePrefabHierarchy);
        }
    }

    /// <summary>
    /// Create or find a GameObject for a RuntimeObj that doesn't have one yet.
    /// </summary>
    private void CreateGameObjectForRuntimeObj(
        RuntimeObj runtimeObj,
        Transform parentTransform,
        bool isInsidePrefabHierarchy
    )
    {
        GameObject visualObj = null;
        bool didInstantiatePrefab = false;

        // Check if this is a prefab root that should be instantiated
        if (runtimeObj.IsPrefabRoot)
        {
            // This is a prefab root - either instantiate or find it
            if (isInsidePrefabHierarchy)
            {
                // We're inside a parent prefab - the nested prefab instance should already exist
                Transform existingChild = parentTransform.Find(runtimeObj.Name);
                if (existingChild != null)
                {
                    visualObj = existingChild.gameObject;
                    didInstantiatePrefab = true;
                    if (ShowDebugInfo)
                    {
                        Debug.Log(
                            $"Found existing nested prefab '{runtimeObj.Name}' in parent prefab hierarchy"
                        );
                    }
                }
            }

            // If not found (or not inside a prefab), instantiate it
            if (visualObj == null)
            {
                GameObject prefabToInstantiate = GetPrefabByID(runtimeObj.RenderPrefabID);
                if (prefabToInstantiate != null)
                {
                    visualObj = Instantiate(prefabToInstantiate, parentTransform);
                    visualObj.name = runtimeObj.Name;
                    didInstantiatePrefab = true;
                    if (ShowDebugInfo)
                    {
                        Debug.Log(
                            $"Instantiated prefab [{runtimeObj.RenderPrefabID}] for '{runtimeObj.Name}'"
                        );
                    }
                }
                else if (ShowDebugInfo)
                {
                    Debug.LogWarning(
                        $"RenderPrefabID {runtimeObj.RenderPrefabID} is invalid for '{runtimeObj.Name}'. Creating empty GameObject."
                    );
                }
            }
        }
        else if (isInsidePrefabHierarchy)
        {
            // Not a prefab root, but we're inside a prefab - find the existing child
            Transform existingChild = parentTransform.Find(runtimeObj.Name);
            if (existingChild != null)
            {
                visualObj = existingChild.gameObject;
                if (ShowDebugInfo)
                {
                    Debug.Log($"Found existing child '{runtimeObj.Name}' in prefab hierarchy");
                }
            }
        }

        // Fallback: create empty GameObject if we couldn't find or instantiate
        if (visualObj == null)
        {
            visualObj = new GameObject(runtimeObj.Name);
            visualObj.transform.SetParent(parentTransform);
            if (ShowDebugInfo)
            {
                Debug.Log($"Created empty GameObject for '{runtimeObj.Name}'");
            }
        }

        // Set transform
        UpdateGameObjectTransform(visualObj, runtimeObj.Transform);

        // Add RuntimeBinding component if not already present
        var existingBinding = visualObj.GetComponent<RuntimeBinding>();
        if (existingBinding == null)
        {
            var binding = visualObj.AddComponent<RuntimeBinding>();
            binding.RuntimeObj = runtimeObj;
        }
        else
        {
            existingBinding.RuntimeObj = runtimeObj;
        }

        // Store in mapping
        idToGameObject[runtimeObj.RuntimeId] = visualObj;

        // Recursively process children
        if (runtimeObj.Children != null)
        {
            bool childrenInsidePrefab = didInstantiatePrefab || isInsidePrefabHierarchy;
            foreach (var child in runtimeObj.Children)
            {
                UpdateRuntimeObjRecursive(child, visualObj.transform, childrenInsidePrefab);
            }
        }
    }

    /// <summary>
    /// Get prefab by RenderPrefabID from the registry.
    /// </summary>
    private GameObject GetPrefabByID(int prefabID)
    {
        if (prefabRegistry == null)
        {
            if (ShowDebugInfo)
            {
                Debug.LogWarning("TileRenderer: No RenderPrefabRegistry assigned!");
            }
            return null;
        }

        return prefabRegistry.GetPrefabByID(prefabID);
    }

    /// <summary>
    /// Check if a RuntimeObj is a level root (should not be rendered directly)
    /// </summary>
    private bool IsLevelRoot(RuntimeObj runtimeObj)
    {
        return runtimeObj.HasComponent<LevelRootComponent>();
    }

    /// <summary>
    /// Update Unity GameObject transform from FPTransform3D
    /// </summary>
    private void UpdateGameObjectTransform(GameObject go, FPTransform3D fpTransform)
    {
        if (go == null || fpTransform == null)
            return;

        Vector3 position = new Vector3(
            fpTransform.LocalPosition.X.ToFloat(),
            fpTransform.LocalPosition.Y.ToFloat(),
            fpTransform.LocalPosition.Z.ToFloat()
        );

        Quaternion rotation = new Quaternion(
            fpTransform.LocalRotation.X.ToFloat(),
            fpTransform.LocalRotation.Y.ToFloat(),
            fpTransform.LocalRotation.Z.ToFloat(),
            fpTransform.LocalRotation.W.ToFloat()
        );

        Vector3 scale = new Vector3(
            fpTransform.LocalScale.X.ToFloat(),
            fpTransform.LocalScale.Y.ToFloat(),
            fpTransform.LocalScale.Z.ToFloat()
        );

        go.transform.localPosition = position;
        go.transform.localRotation = rotation;
        go.transform.localScale = scale;
    }

    /// <summary>
    /// Clear all rendered GameObjects
    /// </summary>
    public void ClearRendering()
    {
        if (renderRoot != null)
        {
            DestroyGameObject(renderRoot);
            renderRoot = null;
        }

        idToGameObject.Clear();
        seenIds.Clear();
        currentGameTile = null;
    }

    /// <summary>
    /// Helper method to destroy a GameObject with appropriate method for play mode vs edit mode
    /// </summary>
    private void DestroyGameObject(GameObject go)
    {
        if (Application.isPlaying)
        {
            Destroy(go);
        }
        else
        {
            DestroyImmediate(go);
        }
    }

    private void OnDestroy()
    {
        ClearRendering();
    }

    private void OnDrawGizmos()
    {
        if (!ShowDebugInfo || PhysicsSim == null)
            return;

        DrawPhysicsGizmos(PhysicsSim);
    }

    /// <summary>
    /// Draw physics debug gizmos for the assigned simulation
    /// </summary>
    private void DrawPhysicsGizmos(World sim)
    {
        if (sim == null)
            return;

        Gizmos.color = Color.green;

        foreach (var body in sim.Bodies)
        {
            Vector3 pos = new Vector3(body.Position.X.ToFloat(), body.Position.Y.ToFloat(), 0f);

            if (body.ShapeType == ShapeType.Box)
            {
                Vector3 size = new Vector3(
                    (body.BoxShape.HalfWidth * FP.Two).ToFloat(),
                    (body.BoxShape.HalfHeight * FP.Two).ToFloat(),
                    0.1f
                );

                Quaternion rot = Quaternion.Euler(0, 0, body.Rotation.ToFloat() * Mathf.Rad2Deg);
                Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, Vector3.one);
                Gizmos.matrix = matrix;
                Gizmos.DrawWireCube(Vector3.zero, size);
                Gizmos.matrix = Matrix4x4.identity;
            }
            else if (body.ShapeType == ShapeType.Circle)
            {
                Gizmos.DrawWireSphere(pos, body.CircleShape.Radius.ToFloat());
            }
        }
    }
}
