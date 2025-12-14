using System;
using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using LockSim;
using UnityEngine;

/// <summary>
/// Base class for Unity components that bind to and render GameCore TileBase instances.
/// Handles the complete rendering pipeline: instantiating prefabs, syncing transforms,
/// and auto-binding GCBinding components.
/// </summary>
[Serializable]
public abstract class TileBinding : MonoBehaviour
{
    [Header("Prefab Configuration")]
    [Tooltip("Reference to the shared RenderPrefabRegistry asset")]
    [SerializeField]
    private RenderPrefabRegistry prefabRegistry;

    [Header("Debug")]
    [SerializeField]
    protected bool showDebugInfo = false;

    [SerializeField]
    protected bool showLockSimDebug = false;

    // Track mapping between RuntimeId and their visual GameObject representations
    private Dictionary<ulong, GameObject> idToGameObject = new Dictionary<ulong, GameObject>();

    // Root GameObject for the visual hierarchy
    private GameObject renderRoot;

    // Track which IDs were seen during the last Render pass
    private HashSet<ulong> seenIds = new HashSet<ulong>();

    // Track the currently rendered tile to detect when it changes
    private TileBase currentTile;

    /// <summary>
    /// Get the TileBase this binding is associated with.
    /// </summary>
    public abstract TileBase Tile { get; }

    /// <summary>
    /// Check if this binding has a valid tile reference.
    /// </summary>
    public abstract bool IsValid { get; }

    /// <summary>
    /// The tile's world ID.
    /// </summary>
    public byte TileWorldId => Tile?.TileWorldId ?? 0;

    /// <summary>
    /// The name of the tile type.
    /// </summary>
    public string TileTypeName => Tile?.GetType().Name ?? "Unknown";

    /// <summary>
    /// Render the tile hierarchy. Call this every frame to keep visuals in sync.
    /// </summary>
    public virtual void Render()
    {
        TileBase tile = Tile;

        // Detect if we're rendering a completely different tile
        bool tileChanged = currentTile != tile;

        if (tile == null || tile.TileRoot == null)
        {
            // If the tile is null, clean up any existing rendering
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
            currentTile = tile;
        }

        GameCoreObj tileRoot = tile.TileRoot;

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
        }
        else
        {
            // Update name if it changed
            if (renderRoot.name != expectedRootName)
            {
                renderRoot.name = expectedRootName;
            }
        }

        // Clear the "seen" set from previous frame
        seenIds.Clear();

        // Traverse the GameCoreObj tree and create/update GameObjects as needed
        UpdateGCObjRecursive(tileRoot, renderRoot.transform, false);

