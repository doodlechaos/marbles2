using System;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Component that binds a Unity GameObject (the render root) to a ThroneTile.
/// Provides easy access to throne state information from Unity's inspector and scripts.
/// </summary>
[Serializable]
public sealed class ThroneTileBinding : TileBinding
{
    /// <summary>
    /// Reference to the ThroneTile this render root represents.
    /// </summary>
    [SerializeField]
    private ThroneTile throneTile;

    /// <summary>
    /// The ThroneTile this binding is associated with.
    /// </summary>
    public ThroneTile ThroneTile
    {
        get => throneTile;
        set => throneTile = value;
    }

    /// <summary>
    /// Implementation of TileBinding.Tile
    /// </summary>
    public override TileBase Tile => throneTile;

    /// <summary>
    /// Implementation of TileBinding.IsValid
    /// </summary>
    public override bool IsValid => throneTile != null;

    /// <summary>
    /// The account ID of the current king.
    /// </summary>
    public ulong KingAccountId => throneTile?.KingAccountId ?? 0;

    /// <summary>
    /// Whether there is currently a king on the throne.
    /// </summary>
    public bool HasKing => throneTile?.HasKing ?? false;

    [SerializeField]
    private Vector2 screenOffset = new Vector2(0, 50);
    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    void OnGUI()
    {
        if (throneTile == null)
            return;

        // Lazy init styles
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
        }

        // Convert world position to screen position
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

        // Skip if behind camera
        if (screenPos.z < 0)
            return;

        // Flip Y for GUI coordinates
        float guiY = Screen.height - screenPos.y;

        // Build display text
        string kingText = HasKing ? $"King: {KingAccountId}" : "Throne Empty";

        // Draw box with label
        Rect boxRect = new Rect(
            screenPos.x + screenOffset.x - 120,
            guiY + screenOffset.y - 20,
            240,
            50
        );

        // Color based on throne state
        GUI.backgroundColor = HasKing ? Color.yellow : Color.gray;
        GUI.Box(boxRect, "", boxStyle);

        // Draw king text
        Rect textRect = new Rect(boxRect.x, boxRect.y + 10, boxRect.width, 30);
        GUI.Label(textRect, kingText, labelStyle);

        GUI.backgroundColor = Color.white;
    }
}
