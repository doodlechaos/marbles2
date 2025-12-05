using System;
using System.Collections.Generic;
using System.Reflection;
using GameCoreLib;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom inspector for RuntimeBinding that shows the attached RuntimeObj
/// and all of its RuntimeObjComponents in a readable, debug‑friendly way.
///
/// This does NOT rely on Unity's built‑in serialization of RuntimeObj / RuntimeObjComponent,
/// so it works even though those types are polymorphic MemoryPack unions that Unity
/// doesn't understand. We just inspect the live C# objects at runtime.
/// </summary>
[CustomEditor(typeof(RuntimeBinding))]
public sealed class RuntimeBindingEditor : Editor
{
    private bool _showComponents = true;

    public override void OnInspectorGUI()
    {
        // Draw the normal inspector first (so you still see the RuntimeObj field, etc.)
        DrawDefaultInspector();

        var binding = (RuntimeBinding)target;
        var runtimeObj = binding.RuntimeObj;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("GameCore Debug View", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Enter Play mode to inspect the live RuntimeObj and its components.",
                MessageType.Info
            );
            return;
        }

        if (runtimeObj == null)
        {
            EditorGUILayout.HelpBox(
                "RuntimeObj is null on this binding at runtime.\n"
                    + "Ensure your renderer / game code assigns a RuntimeObj to RuntimeBinding.RuntimeObj.",
                MessageType.Warning
            );
            return;
        }

        DrawRuntimeObjHeader(runtimeObj);

        _showComponents = EditorGUILayout.Foldout(_showComponents, "RuntimeObjComponents");
        if (_showComponents)
        {
            DrawRuntimeObjComponents(runtimeObj);
        }
    }

    private static void DrawRuntimeObjHeader(RuntimeObj obj)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("RuntimeObj", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Name", obj.Name ?? "<null>");
            EditorGUILayout.LabelField("RuntimeId", obj.RuntimeId.ToString());
            EditorGUILayout.LabelField("Prefab ID", obj.RenderPrefabID.ToString());

            // Simple transform debug info
            var t = obj.Transform;
            EditorGUILayout.LabelField(
                "Position",
                $"({t.Position.X}, {t.Position.Y}, {t.Position.Z})"
            );
            EditorGUILayout.LabelField(
                "Rotation (Euler XYZ)",
                $"({t.EulerAngles.X}, {t.EulerAngles.Y}, {t.EulerAngles.Z})"
            );
            EditorGUILayout.LabelField(
                "Scale",
                $"({t.LossyScale.X}, {t.LossyScale.Y}, {t.LossyScale.Z})"
            );
        }
    }

    private static void DrawRuntimeObjComponents(RuntimeObj obj)
    {
        var components = obj.GameComponents;

        if (components == null || components.Count == 0)
        {
            EditorGUILayout.LabelField("No RuntimeObjComponents on this RuntimeObj.");
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

    private static void DrawComponentFields(RuntimeObjComponent component, Type type)
    {
        // We only care about the concrete type's own public fields (and base data fields),
        // not the RuntimeObj back‑reference etc.
        const BindingFlags flags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        // Walk up the inheritance chain until RuntimeObjComponent
        var types = new List<Type>();
        var current = type;
        while (
            current != null && current != typeof(RuntimeObjComponent) && current != typeof(object)
        )
        {
            types.Add(current);
            current = current.BaseType;
        }

        // Include fields declared directly on RuntimeObjComponent, but skip RuntimeObj reference itself
        types.Add(typeof(RuntimeObjComponent));

        foreach (var t in types)
        {
            var fields = t.GetFields(flags);
            foreach (var field in fields)
            {
                // Skip back‑references and anything Unity shouldn't poke at
                if (field.Name == nameof(RuntimeObjComponent.RuntimeObj))
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
