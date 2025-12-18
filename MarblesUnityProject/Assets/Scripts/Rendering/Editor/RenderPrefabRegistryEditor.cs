using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for RenderPrefabRegistry that auto-assigns IDs to prefabs.
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
            "Click below to automatically update PrefabId on all RenderPrefabRoot components.",
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

        for (int i = 0; i < registry.Prefabs.Count; i++)
        {
            RenderPrefabRoot prefabRoot = registry.Prefabs[i];
            if (prefabRoot == null)
            {
                Debug.LogWarning($"[RenderPrefabRegistry] Prefab at index {i} is null, skipping");
                continue;
            }

            if (prefabRoot.PrefabId != i)
            {
                prefabRoot.SetPrefabId(i);
                EditorUtility.SetDirty(prefabRoot);
                updatedCount++;
                Debug.Log($"[RenderPrefabRegistry] Updated {prefabRoot.name} PrefabId to {i}");
            }
        }

        if (updatedCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log(
                $"[RenderPrefabRegistry] Auto-assign complete! Updated {updatedCount} prefabs"
            );
        }
        else
        {
            Debug.Log("[RenderPrefabRegistry] All prefabs already have correct IDs!");
        }
    }
}
