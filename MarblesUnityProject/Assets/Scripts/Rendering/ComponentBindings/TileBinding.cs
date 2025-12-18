using System;
using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using LockSim;
using UnityEngine;

/// <summary>
/// Base class for Unity components that bind to and render GameCore TileBase instances.
/// Handles the rendering pipeline: spawning prefabs from pools, syncing transforms,
/// and managing the visual hierarchy lifecycle.
///
/// Key features:
/// - Every GameCoreObj gets a corresponding Unity GameObject with GCObjBinding
/// - Each GameObject handles its own binding through IGCBinding components
/// - Prefab children are matched by sibling index for robust binding
/// </summary>
[Serializable]
public abstract class TileBinding : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private RenderObjPools _pools;

    [Header("Debug")]
    [SerializeField]
    protected bool showDebugInfo = false;

    [SerializeField]
    protected bool showLockSimDebug = false;

    // Maps RuntimeId to visual representation
    private readonly Dictionary<ulong, VisualEntry> _visuals = new();

    // Tracks which IDs were seen during current Render pass
    private readonly HashSet<ulong> _seenIds = new();

    // Reusable list for cleanup operations
    private readonly List<ulong> _idsToRemove = new();

    // Root for the visual hierarchy
    private GameObject _renderRoot;

    // Currently rendered tile
    private TileBase _currentTile;

    private struct VisualEntry
    {
        public GameObject GameObject;
        public RenderPrefabRoot PrefabRoot; // Non-null if this is a pooled prefab root
        public bool OwnedByPrefab; // True if managed by a parent prefab's lifecycle
    }

    /// <summary>
    /// Get the TileBase this binding is associated with.
    /// </summary>
    public abstract TileBase Tile { get; }

    /// <summary>
    /// Check if this binding has a valid tile reference.
    /// </summary>
    public abstract bool IsValid { get; }

    public byte TileWorldId => Tile?.TileWorldId ?? 0;
    public string TileTypeName => Tile?.GetType().Name ?? "Unknown";

    /// <summary>
    /// Render the tile hierarchy. Call every frame to keep visuals in sync.
    /// </summary>
    public virtual void Render()
    {
        TileBase tile = Tile;
        bool tileChanged = _currentTile != tile;

        if (tile == null || tile.TileRoot == null)
        {
            ClearRendering();
            return;
        }

        if (tileChanged)
        {
            ClearRendering();
            _currentTile = tile;
        }

        GameCoreObj tileRoot = tile.TileRoot;

        // Ensure render root exists
        if (_renderRoot == null)
        {
            _renderRoot = new GameObject(tileRoot.Name);
            _renderRoot.transform.SetParent(transform);
            _renderRoot.transform.localPosition = Vector3.zero;
            _renderRoot.transform.localRotation = Quaternion.identity;
            _renderRoot.transform.localScale = Vector3.one;
        }
        else if (_renderRoot.name != tileRoot.Name)
        {
            _renderRoot.name = tileRoot.Name;
        }

        _seenIds.Clear();

        // Traverse and update the visual hierarchy
        UpdateVisualHierarchy(tileRoot, _renderRoot.transform, prefabCtx: null);

        // Cleanup visuals for objects no longer in the hierarchy
        CleanupUnseenVisuals();
    }

    private void UpdateVisualHierarchy(
        GameCoreObj gcObj,
        Transform parentTransform,
        RenderPrefabRoot prefabCtx
    )
    {
        // Skip TileRoot containers (they don't need visuals)
        if (gcObj.HasComponent<TileRootComponent>())
        {
            if (gcObj.Children != null)
            {
                foreach (var child in gcObj.Children)
                {
                    UpdateVisualHierarchy(child, parentTransform, prefabCtx);
                }
            }
            return;
        }

        _seenIds.Add(gcObj.RuntimeId);

        bool isNewVisual =
            !_visuals.TryGetValue(gcObj.RuntimeId, out var entry) || entry.GameObject == null;

        if (isNewVisual)
        {
            entry = CreateOrFindVisual(gcObj, parentTransform, prefabCtx);
            _visuals[gcObj.RuntimeId] = entry;
            BindGameObject(entry.GameObject, gcObj);
        }
        else
        {
            UpdateTransform(entry.GameObject, gcObj.Transform);
        }

        // Propagate prefab context to children
        var childPrefabCtx = entry.PrefabRoot ?? prefabCtx;

        if (gcObj.Children != null)
        {
            foreach (var child in gcObj.Children)
            {
                UpdateVisualHierarchy(child, entry.GameObject.transform, childPrefabCtx);
            }
        }
    }

    private VisualEntry CreateOrFindVisual(
        GameCoreObj gcObj,
        Transform parentTransform,
        RenderPrefabRoot prefabCtx
    )
    {
        // Inside a prefab: find existing child by sibling index
        if (prefabCtx != null)
        {
            return FindPrefabChild(gcObj, parentTransform);
        }

        // This is a prefab root: spawn from pool
        if (gcObj.RenderPrefabID >= 0 && _pools != null)
        {
            return SpawnPrefab(gcObj, parentTransform);
        }

        // No prefab: create empty GameObject
        return CreateEmptyVisual(gcObj, parentTransform);
    }

    private VisualEntry FindPrefabChild(GameCoreObj gcObj, Transform parentTransform)
    {
        Transform childTransform =
            gcObj.SiblingIndex >= 0 && gcObj.SiblingIndex < parentTransform.childCount
                ? parentTransform.GetChild(gcObj.SiblingIndex)
                : null;

        if (childTransform == null)
        {
            Debug.LogWarning(
                $"[TileBinding] Prefab child index {gcObj.SiblingIndex} out of range under '{parentTransform.name}'"
            );
            return CreateEmptyVisual(gcObj, parentTransform);
        }

        UpdateTransform(childTransform.gameObject, gcObj.Transform);

        return new VisualEntry { GameObject = childTransform.gameObject, OwnedByPrefab = true };
    }

    private VisualEntry SpawnPrefab(GameCoreObj gcObj, Transform parent)
    {
        var prefabRoot = _pools.Spawn(gcObj.RenderPrefabID, parent);
        prefabRoot.gameObject.name = gcObj.Name;
        UpdateTransform(prefabRoot.gameObject, gcObj.Transform);

        if (showDebugInfo)
            Debug.Log($"Spawned prefab [{gcObj.RenderPrefabID}] for '{gcObj.Name}'");

        return new VisualEntry { GameObject = prefabRoot.gameObject, PrefabRoot = prefabRoot };
    }

    private VisualEntry CreateEmptyVisual(GameCoreObj gcObj, Transform parent)
    {
        var go = new GameObject(gcObj.Name);
        go.transform.SetParent(parent);
        UpdateTransform(go, gcObj.Transform);

        if (showDebugInfo)
            Debug.Log($"Created empty GameObject for '{gcObj.Name}'");

        return new VisualEntry { GameObject = go };
    }

    /// <summary>
    /// Binds all IGCBinding components on a GameObject to the specified GameCoreObj.
    /// Ensures GCObjBinding exists for inspector visibility.
    /// </summary>
    private void BindGameObject(GameObject go, GameCoreObj gcObj)
    {
        // Ensure GCObjBinding exists
        var gcObjBinding = go.GetComponent<GCObjBinding>();
        if (gcObjBinding == null)
        {
            gcObjBinding = go.AddComponent<GCObjBinding>();
        }

        // Bind all IGCBinding components (including GCObjBinding)
        var bindings = go.GetComponents<IGCBinding>();
        foreach (var binding in bindings)
        {
            binding.Bind(gcObj);
        }
    }

    private void CleanupUnseenVisuals()
    {
        _idsToRemove.Clear();

        foreach (var kvp in _visuals)
        {
            if (!_seenIds.Contains(kvp.Key))
            {
                DestroyVisual(kvp.Value);
                _idsToRemove.Add(kvp.Key);
            }
        }

        foreach (var id in _idsToRemove)
        {
            _visuals.Remove(id);
        }
    }

    private void DestroyVisual(VisualEntry entry)
    {
        // Don't destroy children owned by a prefab - they're managed by the prefab lifecycle
        if (entry.OwnedByPrefab)
            return;

        if (entry.PrefabRoot != null && _pools != null)
        {
            _pools.Despawn(entry.PrefabRoot);
        }
        else if (entry.GameObject != null)
        {
            DestroyGameObject(entry.GameObject);
        }
    }

    private void UpdateTransform(GameObject go, FPTransform3D fpTransform)
    {
        if (go == null || fpTransform == null)
            return;

        go.transform.localPosition = new Vector3(
            fpTransform.LocalPosition.X.ToFloat(),
            fpTransform.LocalPosition.Y.ToFloat(),
            fpTransform.LocalPosition.Z.ToFloat()
        );

        go.transform.localRotation = new Quaternion(
            fpTransform.LocalRotation.X.ToFloat(),
            fpTransform.LocalRotation.Y.ToFloat(),
            fpTransform.LocalRotation.Z.ToFloat(),
            fpTransform.LocalRotation.W.ToFloat()
        );

        go.transform.localScale = new Vector3(
            fpTransform.LocalScale.X.ToFloat(),
            fpTransform.LocalScale.Y.ToFloat(),
            fpTransform.LocalScale.Z.ToFloat()
        );
    }

    public void ClearRendering()
    {
        foreach (var entry in _visuals.Values)
        {
            DestroyVisual(entry);
        }
        _visuals.Clear();
        _seenIds.Clear();

        if (_renderRoot != null)
        {
            DestroyGameObject(_renderRoot);
            _renderRoot = null;
        }

        _currentTile = null;
    }

    protected void DestroyGameObject(GameObject go)
    {
        if (Application.isPlaying)
            Destroy(go);
        else
            DestroyImmediate(go);
    }

    protected virtual void OnDestroy()
    {
        ClearRendering();
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
                DrawWireBox(colliderPos, collider.BoxShape, rotation);
            else if (collider.ShapeType == ShapeType.Circle)
                DrawWireCircle(colliderPos, collider.CircleShape.Radius.ToFloat());
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
