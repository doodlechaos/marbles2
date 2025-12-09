using System;
using FPMathLib;
using GameCoreLib;
using LockSim;
using UnityEngine;

/// <summary>
/// Base class for Unity components that bind to GameCore TileBase instances.
/// Provides shared functionality for physics debug visualization.
/// </summary>
[Serializable]
public abstract class TileBinding : MonoBehaviour
{
    /// <summary>
    /// When enabled, draws wireframe outlines for LockSim physics colliders.
    /// Green = dynamic bodies, Blue = static bodies.
    /// </summary>
    [SerializeField]
    protected bool showLockSimDebug = false;

    /// <summary>
    /// Get the TileBase this binding is associated with.
    /// </summary>
    public abstract TileBase Tile { get; }

    /// <summary>
    /// Check if this binding has a valid tile reference.
    /// </summary>
    public abstract bool IsValid { get; }

    /// <summary>
    /// The tile's world ID.
    /// </summary>
    public byte TileWorldId => Tile?.TileWorldId ?? 0;

    /// <summary>
    /// The name of the tile type.
    /// </summary>
    public string TileTypeName => Tile?.GetType().Name ?? "Unknown";

    protected virtual void OnDrawGizmos()
    {
        if (Tile?.Sim == null || !showLockSimDebug)
            return;

        var world = Tile.Sim;
        Vector3 tileOffset = transform.position;

        DrawLockSimColliders(world, tileOffset);
        DrawActiveCollisions(world, tileOffset);
    }

    protected void DrawLockSimColliders(World world, Vector3 tileOffset)
    {
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

    protected void DrawActiveCollisions(World world, Vector3 tileOffset)
    {
        var pairs = world.ActiveCollisionPairs;
        var dataList = world.ActiveCollisionData;

        if (pairs == null || dataList == null)
            return;

        const float contactRadius = 0.05f;
        const float normalLength = 0.4f;

        int count = Math.Min(pairs.Count, dataList.Count);

        for (int i = 0; i < count; i++)
        {
            var data = dataList[i];

            // Contact point in world space
            Vector3 contactPos =
                new Vector3(data.ContactPoint.X.ToFloat(), data.ContactPoint.Y.ToFloat(), 0f)
                + tileOffset;

            // Color: solid vs trigger
            Gizmos.color = data.IsTrigger ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(contactPos, contactRadius);

            // Normal arrow
            Vector3 n = new Vector3(data.Normal.X.ToFloat(), data.Normal.Y.ToFloat(), 0f);

            if (n.sqrMagnitude > 0.0001f)
            {
                Vector3 dir = n.normalized;
                Vector3 end = contactPos + dir * normalLength;

                Gizmos.DrawLine(contactPos, end);

                // Tiny arrow head
                Vector3 right = Quaternion.AngleAxis(20f, Vector3.forward) * (-dir);
                Vector3 left = Quaternion.AngleAxis(-20f, Vector3.forward) * (-dir);
                Gizmos.DrawLine(end, end + right * 0.1f);
                Gizmos.DrawLine(end, end + left * 0.1f);
            }
        }
    }

    protected void DrawWireBox(Vector3 center, BoxShape box, float rotationRad)
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

    protected void DrawWireCircle(Vector3 center, float radius)
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
