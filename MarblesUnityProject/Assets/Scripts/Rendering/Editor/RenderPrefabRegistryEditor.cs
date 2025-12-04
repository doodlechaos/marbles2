using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for RenderPrefabRegistry that adds a button to auto-assign IDs to prefabs.
/// </summary>
[CustomEditor(typeof(RenderPrefabRegistry))]
public class RenderPrefabRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RenderPrefabRegistry registry = (RenderPrefabRegistry)target;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Click the button below to automatically add/update RenderPrefabIdentifier components on all prefabs in the registry.",
            MessageType.Info
        );

        if (GUILayout.Button("Auto-Assign IDs to Prefabs", GUILayout.Height(30)))
        {
            AutoAssignPrefabIDs(registry);
        }
    }

    private void AutoAssignPrefabIDs(RenderPrefabRegistry registry)
    {
        int updatedCount = 0;
        int addedCount = 0;

        for (int i = 0; i < registry.Prefabs.Count; i++)
        {
            GameObject prefab = registry.Prefabs[i];
            if (prefab == null)
            {
                Debug.LogWarning($"[RenderPrefabRegistry] Prefab at index {i} is null, skipping");
                continue;
            }

            // Get or add the identifier component
            RenderPrefabIdentifier identifier = prefab.GetComponent<RenderPrefabIdentifier>();
            if (identifier == null)
            {
                identifier = prefab.AddComponent<RenderPrefabIdentifier>();
                addedCount++;
                Debug.Log($"[RenderPrefabRegistry] Added RenderPrefabIdentifier to {prefab.name}");
            }

            // Set the ID
            if (identifier.RenderPrefabID != i)
            {
                identifier.SetRenderPrefabID(i);
                EditorUtility.SetDirty(prefab);
                updatedCount++;
                Debug.Log(
                    $"[RenderPrefabRegistry] Updated {prefab.name} ID to {i} (was {identifier.RenderPrefabID})"
                );
            }
        }

        if (addedCount > 0 || updatedCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log(
                $"[RenderPrefabRegistry] Auto-assign complete! Added {addedCount} components, updated {updatedCount} IDs"
            );
        }
        else
        {
            Debug.Log("[RenderPrefabRegistry] All prefabs already have correct IDs!");
        }
    }
}
