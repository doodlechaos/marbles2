using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using LockSim;
using UnityEngine;

/// <summary>
/// Renders a single tile's GameCoreObj hierarchy. Completely generic - works with any tile type
/// (GameTiles, ThroneTile, etc.) as long as you provide a TileRoot.
///
/// Multiple instances can be used to render multiple tiles.
/// All instances share the same RenderPrefabRegistry for prefab configuration.
/// </summary>
public abstract class TileRenderer : MonoBehaviour
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
    private TileBase currentTile;

    /// <summary>
    /// Update rendering for the assigned GameTile.
    /// This method automatically syncs the visual representation with the GameCoreObj tree,
    /// even after deserialization. Adds a GameTileBinding component to the root for debugging.
    /// </summary>
    public virtual void Render(TileBase gameTile)
    {
        // Detect if we're rendering a completely different tile
        bool tileChanged = currentTile != gameTile;

        if (gameTile == null || gameTile.TileRoot == null)
        {
            // If the game tile is null, clean up any existing rendering
            if (renderRoot != null)
            {
                DestroyGameObject(renderRoot);
                renderRoot = null;
                idToGameObject.Clear();
            }
            currentTile = null;
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
            currentTile = gameTile;
        }

        GameCoreObj tileRoot = gameTile.TileRoot;

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

            // Create the appropriate binding based on tile type
            EnsureTileBinding(renderRoot, gameTile);
        }
        else
        {
            // Update name if it changed
            if (renderRoot.name != expectedRootName)
            {
                renderRoot.name = expectedRootName;
            }

            // Always keep the binding up to date
            EnsureTileBinding(renderRoot, gameTile);
        }

        // Clear the "seen" set from previous frame
        seenIds.Clear();

        // Traverse the GameCoreObj tree and create/update GameObjects as needed
        // isInsidePrefabHierarchy = false at root level
        UpdateGCObjRecursive(tileRoot, renderRoot.transform, false);

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
    /// Recursively update or create GameObjects for GameCoreObjs in the hierarchy.
    /// </summary>
    private void UpdateGCObjRecursive(
        GameCoreObj gameCoreObj,
        Transform parentTransform,
        bool isInsidePrefabHierarchy
    )
    {
        // Skip level roots (they're just containers)
        if (IsLevelRoot(gameCoreObj))
        {
            // Still process children, but don't render this object
            if (gameCoreObj.Children != null)
            {
                foreach (var child in gameCoreObj.Children)
                {
                    UpdateGCObjRecursive(child, parentTransform, false);
                }
            }
            return;
        }

        // Mark this ID as seen
        seenIds.Add(gameCoreObj.RuntimeId);

        GameObject visualObj;

        // Check if we already have a GameObject for this RuntimeId
        if (idToGameObject.TryGetValue(gameCoreObj.RuntimeId, out visualObj))
        {
            // GameObject exists - just update its transform
            if (visualObj != null)
            {
                UpdateGameObjectTransform(visualObj, gameCoreObj.Transform);

                // Process children with this GameObject as parent
                if (gameCoreObj.Children != null)
                {
                    foreach (var child in gameCoreObj.Children)
                    {
                        UpdateGCObjRecursive(child, visualObj.transform, isInsidePrefabHierarchy);
                    }
                }
            }
            else
            {
                // GameObject was destroyed externally - recreate it
                CreateGameObjectForGCObj(gameCoreObj, parentTransform, isInsidePrefabHierarchy);
            }
        }
        else
        {
            // GameObject doesn't exist yet - create or find it
            CreateGameObjectForGCObj(gameCoreObj, parentTransform, isInsidePrefabHierarchy);
        }
    }

    /// <summary>
    /// Create or find a GameObject for a GameCoreObj that doesn't have one yet.
    /// </summary>
    private void CreateGameObjectForGCObj(
        GameCoreObj gameCoreObj,
        Transform parentTransform,
        bool isInsidePrefabHierarchy
    )
    {
        GameObject visualObj = null;
        bool didInstantiatePrefab = false;

        // Check if this is a prefab root that should be instantiated
        if (gameCoreObj.IsPrefabRoot)
        {
            // This is a prefab root - either instantiate or find it
            if (isInsidePrefabHierarchy)
            {
                // We're inside a parent prefab - the nested prefab instance should already exist
                Transform existingChild = parentTransform.Find(gameCoreObj.Name);
                if (existingChild != null)
                {
                    visualObj = existingChild.gameObject;
                    didInstantiatePrefab = true;
                    if (ShowDebugInfo)
                    {
                        Debug.Log(
                            $"Found existing nested prefab '{gameCoreObj.Name}' in parent prefab hierarchy"
                        );
                    }
                }
            }

            // If not found (or not inside a prefab), instantiate it
            if (visualObj == null)
            {
                GameObject prefabToInstantiate = GetPrefabByID(gameCoreObj.RenderPrefabID);
                if (prefabToInstantiate != null)
                {
                    visualObj = Instantiate(prefabToInstantiate, parentTransform);
                    visualObj.name = gameCoreObj.Name;
                    StripAuthoringComponents(visualObj);
                    didInstantiatePrefab = true;
                    if (ShowDebugInfo)
                    {
                        Debug.Log(
                            $"Instantiated prefab [{gameCoreObj.RenderPrefabID}] for '{gameCoreObj.Name}'"
                        );
                    }
                }
                else if (ShowDebugInfo)
                {
                    Debug.LogWarning(
                        $"RenderPrefabID {gameCoreObj.RenderPrefabID} is invalid for '{gameCoreObj.Name}'. Creating empty GameObject."
                    );
                }
            }
        }
        else if (isInsidePrefabHierarchy)
        {
            // Not a prefab root, but we're inside a prefab - find the existing child
            Transform existingChild = parentTransform.Find(gameCoreObj.Name);
            if (existingChild != null)
            {
                visualObj = existingChild.gameObject;
                if (ShowDebugInfo)
                {
                    Debug.Log($"Found existing child '{gameCoreObj.Name}' in prefab hierarchy");
                }
            }
        }

        // Fallback: create empty GameObject if we couldn't find or instantiate
        if (visualObj == null)
        {
            visualObj = new GameObject(gameCoreObj.Name);
            visualObj.transform.SetParent(parentTransform);
            if (ShowDebugInfo)
            {
                Debug.Log($"Created empty GameObject for '{gameCoreObj.Name}'");
            }
        }

        // Set transform
        UpdateGameObjectTransform(visualObj, gameCoreObj.Transform);

        // Add GCObjBinding component if not already present
        var existingBinding = visualObj.GetComponent<GCObjBinding>();
        if (existingBinding == null)
        {
            var binding = visualObj.AddComponent<GCObjBinding>();
            binding.GameCoreObj = gameCoreObj;
        }
        else
        {
            existingBinding.GameCoreObj = gameCoreObj;
        }

        // Auto-bind any GCBinding components on this GameObject to their corresponding GCComponents
        if (didInstantiatePrefab)
        {
            BindGCBindingComponents(visualObj, gameCoreObj);
        }

        // Store in mapping
        idToGameObject[gameCoreObj.RuntimeId] = visualObj;

        // Recursively process children
        if (gameCoreObj.Children != null)
        {
            bool childrenInsidePrefab = didInstantiatePrefab || isInsidePrefabHierarchy;
            foreach (var child in gameCoreObj.Children)
            {
                UpdateGCObjRecursive(child, visualObj.transform, childrenInsidePrefab);
            }
        }
    }

    private void EnsureTileBinding(GameObject root, TileBase tile)
    {
        if (root == null || tile == null)
            return;

        if (tile is GameTileBase gameTile)
        {
            GameTileBinding binding = root.GetOrAddComponent<GameTileBinding>();
            binding.GameTile = gameTile;
        }
        else if (tile is ThroneTile throneTile)
        {
            ThroneTileBinding binding = root.GetOrAddComponent<ThroneTileBinding>();
            binding.ThroneTile = throneTile;
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
    /// Check if a GameCoreObj is a level root (should not be rendered directly)
    /// </summary>
    private bool IsLevelRoot(GameCoreObj gameCoreObj)
    {
        return gameCoreObj.HasComponent<TileRootComponent>();
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
        currentTile = null;
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

        foreach (var collider in sim.Colliders)
        {
            // Get world transform from parent body if attached
            FPVector2 pos;
            FP rot;
            if (sim.TryGetBody(collider.ParentBodyId, out var body))
            {
                pos = collider.GetWorldPosition(body);
                rot = collider.GetWorldRotation(body);
            }
            else
            {
                pos = collider.LocalPosition;
                rot = collider.LocalRotation;
            }

            Vector3 worldPos = new Vector3(pos.X.ToFloat(), pos.Y.ToFloat(), 0f);

            if (collider.ShapeType == ShapeType.Box)
            {
                Vector3 size = new Vector3(
                    (collider.BoxShape.HalfWidth * FP.Two).ToFloat(),
                    (collider.BoxShape.HalfHeight * FP.Two).ToFloat(),
                    0.1f
                );

                Quaternion rotation = Quaternion.Euler(0, 0, rot.ToFloat() * Mathf.Rad2Deg);
                Matrix4x4 matrix = Matrix4x4.TRS(worldPos, rotation, Vector3.one);
                Gizmos.matrix = matrix;
                Gizmos.DrawWireCube(Vector3.zero, size);
                Gizmos.matrix = Matrix4x4.identity;
            }
            else if (collider.ShapeType == ShapeType.Circle)
            {
                Gizmos.DrawWireSphere(worldPos, collider.CircleShape.Radius.ToFloat());
            }
        }
    }

    /// <summary>
    /// Automatically binds any IGCBinding components on the GameObject to their
    /// corresponding GCComponents from the GameCoreObj.
    /// </summary>
    /// <param name="visualObj">The instantiated prefab GameObject.</param>
    /// <param name="gameCoreObj">The GameCoreObj to bind components from.</param>
    private void BindGCBindingComponents(GameObject visualObj, GameCoreObj gameCoreObj)
    {
        if (visualObj == null || gameCoreObj == null)
            return;

        // Find all IGCBinding components on this GameObject (not children - they get bound separately)
        var bindings = visualObj.GetComponents<IGCBinding>();
        foreach (var binding in bindings)
        {
            bool success = binding.TryBindToObject(gameCoreObj);
            if (ShowDebugInfo && success)
            {
                Debug.Log($"Bound {binding.GetType().Name} to {gameCoreObj.Name}");
            }
        }
    }

    /// <summary>
    /// Strip out authoring-only components from instantiated prefabs so the runtime view
    /// only shows client-side visuals.
    /// </summary>
    /// <param name="root">The instantiated prefab root.</param>
    private void StripAuthoringComponents(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        RemoveComponentsInChildren<GameComponentAuth>(root);
        RemoveComponentsInChildren<Rigidbody2D>(root);
        RemoveComponentsInChildren<Collider2D>(root);
    }

    private void RemoveComponentsInChildren<T>(GameObject root)
        where T : Component
    {
        T[] components = root.GetComponentsInChildren<T>(true);
        foreach (T component in components)
        {
            if (component == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(component);
            }
            else
            {
                DestroyImmediate(component);
            }
        }
    }
}
