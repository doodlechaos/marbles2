using System;
using System.Collections.Generic;
using System.Reflection;
using GameCoreLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom inspector for GCObjBinding that shows the attached GameCoreObj
/// and all of its GCComponents in a readable, debug‑friendly way.
///
/// This does NOT rely on Unity's built‑in serialization of GameCoreObj / GCComponent,
/// so it works even though those types are polymorphic MemoryPack unions that Unity
/// doesn't understand. We just inspect the live C# objects at runtime.
/// </summary>
[CustomEditor(typeof(GCObjBinding))]
public sealed class GameCoreObjBindingEditor : Editor
{
    private bool _showComponents = true;
    private bool _showTransformDetails = true;

    public override void OnInspectorGUI()
    {
        var binding = (GCObjBinding)target;
        var gameCoreObj = binding.GameCoreObj;

        EditorGUILayout.LabelField("GameCore Object Binding", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Enter Play mode to inspect the live GameCoreObj and its components.",
                MessageType.Info
            );
            return;
        }

        if (gameCoreObj == null)
        {
            EditorGUILayout.HelpBox(
                "GameCoreObj is null. This GameObject is not currently bound to a GameCore object.",
                MessageType.Warning
            );
            return;
        }

        DrawGameCoreObjHeader(gameCoreObj);

        EditorGUILayout.Space();
        _showComponents = EditorGUILayout.Foldout(
            _showComponents,
            $"Components ({gameCoreObj.GameComponents?.Count ?? 0})"
        );
        if (_showComponents)
        {
            EditorGUI.indentLevel++;
            DrawGameCoreObjComponents(gameCoreObj);
            EditorGUI.indentLevel--;
        }
    }

    private void DrawGameCoreObjHeader(GameCoreObj obj)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Name", obj.Name ?? "<null>");
            EditorGUILayout.LabelField("RuntimeId", obj.RuntimeId.ToString());

            string prefabInfo =
                obj.RenderPrefabID >= 0
                    ? $"{obj.RenderPrefabID} (Prefab Root)"
                    : "-1 (Not a prefab)";
            EditorGUILayout.LabelField("Prefab ID", prefabInfo);
            EditorGUILayout.LabelField("Sibling Index", obj.SiblingIndex.ToString());
            EditorGUILayout.LabelField("Active", obj.Active.ToString());

            // Physics info
            if (obj.HasPhysicsBody)
            {
                EditorGUILayout.LabelField("Physics Body ID", obj.PhysicsBodyId.ToString());
            }
        }

        EditorGUILayout.Space();
        _showTransformDetails = EditorGUILayout.Foldout(_showTransformDetails, "Transform");
        if (_showTransformDetails)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                var t = obj.Transform;

                // Local transform
                EditorGUILayout.LabelField(
                    "Local Position",
                    $"({t.LocalPosition.X:F3}, {t.LocalPosition.Y:F3}, {t.LocalPosition.Z:F3})"
                );
                EditorGUILayout.LabelField(
                    "Local Rotation (Euler)",
                    $"({t.LocalRotation.EulerAngles.X:F1}°, {t.LocalRotation.EulerAngles.Y:F1}°, {t.LocalRotation.EulerAngles.Z:F1}°)"
                );
                EditorGUILayout.LabelField(
                    "Local Scale",
                    $"({t.LocalScale.X:F3}, {t.LocalScale.Y:F3}, {t.LocalScale.Z:F3})"
                );

                // World transform (if available)
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(
                    "World Position",
                    $"({t.Position.X:F3}, {t.Position.Y:F3}, {t.Position.Z:F3})"
                );
            }
        }
    }

    private static void DrawGameCoreObjComponents(GameCoreObj obj)
    {
        var components = obj.GameComponents;

        if (components == null || components.Count == 0)
        {
            EditorGUILayout.LabelField("No GameCoreObjComponents on this GameCoreObj.");
            return;
        }

        EditorGUILayout.LabelField($"Count: {components.Count}");

        foreach (var comp in components)
        {
            if (comp == null)
                continue;

            using (new EditorGUILayout.VerticalScope("box"))
            {
                var type = comp.GetType();
                EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);

                // Common base fields
                EditorGUILayout.Toggle("Enabled", comp.Enabled);

                // Dump public instance fields for quick debugging
                DrawComponentFields(comp, type);
            }
        }
    }

    private static void DrawComponentFields(GCComponent component, Type type)
    {
        // We only care about the concrete type's own public fields (and base data fields),
        // not the GameCoreObj back‑reference etc.
        const BindingFlags flags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        // Walk up the inheritance chain until GCComponent
        var types = new List<Type>();
        var current = type;
        while (current != null && current != typeof(GCComponent) && current != typeof(object))
        {
            types.Add(current);
            current = current.BaseType;
        }

        // Include fields declared directly on GCComponent, but skip GameCoreObj reference itself
        types.Add(typeof(GCComponent));

        foreach (var t in types)
        {
            var fields = t.GetFields(flags);
            foreach (var field in fields)
            {
                // Skip back‑references and anything Unity shouldn't poke at
                if (field.Name == nameof(GCComponent.GCObj))
                    continue;

                var value = field.GetValue(component);
                DrawFieldValue(field.Name, value);
            }
        }
    }

    private static void DrawFieldValue(string label, object value)
    {
        if (value is null)
        {
            EditorGUILayout.LabelField(label, "<null>");
            return;
        }

        switch (value)
        {
            case bool b:
                EditorGUILayout.Toggle(label, b);
                break;
            case int i:
                EditorGUILayout.IntField(label, i);
                break;
            case uint ui:
                EditorGUILayout.LongField(label, ui);
                break;
            case long l:
                EditorGUILayout.LongField(label, l);
                break;
            case ulong ul:
                // Unity has no ulong field, so just show as string
                EditorGUILayout.LabelField(label, ul.ToString());
                break;
            case float f:
                EditorGUILayout.FloatField(label, f);
                break;
            case string s:
                EditorGUILayout.LabelField(label, s);
                break;
            default:
                // FPMath types and anything else: just ToString them
                EditorGUILayout.LabelField(label, value.ToString());
                break;
        }
    }
}
