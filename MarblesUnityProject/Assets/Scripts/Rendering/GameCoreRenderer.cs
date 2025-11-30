using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using UnityEngine;

public class GameCoreRenderer : MonoBehaviour
{
    [Header("Rendering Settings")]
    [Tooltip("Update rendering every frame")]
    public bool AutoUpdate = true;

    [Header("Prefab Configuration")]
    [Tooltip(
        "List of prefabs for rendering. RenderPrefabID uses 0-based indexing (0 = first prefab, 1 = second, etc). ID -1 = no prefab."
    )]
    [SerializeField]
    private List<GameObject> renderPrefabs = new List<GameObject>();

    [Header("Tile Orchestrator")]
    [Tooltip("Reference to the TileOrchestrator for positioning render roots")]
    public TileOrchestrator TileOrchestrator;

    [Header("Debug")]
    public bool ShowDebugInfo = false;

    // Track mapping between RuntimeId and their visual GameObject representations for each tile
    private Dictionary<ulong, GameObject> tile1IdToGameObject = new Dictionary<ulong, GameObject>();
    private Dictionary<ulong, GameObject> tile2IdToGameObject = new Dictionary<ulong, GameObject>();

    // Root GameObjects for each tile's visual hierarchy
    private GameObject renderRoot1;
    private GameObject renderRoot2;

    // Track which IDs were seen during the last UpdateRendering pass for each tile
    private HashSet<ulong> tile1SeenIds = new HashSet<ulong>();
    private HashSet<ulong> tile2SeenIds = new HashSet<ulong>();

    private void Update()
    {
        if (AutoUpdate)
        {
            UpdateRendering();
        }
    }

    /// <summary>
    /// Update rendering for both GameTiles from GameCore.
    /// This method automatically syncs the visual representation with the GameCore state,
    /// even after deserialization.
    /// </summary>
    public void UpdateRendering()
    {
        if (GameManager.Inst == null || GameManager.Inst.GameCore == null)
            return;

        GameCore gameCore = GameManager.Inst.GameCore;

        // Update GameTile1
        UpdateGameTile(
            gameCore.GameTile1,
            ref renderRoot1,
            tile1IdToGameObject,
            tile1SeenIds,
            "GameTile1",
            TileOrchestrator.GameTile1Origin
        );

        // Update GameTile2
        UpdateGameTile(
            gameCore.GameTile2,
            ref renderRoot2,
            tile2IdToGameObject,
            tile2SeenIds,
            "GameTile2",
            TileOrchestrator.GameTile2Origin
        );
    }

