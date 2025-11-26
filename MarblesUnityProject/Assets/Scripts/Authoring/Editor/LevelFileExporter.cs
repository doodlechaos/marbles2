using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPMathLib;
using GameCoreLib;
using MemoryPack;
using Newtonsoft.Json;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class LevelFileExporter : EditorWindow
{
    private GameCoreRenderer cachedRuntimeRenderer;
    private List<GameObject> cachedRenderPrefabs;

    private const string SERVER_URL = "http://127.0.0.1:3000";
    private const string MODULE_NAME = "marbles2";

    [MenuItem("Window/LockSim/LevelFileExporter")]
    public static void ShowWindow()
    {
        GetWindow<LevelFileExporter>("Level File Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Export Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Export and Upload Levels to SpacetimeDB"))
        {
            // Fire and forget pattern for async method in editor
            ExportAndUploadLevelsToSpacetimeDBAsync();
        }
    }

    // Export and upload level files directly to SpacetimeDB
    private async void ExportAndUploadLevelsToSpacetimeDBAsync()
    {
        DbConnection adminConn = null;

        try
        {
            // Find RuntimeRenderer in scene to get the render prefabs list
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

            // Create a temporary admin connection
            Debug.Log("Creating temporary admin connection...");
            adminConn = await STDB.GetTempAdminConnection();
            Debug.Log("Admin connection established!");

            // Find all prefabs in Assets/Prefabs folder
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

                // Check if the root has LevelFileAuth component
                LevelFileAuth levelFileAuth = prefab.GetComponent<LevelFileAuth>();
                if (levelFileAuth != null)
                {
                    // Serialize the entire prefab hierarchy to RuntimeObj
                    RuntimeObj runtimeObj = SerializeGameObject(prefab, null);
                    string objHierarchyJson = JsonConvert.SerializeObject(runtimeObj);

                    // Create LevelFile object
                    string levelName = Path.GetFileNameWithoutExtension(assetPath);
                    LevelFile levelFile = new LevelFile(
                        guid,
                        new LevelMetadata
                        {
                            LevelName = levelName,
                            Rarity = levelFileAuth.Rarity,
                            MinAuctionSpots = levelFileAuth.MinAuctionSpots,
                            MaxAuctionSpots = levelFileAuth.MaxAuctionSpots,
                            MaxRaffleDraws = levelFileAuth.MaxRaffleDraws,
                        },
                        objHierarchyJson
                    );

                    // Serialize to binary
                    byte[] levelFileBinary = levelFile.ToBinary();

                    // Upload to SpacetimeDB using the reducer
                    UploadLevelFileToSpacetimeDB(adminConn, guid, levelFileBinary);

                    Debug.Log($"✓ Uploaded: {levelName} (GUID: {guid})");
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
            // Always disconnect the temporary connection
            if (adminConn != null)
            {
                Debug.Log("Disconnecting temporary admin connection...");
                adminConn.Disconnect();
            }
        }
    }

    // Recursively serialize a GameObject and all its children into RuntimeObj
    private RuntimeObj SerializeGameObject(GameObject go, RuntimeObj parent)
    {
        Debug.Log($"Serializing GameObject: {go.name}. Children: {go.transform.childCount}");
        RuntimeObj runtimeObj = new RuntimeObj
        {
            Name = go.name,
            Children = new List<RuntimeObj>(),
            Transform = ConvertToFPTransform(go.transform),
            Components = SerializeComponents(go),
            RenderPrefabID = GetRenderPrefabID(go),
        };

        // Recursively serialize all children
        foreach (Transform child in go.transform)
        {
            RuntimeObj childObj = SerializeGameObject(child.gameObject, runtimeObj);
            runtimeObj.Children.Add(childObj);
        }

        return runtimeObj;
    }

    /// <summary>
    /// Find and cache the RuntimeRenderer's render prefabs list from the scene
    /// </summary>
    private bool CacheRuntimeRendererPrefabs()
    {
        // Find RuntimeRenderer in the scene
        cachedRuntimeRenderer = FindFirstObjectByType<GameCoreRenderer>();

        if (cachedRuntimeRenderer == null)
        {
            return false;
        }

        // Use reflection to access the private renderPrefabs field
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
    /// Get the RenderPrefabID for a GameObject by checking if it matches any prefab in the RuntimeRenderer's list
    /// </summary>
    private int GetRenderPrefabID(GameObject go)
    {
        if (cachedRenderPrefabs == null || cachedRenderPrefabs.Count == 0)
        {
            return -1;
        }

        // Get the source prefab of this GameObject (if it's a prefab instance)
        GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);

        // If no source prefab, check if the GameObject itself is a prefab asset
        if (sourcePrefab == null)
        {
            sourcePrefab = PrefabUtility.IsPartOfPrefabAsset(go) ? go : null;
        }

        // If we found a source prefab, check if it matches any in our render prefabs list
        if (sourcePrefab != null)
        {
            for (int i = 0; i < cachedRenderPrefabs.Count; i++)
            {
                if (
                    cachedRenderPrefabs[i] != null
                    && (
                        cachedRenderPrefabs[i] == sourcePrefab
                        || PrefabUtility.GetCorrespondingObjectFromSource(cachedRenderPrefabs[i])
                            == sourcePrefab
                    )
                )
                {
                    // Return 0-based index
                    return i;
                }
            }
        }
        else
        {
            Debug.LogError($"No source prefab found for {go.name}");
        }

        // Default to -1 (no prefab / empty GameObject)
        return -1;
    }

    private FPTransform3D ConvertToFPTransform(Transform t)
    {
        // Convert Unity Transform to fixed-point FPTransform3D
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

    private List<GameCoreLib.ComponentData> SerializeComponents(GameObject go)
    {
        List<GameCoreLib.ComponentData> components = new List<GameCoreLib.ComponentData>();
        Component[] allComponents = go.GetComponents<Component>();

        foreach (Component component in allComponents)
        {
            // Skip Transform component as we handle it separately
            if (component is Transform)
                continue;

            GameCoreLib.ComponentData compData = new GameCoreLib.ComponentData
            {
                type = component.GetType().FullName,
                enabled = true,
                data = SerializeComponentFields(component),
            };

            // Check if component has enabled property
            var enabledProp = component.GetType().GetProperty("enabled");
            if (enabledProp != null)
            {
                compData.enabled = (bool)enabledProp.GetValue(component);
            }

            components.Add(compData);
        }

        return components;
    }

    private string SerializeComponentFields(Component component)
    {
        // Use Unity's JsonUtility to serialize the component data
        // This will capture all serializable fields
        try
        {
            return JsonUtility.ToJson(component, false);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(
                $"Failed to serialize component {component.GetType().Name}: {e.Message}"
            );
            return "{}";
        }
    }

    /// <summary>
    /// Upload a single level file to SpacetimeDB using the module bindings
    /// Calls the UpsertLevelFileData reducer to add/replace the level data
    /// </summary>
    private void UploadLevelFileToSpacetimeDB(
        DbConnection conn,
        string unityPrefabGUID,
        byte[] levelFileBinary
    )
    {
        // Create the LevelFileData struct
        var levelFileData = new LevelFileData
        {
            UnityPrefabGuid = unityPrefabGUID,
            LevelFileBinary = levelFileBinary.ToList(),
        };

        // Call the reducer through the generated bindings
        conn.Reducers.UpsertLevelFileData(levelFileData);
    }
}
