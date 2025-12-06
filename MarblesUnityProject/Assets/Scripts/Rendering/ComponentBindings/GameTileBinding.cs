using System;
using FPMathLib;
using GameCoreLib;
using LockSim;
using UnityEngine;

/// <summary>
/// Component that binds a Unity GameObject (the render root) to a GameTileBase.
/// Provides easy access to tile state information from Unity's inspector and scripts.
/// </summary>
[Serializable]
public sealed class GameTileBinding : MonoBehaviour
{
    /// <summary>
    /// Reference to the GameTileBase this render root represents.
    /// </summary>
    [SerializeField]
    private GameTileBase gameTile;

    /// <summary>
    /// When enabled, draws wireframe outlines for LockSim physics colliders.
    /// Green = dynamic bodies, Blue = static bodies.
    /// </summary>
    [SerializeField]
    private bool showLockSimColliders = false;

    /// <summary>
    /// The GameTile this binding is associated with.
    /// </summary>
    public GameTileBase GameTile
    {
        get => gameTile;
        set => gameTile = value;
    }

    /// <summary>
    /// The current state of the game tile.
    /// </summary>
    public GameTileState State => gameTile?.State ?? GameTileState.Spinning;

    /// <summary>
    /// The tile's world ID (1 or 2).
    /// </summary>
    public byte TileWorldId => gameTile?.TileWorldId ?? 0;

    /// <summary>
    /// The name of the GameTile type (e.g., "SimpleBattleRoyale").
    /// </summary>
    public string GameTypeName => gameTile?.GetType().Name ?? "Unknown";

    /// <summary>
    /// Check if this binding has a valid GameTile reference.
    /// </summary>
    public bool IsValid => gameTile != null;

    [SerializeField]
    private Vector2 screenOffset = new Vector2(0, 50);
    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    /// <summary>
    /// Try to get the GameTile as a specific derived type.
    /// </summary>
    /// <typeparam name="T">The specific GameTile type to cast to.</typeparam>
    /// <param name="result">The casted GameTile if successful.</param>
    /// <returns>True if the cast was successful.</returns>
    public bool TryGetGameTile<T>(out T result)
        where T : GameTileBase
    {
        result = gameTile as T;
        return result != null;
    }

    void OnGUI()
    {
        // Lazy init styles
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 28,
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
        var tile = GameTile;
        string stateText = $"Tile {TileWorldId}: {tile.State}";
        string stepsText = $"Steps: {tile.StateSteps}";

        // Draw box with label
        Rect boxRect = new Rect(
            screenPos.x + screenOffset.x - 160,
            guiY + screenOffset.y - 30,
            320,
            90
        );

        // Color based on state
        Color stateColor = tile.State switch
        {
            GameTileState.Spinning => Color.yellow,
            GameTileState.OpeningDoor => Color.cyan,
            GameTileState.Bidding => Color.green,
            GameTileState.Gameplay => Color.white,
            GameTileState.ScoreScreen => Color.magenta,
            GameTileState.Finished => Color.gray,
            _ => Color.white,
        };

        GUI.backgroundColor = stateColor;
        GUI.Box(boxRect, "", boxStyle);

        // Draw state text
        Rect stateRect = new Rect(boxRect.x, boxRect.y + 10, boxRect.width, 30);
        GUI.Label(stateRect, stateText, labelStyle);

        // Draw steps text below
        Rect stepsRect = new Rect(boxRect.x, boxRect.y + 45, boxRect.width, 30);
        GUI.Label(stepsRect, stepsText, labelStyle);

        GUI.backgroundColor = Color.white;
    }

    void OnDrawGizmos()
    {
        if (!showLockSimColliders || gameTile?.Sim == null)
            return;

        var world = gameTile.Sim;
        Vector3 tileOffset = transform.position;

        foreach (var collider in world.Colliders)
        {
            // Get world transform from parent body if attached
            FPVector2 pos;
            FP rot;
            bool isDynamic = false;

            if (world.TryGetBody(collider.ParentBodyId, out var body))
            {
                pos = collider.GetWorldPosition(body);
                rot = collider.GetWorldRotation(body);
                isDynamic = body.BodyType == BodyType.Dynamic;
            }
            else
            {
                pos = collider.LocalPosition;
                rot = collider.LocalRotation;
            }

            Gizmos.color = isDynamic ? Color.green : Color.blue;

            Vector3 colliderPos = new Vector3(pos.X.ToFloat(), pos.Y.ToFloat(), 0f) + tileOffset;
            float rotation = rot.ToFloat();

            if (collider.ShapeType == ShapeType.Box)
            {
                DrawWireBox(colliderPos, collider.BoxShape, rotation);
            }
            else if (collider.ShapeType == ShapeType.Circle)
            {
                DrawWireCircle(colliderPos, collider.CircleShape.Radius.ToFloat());
            }
        }
    }

    private void DrawWireBox(Vector3 center, BoxShape box, float rotationRad)
    {
        float hw = box.HalfWidth.ToFloat();
        float hh = box.HalfHeight.ToFloat();

        // Compute rotated corners
        float cos = Mathf.Cos(rotationRad);
        float sin = Mathf.Sin(rotationRad);

        // Local corners (before rotation)
        Vector2[] localCorners = new Vector2[]
        {
            new Vector2(-hw, -hh),
            new Vector2(hw, -hh),
            new Vector2(hw, hh),
            new Vector2(-hw, hh),
        };

        // Rotate and translate corners to world space
        Vector3[] worldCorners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            float rx = localCorners[i].x * cos - localCorners[i].y * sin;
            float ry = localCorners[i].x * sin + localCorners[i].y * cos;
            worldCorners[i] = center + new Vector3(rx, ry, 0f);
        }

        // Draw the 4 edges
        Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
        Gizmos.DrawLine(worldCorners[2], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[3], worldCorners[0]);
    }

    private void DrawWireCircle(Vector3 center, float radius)
    {
        const int segments = 32;
        float angleStep = 2f * Mathf.PI / segments;

        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 point =
                center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}
