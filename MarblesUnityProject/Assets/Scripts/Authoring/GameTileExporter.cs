using System.Collections.Generic;
using System.IO;
using System.Linq;
using FPMathLib;
using GameCoreLib;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class LevelFileExporter : EditorWindow
{
    private GameCoreRenderer cachedRuntimeRenderer;
    private List<GameObject> cachedRenderPrefabs;

    [MenuItem("Window/LockSim/LevelFileExporter")]
    public static void ShowWindow()
    {
        GetWindow<LevelFileExporter>("Level File Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Export Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Levels to JSON Files"))
        {
            ExportLevelsToJSON();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Upload Level Files to SpacetimeDB"))
        {
            UploadLevelsToSpacetimeDB();
        }
    }

    // Step 1 & 2: Export prefabs to JSON
    private void ExportLevelsToJSON()
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

        // Create output directory if it doesn't exist
        string outputDir = "Temp/LevelJSONs";
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Find all prefabs in Assets/Prefabs folder
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        int exportedCount = 0;

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
                string json = JsonConvert.SerializeObject(runtimeObj, Formatting.Indented); //JsonUtility.ToJson(runtimeObj, true);

                // Save to file
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                string outputPath = Path.Combine(outputDir, fileName + ".json");
                File.WriteAllText(outputPath, json);

                Debug.Log($"Exported: {fileName} -> {outputPath}");
                exportedCount++;
            }
        }

        Debug.Log($"Export complete! {exportedCount} level(s) exported to {outputDir}");
        EditorUtility.RevealInFinder(outputDir);
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

    // Step 3-5: Upload to SpacetimeDB (placeholder for now)
    private void UploadLevelsToSpacetimeDB()
    {
        Debug.Log("Upload to SpacetimeDB - To be implemented");
        // TODO: Upload the level files to spacetimedb via http api. I think we can do a direct insert.
    }
}
