using System;
using System.Collections.Generic;
using System.Reflection;
using GameCoreLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom inspector for RuntimeBinding that shows the attached GameCoreObj
/// and all of its GameCoreObjComponents in a readable, debug‑friendly way.
///
/// This does NOT rely on Unity's built‑in serialization of GameCoreObj / GameCoreObjComponent,
/// so it works even though those types are polymorphic MemoryPack unions that Unity
/// doesn't understand. We just inspect the live C# objects at runtime.
/// </summary>
[CustomEditor(typeof(GCObjBinding))]
public sealed class GameCoreObjBindingEditor : Editor
{
    private bool _showComponents = true;

    public override void OnInspectorGUI()
    {
        // Draw the normal inspector first (so you still see the GameCoreObj field, etc.)
        DrawDefaultInspector();

        var binding = (GCObjBinding)target;
        var gameCoreObj = binding.GameCoreObj;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("GameCore Debug View", EditorStyles.boldLabel);

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
                "GameCoreObj is null on this binding at runtime.\n"
                    + "Ensure your renderer / game code assigns a GameCoreObj to RuntimeBinding.GameCoreObj.",
                MessageType.Warning
            );
            return;
        }

        DrawGameCoreObjHeader(gameCoreObj);

        _showComponents = EditorGUILayout.Foldout(_showComponents, "GameCoreObjComponents");
        if (_showComponents)
        {
            DrawGameCoreObjComponents(gameCoreObj);
        }
    }

    private static void DrawGameCoreObjHeader(GameCoreObj obj)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("GameCoreObj", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Name", obj.Name ?? "<null>");
            EditorGUILayout.LabelField("RuntimeId", obj.RuntimeId.ToString());
            EditorGUILayout.LabelField("Prefab ID", obj.RenderPrefabID.ToString());

            // Simple transform debug info
            var t = obj.Transform;
            EditorGUILayout.LabelField(
                "LocalPosition",
                $"({t.LocalPosition.X}, {t.LocalPosition.Y}, {t.LocalPosition.Z})"
            );
            EditorGUILayout.LabelField(
                "LocalRotation (Euler XYZ)",
                $"({t.LocalRotation.EulerAngles.X}, {t.LocalRotation.EulerAngles.Y}, {t.LocalRotation.EulerAngles.Z})"
            );
            EditorGUILayout.LabelField(
                "LocalScale",
                $"({t.LocalScale.X}, {t.LocalScale.Y}, {t.LocalScale.Z})"
            );
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