    /// <summary>
    /// Update rendering for a specific GameTile
    /// </summary>
    private void UpdateGameTile(
        GameTileBase gameTile,
        ref GameObject renderRoot,
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds,
        string tileName,
        Transform originTransform
    )
    {
        if (gameTile == null || gameTile.TileRoot == null)
        {
            // If the tile or its root is null, clean up any existing rendering
            if (renderRoot != null)
            {
                DestroyGameObject(renderRoot);
                renderRoot = null;
                idToGameObject.Clear();
            }
            return;
        }

        // Get the specific game type name for better identification
        string gameTypeName = gameTile.GetType().Name;
        string expectedRootName = $"RenderRoot_{tileName}_{gameTypeName}";

        // Ensure we have a render root
        if (renderRoot == null)
        {
            renderRoot = new GameObject(expectedRootName);
            renderRoot.transform.SetParent(originTransform);
            renderRoot.transform.localPosition = Vector3.zero;
            renderRoot.transform.localRotation = Quaternion.identity;
            renderRoot.transform.localScale = Vector3.one;

            // Add GameTileBinding component
            var tileBinding = renderRoot.AddComponent<GameTileBinding>();
            tileBinding.GameTile = gameTile;
        }
        else
        {
            // Update name if game type changed (e.g., after loading a different game)
            if (renderRoot.name != expectedRootName)
            {
                renderRoot.name = expectedRootName;
            }

            // Update the GameTileBinding reference in case the GameTile instance changed
            var tileBinding = renderRoot.GetComponent<GameTileBinding>();
            if (tileBinding != null && tileBinding.GameTile != gameTile)
            {
                tileBinding.GameTile = gameTile;
            }
        }

        // Clear the "seen" set from previous frame
        seenIds.Clear();

        // Traverse the RuntimeObj tree and create/update GameObjects as needed
        // isInsidePrefabHierarchy = false at root level
        UpdateRuntimeObjRecursive(
            gameTile.TileRoot,
            renderRoot.transform,
            idToGameObject,
            seenIds,
            false
        );

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
    /// <param name="isInsidePrefabHierarchy">True if we're processing children of an instantiated prefab.
    /// When true, non-prefab-root children should be found in the existing hierarchy rather than created new.</param>
    private void UpdateRuntimeObjRecursive(
        RuntimeObj runtimeObj,
        Transform parentTransform,
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds,
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
                    UpdateRuntimeObjRecursive(
                        child,
                        parentTransform,
                        idToGameObject,
                        seenIds,
                        false
                    );
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
                // Children inherit the isInsidePrefabHierarchy status (they're still inside the same prefab)
                if (runtimeObj.Children != null)
                {
                    foreach (var child in runtimeObj.Children)
                    {
                        UpdateRuntimeObjRecursive(
                            child,
                            visualObj.transform,
                            idToGameObject,
                            seenIds,
                            isInsidePrefabHierarchy
                        );
                    }
                }
            }
            else
            {
                // GameObject was destroyed externally - recreate it
                CreateGameObjectForRuntimeObj(
                    runtimeObj,
                    parentTransform,
                    idToGameObject,
                    seenIds,
                    isInsidePrefabHierarchy
                );
            }
        }
        else
        {
            // GameObject doesn't exist yet - create or find it
            CreateGameObjectForRuntimeObj(
                runtimeObj,
                parentTransform,
                idToGameObject,
                seenIds,
                isInsidePrefabHierarchy
            );
        }
    }

    /// <summary>
    /// Create or find a GameObject for a RuntimeObj that doesn't have one yet.
    /// This can happen after deserialization or when new objects are added to the tree.
    /// </summary>
    /// <param name="isInsidePrefabHierarchy">True if we're inside an instantiated prefab's hierarchy.
    /// When true and the RuntimeObj is not a prefab root, we try to find the existing child GO
    /// rather than creating a new one.</param>
    private void CreateGameObjectForRuntimeObj(
        RuntimeObj runtimeObj,
        Transform parentTransform,
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds,
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
                    didInstantiatePrefab = true; // Children are from this nested prefab
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
            // Update the existing binding's reference
            existingBinding.RuntimeObj = runtimeObj;
        }

        // Store in mapping
        idToGameObject[runtimeObj.RuntimeId] = visualObj;

        // Recursively process children
        // If we instantiated a prefab, children are inside that prefab hierarchy
        if (runtimeObj.Children != null)
        {
            bool childrenInsidePrefab = didInstantiatePrefab || isInsidePrefabHierarchy;
            foreach (var child in runtimeObj.Children)
            {
                UpdateRuntimeObjRecursive(
                    child,
                    visualObj.transform,
                    idToGameObject,
                    seenIds,
                    childrenInsidePrefab
                );
            }
        }
    }

    /// <summary>
    /// Get prefab by RenderPrefabID. Uses 0-based indexing.
    /// ID -1 = no prefab, ID 0 = first prefab in list, etc.
    /// </summary>
    private GameObject GetPrefabByID(int prefabID)
    {
        if (prefabID < 0)
            return null;

        if (prefabID >= 0 && prefabID < renderPrefabs.Count)
        {
            return renderPrefabs[prefabID];
        }

        return null;
    }

    /// <summary>
    /// Check if a RuntimeObj is a level root (should not be rendered directly)
    /// </summary>
    private bool IsLevelRoot(RuntimeObj runtimeObj)
    {
        // Check for LevelRootComponent marker
        return runtimeObj.HasComponent<LevelRootComponent>();
    }

    /// <summary>
    /// Update Unity GameObject transform from FPTransform3D
    /// </summary>
    private void UpdateGameObjectTransform(GameObject go, FPTransform3D fpTransform)
    {
        if (go == null || fpTransform == null)
            return;

        // Convert fixed-point values to Unity floats
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
    /// Clear all rendered GameObjects for both tiles
    /// </summary>
    public void ClearRendering()
    {
        ClearTileRendering(ref renderRoot1, tile1IdToGameObject, tile1SeenIds);
        ClearTileRendering(ref renderRoot2, tile2IdToGameObject, tile2SeenIds);
    }

    /// <summary>
    /// Clear rendering for a specific tile
    /// </summary>
    private void ClearTileRendering(
        ref GameObject renderRoot,
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds
    )
    {
        if (renderRoot != null)
        {
            DestroyGameObject(renderRoot);
            renderRoot = null;
        }

        idToGameObject.Clear();
        seenIds.Clear();
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
        if (!ShowDebugInfo || GameManager.Inst == null || GameManager.Inst.GameCore == null)
            return;

        // Draw physics bodies for both tiles
        DrawPhysicsGizmos(GameManager.Inst.GameCore.GameTile1);
        DrawPhysicsGizmos(GameManager.Inst.GameCore.GameTile2);
    }

    /// <summary>
    /// Draw physics debug gizmos for a specific GameTile
    /// </summary>
    private void DrawPhysicsGizmos(GameTileBase gameTile)
    {
        if (gameTile == null || gameTile.Sim == null)
            return;

        Gizmos.color = Color.green;

        foreach (var body in gameTile.Sim.Bodies)
        {
            Vector3 pos = new Vector3(body.Position.X.ToFloat(), body.Position.Y.ToFloat(), 0f);

            if (body.ShapeType == LockSim.ShapeType.Box)
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
            else if (body.ShapeType == LockSim.ShapeType.Circle)
            {
                Gizmos.DrawWireSphere(pos, body.CircleShape.Radius.ToFloat());
            }
        }
    }
}
