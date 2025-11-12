using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using UnityEngine;

public class RuntimeRenderer : MonoBehaviour
{
    [Header("Rendering Settings")]
    [Tooltip("The GameTile to render from GameCore")]
    public int GameTileId = 1;

    [Tooltip("Update rendering every frame")]
    public bool AutoUpdate = true;

    [Header("Prefab Configuration")]
    [Tooltip("List of prefabs for rendering. RenderPrefabID uses 0-based indexing (0 = first prefab, 1 = second, etc). ID -1 = no prefab.")]
    [SerializeField] private List<GameObject> renderPrefabs = new List<GameObject>();

    [Header("Debug")]
    public bool ShowDebugInfo = false;

    // Track mapping between RuntimeObj and their visual GameObject representations
    private Dictionary<RuntimeObj, GameObject> runtimeObjToGameObject = new Dictionary<RuntimeObj, GameObject>();

    // Reference to the GameTile we're rendering
    private GameTile gameTile;

    // Root GameObject for this tile's visual hierarchy
    private GameObject renderRoot;

    private void Update()
    {
        if (AutoUpdate)
        {
            UpdateRendering();
        }
    }

    /// <summary>
    /// Render the specified GameTile
    /// </summary>
    public void RenderGameTile(GameTile tile)
    {
        if (tile == null)
        {
            Debug.LogError("Cannot render null GameTile");
            return;
        }

        // Clear existing rendering
        ClearRendering();

        gameTile = tile;

        if (gameTile.TileRoot == null)
        {
            if (ShowDebugInfo)
            {
                Debug.Log("GameTile has no TileRoot to render");
            }
            return;
        }

        // Create root GameObject for this tile
        renderRoot = new GameObject($"RenderRoot_Tile{gameTile.WorldId}");
        renderRoot.transform.SetParent(transform);
        renderRoot.transform.localPosition = Vector3.zero;
        renderRoot.transform.localRotation = Quaternion.identity;
        renderRoot.transform.localScale = Vector3.one;

        // Recursively create the visual hierarchy
        CreateVisualHierarchy(gameTile.TileRoot, renderRoot.transform);

        if (ShowDebugInfo)
        {
            Debug.Log($"RuntimeRenderer: Rendered GameTile {gameTile.WorldId} with {runtimeObjToGameObject.Count} objects");
        }
    }

    /// <summary>
    /// Automatically render the GameTile from GameManager based on GameTileId
    /// </summary>
    public void RenderFromGameManager()
    {
        if (GameManager.Inst == null)
        {
            Debug.LogError("GameManager.Instance is null");
            return;
        }

        GameTile tile = null;

        if (GameTileId == 1)
        {
            tile = GameManager.Inst.GameCore.GameTile1;
        }
        else if (GameTileId == 2)
        {
            tile = GameManager.Inst.GameCore.GameTile2;
        }

        if (tile != null)
        {
            RenderGameTile(tile);
        }
        else
        {
            Debug.LogError($"GameTile {GameTileId} not found");
        }
    }

    /// <summary>
    /// Recursively create visual GameObject hierarchy from RuntimeObj hierarchy
    /// </summary>
    private void CreateVisualHierarchy(RuntimeObj runtimeObj, Transform parentTransform)
    {
        // Check if this RuntimeObj has a GameTileAuth component - if so, skip rendering it
        // GameTileAuth is just an authoring marker and shouldn't be visually rendered
        if (HasGameTileAuthComponent(runtimeObj))
        {
            // Don't create a visual object for this, but still process children
            if (runtimeObj.Children != null)
            {
                foreach (var child in runtimeObj.Children)
                {
                    CreateVisualHierarchy(child, parentTransform);
                }
            }
            return;
        }

        GameObject visualObj = null;

        // Look up prefab by RenderPrefabID
        GameObject prefabToInstantiate = GetPrefabByID(runtimeObj.RenderPrefabID);

        if (prefabToInstantiate != null)
        {
            // Instantiate the prefab
            visualObj = Instantiate(prefabToInstantiate, parentTransform);
            visualObj.name = runtimeObj.Name; // Keep original RuntimeObj name

            //If the prefab contained any authored data components that affect visuals, we can set them here. 
        }
        else
        {
            // No prefab (ID -1 or invalid ID), create empty GameObject
            visualObj = new GameObject(runtimeObj.Name);
            visualObj.transform.SetParent(parentTransform);

            if (ShowDebugInfo && runtimeObj.RenderPrefabID >= 0)
            {
                Debug.LogWarning($"RenderPrefabID {runtimeObj.RenderPrefabID} is invalid for '{runtimeObj.Name}'. Creating empty GameObject.");
            }
        }

        // Set transform from RuntimeObj's FPTransform3D
        UpdateGameObjectTransform(visualObj, runtimeObj.Transform);

        // Store mapping
        runtimeObjToGameObject[runtimeObj] = visualObj;

        // Recursively create children
        if (runtimeObj.Children != null)
        {
            foreach (var child in runtimeObj.Children)
            {
                CreateVisualHierarchy(child, visualObj.transform);
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
    /// Update all visual GameObjects from their RuntimeObj transforms
    /// Call this after physics simulation steps to update visuals
    /// </summary>
    public void UpdateRendering()
    {
        if (gameTile == null || gameTile.TileRoot == null)
            return;

        foreach (var kvp in runtimeObjToGameObject)
        {
            RuntimeObj runtimeObj = kvp.Key;
            GameObject visualObj = kvp.Value;

            if (visualObj != null && runtimeObj.Transform != null)
            {
                UpdateGameObjectTransform(visualObj, runtimeObj.Transform);
            }
        }
    }

    /// <summary>
    /// Clear all rendered GameObjects
    /// </summary>
    public void ClearRendering()
    {
        if (renderRoot != null)
        {
            if (Application.isPlaying)
            {
                Destroy(renderRoot);
            }
            else
            {
                DestroyImmediate(renderRoot);
            }
            renderRoot = null;
        }

        runtimeObjToGameObject.Clear();
        gameTile = null;
    }

    private void OnDestroy()
    {
        ClearRendering();
    }

    private void OnDrawGizmos()
    {
        if (!ShowDebugInfo || gameTile == null || gameTile.Sim == null)
            return;

        // Draw physics bodies for debugging
        Gizmos.color = Color.green;

        foreach (var body in gameTile.Sim.Bodies)
        {
            Vector3 pos = new Vector3(
                body.Position.X.ToFloat(),
                body.Position.Y.ToFloat(),
                0f
            );

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
