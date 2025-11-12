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
            "GameTile1"
        );

        // Update GameTile2
        UpdateGameTile(
            gameCore.GameTile2,
            ref renderRoot2,
            tile2IdToGameObject,
            tile2SeenIds,
            "GameTile2"
        );
    }

    /// <summary>
    /// Update rendering for a specific GameTile
    /// </summary>
    private void UpdateGameTile(
        GameTile gameTile,
        ref GameObject renderRoot,
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds,
        string tileName
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

        // Ensure we have a render root
        if (renderRoot == null)
        {
            renderRoot = new GameObject($"RenderRoot_{tileName}");
            renderRoot.transform.SetParent(transform);
            renderRoot.transform.localPosition = Vector3.zero;
            renderRoot.transform.localRotation = Quaternion.identity;
            renderRoot.transform.localScale = Vector3.one;
        }

        // Clear the "seen" set from previous frame
        seenIds.Clear();

        // Traverse the RuntimeObj tree and create/update GameObjects as needed
        UpdateRuntimeObjRecursive(gameTile.TileRoot, renderRoot.transform, idToGameObject, seenIds);

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
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds
    )
    {
        // Skip GameTileAuth components (they're authoring-only)
        if (HasGameTileAuthComponent(runtimeObj))
        {
            // Still process children, but don't render this object
            if (runtimeObj.Children != null)
            {
                foreach (var child in runtimeObj.Children)
                {
                    UpdateRuntimeObjRecursive(child, parentTransform, idToGameObject, seenIds);
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
                            idToGameObject,
                            seenIds
                        );
                    }
                }
            }
            else
            {
                // GameObject was destroyed externally - recreate it
                CreateGameObjectForRuntimeObj(runtimeObj, parentTransform, idToGameObject, seenIds);
            }
        }
        else
        {
            // GameObject doesn't exist yet - create it
            CreateGameObjectForRuntimeObj(runtimeObj, parentTransform, idToGameObject, seenIds);
        }
    }

    /// <summary>
    /// Create a new GameObject for a RuntimeObj that doesn't have one yet.
    /// This can happen after deserialization or when new objects are added to the tree.
    /// </summary>
    private void CreateGameObjectForRuntimeObj(
        RuntimeObj runtimeObj,
        Transform parentTransform,
        Dictionary<ulong, GameObject> idToGameObject,
        HashSet<ulong> seenIds
    )
    {
        GameObject visualObj = null;

        // Look up prefab by RenderPrefabID
        GameObject prefabToInstantiate = GetPrefabByID(runtimeObj.RenderPrefabID);

        if (prefabToInstantiate != null)
        {
            // Instantiate the prefab
            visualObj = Instantiate(prefabToInstantiate, parentTransform);
            visualObj.name = runtimeObj.Name;
        }
        else
        {
            // No prefab (ID -1 or invalid ID), create empty GameObject
            visualObj = new GameObject(runtimeObj.Name);
            visualObj.transform.SetParent(parentTransform);

            if (ShowDebugInfo && runtimeObj.RenderPrefabID >= 0)
            {
                Debug.LogWarning(
                    $"RenderPrefabID {runtimeObj.RenderPrefabID} is invalid for '{runtimeObj.Name}'. Creating empty GameObject."
                );
            }
        }

        // Set transform
        UpdateGameObjectTransform(visualObj, runtimeObj.Transform);

        // Add RuntimeBinding component
        var binding = visualObj.AddComponent<RuntimeBinding>();
        binding.RuntimeId = runtimeObj.RuntimeId;

        // Store in mapping
        idToGameObject[runtimeObj.RuntimeId] = visualObj;

        if (ShowDebugInfo)
        {
            Debug.Log(
                $"Created GameObject for RuntimeObj '{runtimeObj.Name}' (ID: {runtimeObj.RuntimeId})"
            );
        }

        // Recursively create children
        if (runtimeObj.Children != null)
        {
            foreach (var child in runtimeObj.Children)
            {
                UpdateRuntimeObjRecursive(child, visualObj.transform, idToGameObject, seenIds);
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
    /// Check if a RuntimeObj has a GameTileAuth component
    /// </summary>
    private bool HasGameTileAuthComponent(RuntimeObj runtimeObj)
    {
        if (runtimeObj.Components == null)
            return false;

        foreach (var component in runtimeObj.Components)
        {
            if (component.type != null && component.type.Contains("GameTileAuth"))
            {
                return true;
            }
        }

        return false;
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
    private void DrawPhysicsGizmos(GameTile gameTile)
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