        // Destroy any GameObjects whose RuntimeIds were not seen
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
            if (visualObj != null)
            {
                UpdateGameObjectTransform(visualObj, gameCoreObj.Transform);

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
                CreateGameObjectForGCObj(gameCoreObj, parentTransform, isInsidePrefabHierarchy);
            }
        }
        else
        {
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
            if (isInsidePrefabHierarchy)
            {
                Transform existingChild = parentTransform.Find(gameCoreObj.Name);
                if (existingChild != null)
                {
                    visualObj = existingChild.gameObject;
                    didInstantiatePrefab = true;
                    if (showDebugInfo)
                    {
                        Debug.Log(
                            $"Found existing nested prefab '{gameCoreObj.Name}' in parent prefab hierarchy"
                        );
                    }
                }
            }

            if (visualObj == null)
            {
                GameObject prefabToInstantiate = GetPrefabByID(gameCoreObj.RenderPrefabID);
                if (prefabToInstantiate != null)
                {
                    visualObj = Instantiate(prefabToInstantiate, parentTransform);
                    visualObj.name = gameCoreObj.Name;
                    StripAuthoringComponents(visualObj);
                    didInstantiatePrefab = true;
                    if (showDebugInfo)
                    {
                        Debug.Log(
                            $"Instantiated prefab [{gameCoreObj.RenderPrefabID}] for '{gameCoreObj.Name}'"
                        );
                    }
                }
                else if (showDebugInfo)
                {
                    Debug.LogWarning(
                        $"RenderPrefabID {gameCoreObj.RenderPrefabID} is invalid for '{gameCoreObj.Name}'. Creating empty GameObject."
                    );
                }
            }
        }
        else if (isInsidePrefabHierarchy)
        {
            Transform existingChild = parentTransform.Find(gameCoreObj.Name);
            if (existingChild != null)
            {
                visualObj = existingChild.gameObject;
                if (showDebugInfo)
                {
                    Debug.Log($"Found existing child '{gameCoreObj.Name}' in prefab hierarchy");
                }
            }
        }

        // Fallback: create empty GameObject
        if (visualObj == null)
        {
            visualObj = new GameObject(gameCoreObj.Name);
            visualObj.transform.SetParent(parentTransform);
            if (showDebugInfo)
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

        // Auto-bind any GCBinding components on this GameObject
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

    /// <summary>
    /// Get prefab by RenderPrefabID from the registry.
    /// </summary>
    private GameObject GetPrefabByID(int prefabID)
    {
        if (prefabRegistry == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("TileBinding: No RenderPrefabRegistry assigned!");
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
    /// Helper method to destroy a GameObject appropriately for play/edit mode
    /// </summary>
    protected void DestroyGameObject(GameObject go)
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

    protected virtual void OnDestroy()
    {
        ClearRendering();
    }

    /// <summary>
    /// Automatically binds any IGCBinding components on the GameObject to their
    /// corresponding GCComponents from the GameCoreObj.
    /// </summary>
    private void BindGCBindingComponents(GameObject visualObj, GameCoreObj gameCoreObj)
    {
        if (visualObj == null || gameCoreObj == null)
            return;

        var bindings = visualObj.GetComponents<IGCBinding>();
        foreach (var binding in bindings)
        {
            bool success = binding.TryBindToObject(gameCoreObj);
            if (showDebugInfo && success)
            {
                Debug.Log($"Bound {binding.GetType().Name} to {gameCoreObj.Name}");
            }
        }
    }

    /// <summary>
    /// Strip out authoring-only components from instantiated prefabs.
    /// </summary>
    private void StripAuthoringComponents(GameObject root)
    {
        if (root == null)
            return;

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
                continue;

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

    #region Physics Debug Gizmos

    protected virtual void OnDrawGizmos()
    {
        if (Tile?.Sim == null || !showLockSimDebug)
            return;

        var world = Tile.Sim;
        Vector3 tileOffset = transform.position;

        DrawLockSimColliders(world, tileOffset);
        DrawActiveCollisions(world, tileOffset);
    }

    protected void DrawLockSimColliders(World world, Vector3 tileOffset)
    {
        foreach (var collider in world.Colliders)
        {
            FPVector2 pos;
            FP rot;
            bool isDynamic = false;

            if (world.TryGetBody(collider.ParentBodyId, out var body))
            {
                pos = collider.GetWorldPosition(body);
                rot = collider.GetWorldRotation(body);
                isDynamic = body.BodyType == BodyType.Dynamic;
            }
            else
            {
                pos = collider.LocalPosition;
                rot = collider.LocalRotation;
            }

            Gizmos.color = isDynamic ? Color.green : Color.blue;

            Vector3 colliderPos = new Vector3(pos.X.ToFloat(), pos.Y.ToFloat(), 0f) + tileOffset;
            float rotation = rot.ToFloat();

            if (collider.ShapeType == ShapeType.Box)
            {
                DrawWireBox(colliderPos, collider.BoxShape, rotation);
            }
            else if (collider.ShapeType == ShapeType.Circle)
            {
                DrawWireCircle(colliderPos, collider.CircleShape.Radius.ToFloat());
            }
        }
    }

    protected void DrawActiveCollisions(World world, Vector3 tileOffset)
    {
        var pairs = world.ActiveCollisionPairs;
        var dataList = world.ActiveCollisionData;

        if (pairs == null || dataList == null)
            return;

        const float contactRadius = 0.05f;
        const float normalLength = 0.4f;

        int count = Math.Min(pairs.Count, dataList.Count);

        for (int i = 0; i < count; i++)
        {
            var data = dataList[i];

            Vector3 contactPos =
                new Vector3(data.ContactPoint.X.ToFloat(), data.ContactPoint.Y.ToFloat(), 0f)
                + tileOffset;

            Gizmos.color = data.IsTrigger ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(contactPos, contactRadius);

            Vector3 n = new Vector3(data.Normal.X.ToFloat(), data.Normal.Y.ToFloat(), 0f);

            if (n.sqrMagnitude > 0.0001f)
            {
                Vector3 dir = n.normalized;
                Vector3 end = contactPos + dir * normalLength;

                Gizmos.DrawLine(contactPos, end);

                Vector3 right = Quaternion.AngleAxis(20f, Vector3.forward) * (-dir);
                Vector3 left = Quaternion.AngleAxis(-20f, Vector3.forward) * (-dir);
                Gizmos.DrawLine(end, end + right * 0.1f);
                Gizmos.DrawLine(end, end + left * 0.1f);
            }
        }
    }

    protected void DrawWireBox(Vector3 center, BoxShape box, float rotationRad)
    {
        float hw = box.HalfWidth.ToFloat();
        float hh = box.HalfHeight.ToFloat();

        float cos = Mathf.Cos(rotationRad);
        float sin = Mathf.Sin(rotationRad);

        Vector2[] localCorners = new Vector2[]
        {
            new Vector2(-hw, -hh),
            new Vector2(hw, -hh),
            new Vector2(hw, hh),
            new Vector2(-hw, hh),
        };

        Vector3[] worldCorners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            float rx = localCorners[i].x * cos - localCorners[i].y * sin;
            float ry = localCorners[i].x * sin + localCorners[i].y * cos;
            worldCorners[i] = center + new Vector3(rx, ry, 0f);
        }

        Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
        Gizmos.DrawLine(worldCorners[2], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[3], worldCorners[0]);
    }

    protected void DrawWireCircle(Vector3 center, float radius)
    {
        const int segments = 32;
        float angleStep = 2f * Mathf.PI / segments;

        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 point =
                center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }

    #endregion
}
