using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using GameCoreLib;
using FPMathLib;
using Newtonsoft.Json;

public class GameTileExporter : EditorWindow
{
    [MenuItem("Window/LockSim/GameTileExporter")]
    public static void ShowWindow()
    {
        GetWindow<GameTileExporter>("Game Tile Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Export Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Levels to JSON Files"))
        {
            ExportLevelsToJSON();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Upload Levels to SpacetimeDB"))
        {
            UploadLevelsToSpacetimeDB();
        }
    }

    // Step 1 & 2: Export prefabs to JSON
    private void ExportLevelsToJSON()
    {
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

            // Check if the root has GameTileAuth component
            GameTileAuth gameTileAuth = prefab.GetComponent<GameTileAuth>();
            if (gameTileAuth != null)
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
        RuntimeObj runtimeObj = new RuntimeObj
        {
            Name = go.name,
            Parent = null, // Don't serialize parent reference to avoid circular dependencies
            Children = new List<RuntimeObj>(),
            Transform = ConvertToFPTransform(go.transform),
            Components = SerializeComponents(go)
        };

        // Recursively serialize all children
        foreach (Transform child in go.transform)
        {
            RuntimeObj childObj = SerializeGameObject(child.gameObject, runtimeObj);
            runtimeObj.Children.Add(childObj);
        }

        return runtimeObj;
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

        FPVector3 localScale = FPVector3.FromFloats(
            t.localScale.x,
            t.localScale.y,
            t.localScale.z
        );

        return new FPTransform3D(localPos, localRot, localScale);
    }

    private List<GameCoreLib.ComponentData> SerializeComponents(GameObject go)
    {
        List<GameCoreLib.ComponentData> components = new List<GameCoreLib.ComponentData>();
        Component[] allComponents = go.GetComponents<Component>();

        foreach (Component component in allComponents)
        {
            // Skip Transform component as we handle it separately
            if (component is Transform) continue;

            GameCoreLib.ComponentData compData = new GameCoreLib.ComponentData
            {
                type = component.GetType().FullName,
                enabled = true,
                data = SerializeComponentFields(component)
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
            Debug.LogWarning($"Failed to serialize component {component.GetType().Name}: {e.Message}");
            return "{}";
        }
    }

    // Step 3-5: Upload to SpacetimeDB (placeholder for now)
    private void UploadLevelsToSpacetimeDB()
    {
        Debug.Log("Upload to SpacetimeDB - To be implemented");
        // TODO: Implement steps 3-5
    }
}
