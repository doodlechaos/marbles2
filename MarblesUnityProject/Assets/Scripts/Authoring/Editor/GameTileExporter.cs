using System;
using System.Collections.Generic;
using System.IO;
using FPMathLib;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;

public class GameTileExporter : EditorWindow
{
    private GameCoreRenderer cachedRuntimeRenderer;
    private List<GameObject> cachedRenderPrefabs;

    private const string PREF_SERVER_URL = "GameTileExporter_ServerUrl";
    private const string PREF_MODULE_NAME = "GameTileExporter_ModuleName";

    private const string DEFAULT_SERVER_URL = "http://127.0.0.1:3000";
    private const string DEFAULT_MODULE_NAME = "marbles2";

    private string serverUrl;
    private string moduleName;

    [MenuItem("Window/GameCore/GameTileExporter")]
    public static void ShowWindow()
    {
        GetWindow<GameTileExporter>("Game Tile Exporter");
    }

    void OnEnable()
    {
        serverUrl = EditorPrefs.GetString(PREF_SERVER_URL, DEFAULT_SERVER_URL);
        moduleName = EditorPrefs.GetString(PREF_MODULE_NAME, DEFAULT_MODULE_NAME);
    }

    void OnGUI()
    {
        GUILayout.Label("Game Tile Export Tools", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        serverUrl = EditorGUILayout.TextField("Server URL", serverUrl);
        moduleName = EditorGUILayout.TextField("Module Name", moduleName);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(PREF_SERVER_URL, serverUrl);
            EditorPrefs.SetString(PREF_MODULE_NAME, moduleName);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Export and Upload Levels to SpacetimeDB"))
        {
            ExportAndUploadLevelsToSpacetimeDBAsync();
        }
    }

    private async void ExportAndUploadLevelsToSpacetimeDBAsync()
    {
        DbConnection adminConn = null;

        try
        {
            if (!CacheRuntimeRendererPrefabs())
            {
                Debug.LogError(
                    "Could not find RuntimeRenderer in scene. Please make sure there's a RuntimeRenderer component in the scene."
                );
                return;
            }

            Debug.Log(
                $"Found RuntimeRenderer with {cachedRenderPrefabs.Count} render prefabs configured."
            );

            Debug.Log("Creating temporary admin connection...");
            adminConn = await STDB.GetTempAdminConnection();
            Debug.Log("Admin connection established!");

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
            int uploadedCount = 0;

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab == null)
                {
                    Debug.LogWarning($"Failed to load prefab at {assetPath}");
                    continue;
                }

                GameTileAuthBase gameTileAuth = prefab.GetComponent<GameTileAuthBase>();
                if (gameTileAuth != null)
                {
                    // Create the appropriate GameTileBase type based on the auth component
                    GameTileBase gameTile = CreateGameTileFromAuth(gameTileAuth);
                    if (gameTile == null)
                    {
                        Debug.LogWarning(
                            $"Unknown game tile auth type: {gameTileAuth.GetType().Name}"
                        );
                        continue;
                    }

                    // Serialize the GameObject hierarchy into a RuntimeObj and set as TileRoot
                    gameTile.TileRoot = SerializeGameObject(prefab, null);

                    // Serialize the entire GameTileBase with MemoryPack
                    byte[] gameTileBinary = MemoryPackSerializer.Serialize<GameTileBase>(gameTile);

                    string tileName = Path.GetFileNameWithoutExtension(assetPath);

                    // Upload to SpacetimeDB
                    UploadGameTileToSpacetimeDB(
                        adminConn,
                        guid,
                        tileName,
                        gameTileAuth,
                        gameTileBinary
                    );

                    Debug.Log(
                        $"✓ Uploaded: {tileName} (GUID: {guid}, Size: {gameTileBinary.Length} bytes)"
                    );
                    uploadedCount++;
                }
            }

            Debug.Log($"Upload complete! {uploadedCount} level(s) uploaded successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during export/upload: {ex}");
        }
        finally
        {
            if (adminConn != null)
            {
                Debug.Log("Disconnecting temporary admin connection...");
                adminConn.Disconnect();
            }
        }
    }

    /// <summary>
    /// Create the appropriate GameTileBase subtype based on the auth component type.
    /// </summary>
    private GameTileBase CreateGameTileFromAuth(GameTileAuthBase gameTileAuth)
    {
        // Map auth types to GameTile types
        return gameTileAuth switch
        {
            SimpleBattleRoyaleAuth => new SimpleBattleRoyale(),
            // Add other game modes here as they're created:
            // SurvivalAuth => new Survival(),
            // RaceAuth => new Race(),
            _ => null,
        };
    }

    /// <summary>
    /// Recursively serialize a GameObject and all its children into RuntimeObj
    /// </summary>
    private RuntimeObj SerializeGameObject(GameObject go, RuntimeObj parent)
    {
        Debug.Log($"Serializing GameObject: {go.name}. Children: {go.transform.childCount}");

        RuntimeObj runtimeObj = new RuntimeObj
        {
            Name = go.name,
            Children = new List<RuntimeObj>(),
            Transform = ConvertToFPTransform(go.transform),
            GameComponents = SerializeGameComponents(go),
            RenderPrefabID = GetRenderPrefabID(go),
        };

        // Add LevelRootComponent for objects with LevelFileAuth (marks them as level roots)
        GameTileAuthBase gameTileAuth = go.GetComponent<GameTileAuthBase>();
        if (gameTileAuth != null)
        {
            runtimeObj.GameComponents.Add(
                new LevelRootComponent
                {
                    GameModeType = gameTileAuth.GetType().Name.Replace("Auth", ""),
                }
            );
            Debug.Log(
                $"  Added LevelRootComponent with GameModeType: {gameTileAuth.GetType().Name.Replace("Auth", "")}"
            );
        }

        // Recursively serialize all children
        foreach (Transform child in go.transform)
        {
            RuntimeObj childObj = SerializeGameObject(child.gameObject, runtimeObj);
            runtimeObj.Children.Add(childObj);
        }

        return runtimeObj;
    }

    /// <summary>
    /// Serialize all GameComponentAuth components on a GameObject to GameComponents.
    /// Also auto-exports Unity physics components if no explicit auth component exists.
    /// </summary>
    private List<GameComponent> SerializeGameComponents(GameObject go)
    {
        List<GameComponent> components = new List<GameComponent>();

        // First, export all explicit GameComponentAuth components
        GameComponentAuth[] authComponents = go.GetComponents<GameComponentAuth>();
        foreach (var auth in authComponents)
        {
            try
            {
                GameComponent gameComponent = auth.ToGameComponent();
                if (gameComponent != null)
                {
                    components.Add(gameComponent);
                    Debug.Log(
                        $"  Exported {auth.GetType().Name} -> {gameComponent.GetType().Name}"
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    $"Failed to export {auth.GetType().Name} on {go.name}: {e.Message}"
                );
            }
        }

        // Auto-export Unity physics components if no explicit auth exists
        AutoExportUnityPhysicsComponents(go, components);

        return components;
    }

    /// <summary>
    /// Automatically export Unity physics components that don't have explicit auth components.
    /// </summary>
    private void AutoExportUnityPhysicsComponents(GameObject go, List<GameComponent> components)
    {
        // Check if we already have these component types from auth
        bool hasBoxCollider = false;
        bool hasCircleCollider = false;
        bool hasRigidbody = false;

        foreach (var comp in components)
        {
            if (comp is BoxCollider2DComponent)
                hasBoxCollider = true;
            if (comp is CircleCollider2DComponent)
                hasCircleCollider = true;
            if (comp is Rigidbody2DComponent)
                hasRigidbody = true;
        }

        // Auto-export BoxCollider2D
        if (!hasBoxCollider)
        {
            var boxCollider = go.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                components.Add(
                    new BoxCollider2DComponent
                    {
                        Enabled = boxCollider.enabled,
                        Size = FPVector2.FromFloats(boxCollider.size.x, boxCollider.size.y),
                        Offset = FPVector2.FromFloats(boxCollider.offset.x, boxCollider.offset.y),
                        IsTrigger = boxCollider.isTrigger,
                    }
                );
                Debug.Log($"  Auto-exported BoxCollider2D on {go.name}");
            }
        }

        // Auto-export CircleCollider2D
        if (!hasCircleCollider)
        {
            var circleCollider = go.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                components.Add(
                    new CircleCollider2DComponent
                    {
                        Enabled = circleCollider.enabled,
                        Radius = FP.FromFloat(circleCollider.radius),
                        Offset = FPVector2.FromFloats(
                            circleCollider.offset.x,
                            circleCollider.offset.y
                        ),
                        IsTrigger = circleCollider.isTrigger,
                    }
                );
                Debug.Log($"  Auto-exported CircleCollider2D on {go.name}");
            }
        }

        // Auto-export Rigidbody2D
        if (!hasRigidbody)
        {
            var rigidbody = go.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                components.Add(
                    new Rigidbody2DComponent
                    {
                        Enabled = rigidbody.simulated,
                        BodyType = ConvertBodyType(rigidbody.bodyType),
                        Mass = FP.FromFloat(rigidbody.mass),
                        LinearDrag = FP.FromFloat(rigidbody.linearDamping),
                        AngularDrag = FP.FromFloat(rigidbody.angularDamping),
                        GravityScale = FP.FromFloat(rigidbody.gravityScale),
                        FreezeRotation = rigidbody.freezeRotation,
                    }
                );
                Debug.Log($"  Auto-exported Rigidbody2D on {go.name}");
            }
        }
    }

    private Rigidbody2DType ConvertBodyType(RigidbodyType2D unityType)
    {
        return unityType switch
        {
            RigidbodyType2D.Dynamic => Rigidbody2DType.Dynamic,
            RigidbodyType2D.Kinematic => Rigidbody2DType.Kinematic,
            RigidbodyType2D.Static => Rigidbody2DType.Static,
            _ => Rigidbody2DType.Dynamic,
        };
    }

    private bool CacheRuntimeRendererPrefabs()
    {
        cachedRuntimeRenderer = FindFirstObjectByType<GameCoreRenderer>();

        if (cachedRuntimeRenderer == null)
        {
            return false;
        }

        var renderPrefabsField = typeof(GameCoreRenderer).GetField(
            "renderPrefabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (renderPrefabsField != null)
        {
            cachedRenderPrefabs =
                renderPrefabsField.GetValue(cachedRuntimeRenderer) as List<GameObject>;
            return cachedRenderPrefabs != null;
        }

        return false;
    }

    /// <summary>
    /// Get the RenderPrefabID for a GameObject, but ONLY if it's a prefab root.
    /// This ensures we can distinguish between "this is a prefab I should instantiate" vs
    /// "this is a child that's part of another prefab's definition."
    /// </summary>
    private int GetRenderPrefabID(GameObject go)
    {
        if (cachedRenderPrefabs == null || cachedRenderPrefabs.Count == 0)
        {
            return -1;
        }

        // NEW POLICY: Only assign RenderPrefabID to prefab instance roots.
        // Children within prefabs should NOT get a RenderPrefabID, even if they
        // happen to match a render prefab's path. This allows the renderer to
        // instantiate prefabs only at the appropriate roots and find/link children
        // from the instantiated hierarchy.
        if (!IsPrefabInstanceRoot(go))
        {
            return -1;
        }

        // Get the asset path of the prefab this object corresponds to
        string goAssetPath = GetPrefabAssetPath(go);

        if (string.IsNullOrEmpty(goAssetPath))
        {
            // This object isn't from a prefab - that's okay, return -1
            return -1;
        }

        // Compare by asset path - more reliable than object reference comparison
        for (int i = 0; i < cachedRenderPrefabs.Count; i++)
        {
            if (cachedRenderPrefabs[i] != null)
            {
                string renderPrefabPath = AssetDatabase.GetAssetPath(cachedRenderPrefabs[i]);
                if (!string.IsNullOrEmpty(renderPrefabPath) && renderPrefabPath == goAssetPath)
                {
                    Debug.Log(
                        $"  Prefab root '{go.name}' matched render prefab [{i}]: {renderPrefabPath}"
                    );
                    return i;
                }
            }
        }

        return -1;
    }

    /// <summary>
    /// Check if a GameObject is the root of a prefab instance (not just a child within a prefab).
    /// This is used to determine if RenderPrefabID should be assigned.
    /// </summary>
    private bool IsPrefabInstanceRoot(GameObject go)
    {
        // Check if this is the root of a prefab instance (including nested prefabs)
        return PrefabUtility.IsAnyPrefabInstanceRoot(go);
    }

    /// <summary>
    /// Get the asset path of the prefab that a GameObject corresponds to.
    /// Handles nested prefabs within prefab assets correctly.
    /// </summary>
    private string GetPrefabAssetPath(GameObject go)
    {
        // For nested prefabs within prefab assets, GetCorrespondingObjectFromOriginalSource
        // properly traverses the nested prefab chain to find the original prefab asset
        GameObject originalSource = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);
        if (originalSource != null)
        {
            // Make sure we're getting the root of the prefab, not a child object
            GameObject prefabRoot = originalSource.transform.root.gameObject;
            return AssetDatabase.GetAssetPath(prefabRoot);
        }

        // Fallback: try GetCorrespondingObjectFromSource for scene instances
        GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(go);
        if (source != null)
        {
            GameObject prefabRoot = source.transform.root.gameObject;
            return AssetDatabase.GetAssetPath(prefabRoot);
        }

        // If the object is directly part of a prefab asset (not nested), get its path
        if (PrefabUtility.IsPartOfPrefabAsset(go))
        {
            GameObject prefabRoot = go.transform.root.gameObject;
            return AssetDatabase.GetAssetPath(prefabRoot);
        }

        return null;
    }

    private FPTransform3D ConvertToFPTransform(Transform t)
    {
        FPVector3 localPos = FPVector3.FromFloats(
            t.localPosition.x,
            t.localPosition.y,
            t.localPosition.z
        );

        FPQuaternion localRot = FPQuaternion.FromFloats(
            t.localRotation.x,
            t.localRotation.y,
            t.localRotation.z,
            t.localRotation.w
        );

        FPVector3 localScale = FPVector3.FromFloats(t.localScale.x, t.localScale.y, t.localScale.z);

        return new FPTransform3D(localPos, localRot, localScale);
    }

    private void UploadGameTileToSpacetimeDB(
        DbConnection conn,
        string unityPrefabGUID,
        string tileName,
        GameTileAuthBase gameTileAuth,
        byte[] gameTileBinary
    )
    {
        var gameTileData = new GameTile
        {
            UnityPrefabGuid = unityPrefabGUID,
            TileName = tileName,
            Rarity = (int)gameTileAuth.AppearanceFrequency,
            MinAuctionSpots = gameTileAuth.MinAuctionSpots,
            MaxAuctionSpots = gameTileAuth.MaxAuctionSpots,
            MaxRaffleDraws = gameTileAuth.MaxRaffleDraws,
            GameTileBinary = new List<byte>(gameTileBinary),
        };

        conn.Reducers.UpsertGameTile(gameTileData);
    }
}
