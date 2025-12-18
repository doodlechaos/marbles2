using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GreyOutAttribute))]
public class GreyOutDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Save the original GUI enabled state
        bool previousGUIState = GUI.enabled;

        // Disable the GUI to grey out the property
        GUI.enabled = false;

        // Draw the property
        EditorGUI.PropertyField(position, property, label);

        // Restore the original GUI enabled state
        GUI.enabled = previousGUIState;
    }
}
