namespace LockSim
{
    using System;
    using FPMathLib;
    using MemoryPack;

    /// <summary>
    /// The type of collision/trigger event.
    /// </summary>
    public enum CollisionEventType : byte
    {
        /// <summary>Two colliders just started overlapping this frame.</summary>
        Enter = 0,

        /// <summary>Two colliders continue to overlap from the previous frame.</summary>
        Stay = 1,

        /// <summary>Two colliders stopped overlapping this frame.</summary>
        Exit = 2,
    }

    /// <summary>
    /// Identifies a unique pair of colliders, stored with ordered IDs for deterministic comparison.
    /// Used internally to track active collisions across frames.
    /// </summary>
    [Serializable]
    [MemoryPackable]
    public readonly partial struct ColliderPair : IEquatable<ColliderPair>
    {
        /// <summary>The smaller collider ID.</summary>
        public readonly int ColliderIdA;

        /// <summary>The larger collider ID.</summary>
        public readonly int ColliderIdB;

        public ColliderPair(int colliderIdA, int colliderIdB)
        {
            // Always store in sorted order for deterministic comparison
            if (colliderIdA <= colliderIdB)
            {
                ColliderIdA = colliderIdA;
                ColliderIdB = colliderIdB;
            }
            else
            {
                ColliderIdA = colliderIdB;
                ColliderIdB = colliderIdA;
            }
        }

        public bool Equals(ColliderPair other)
        {
            return ColliderIdA == other.ColliderIdA && ColliderIdB == other.ColliderIdB;
        }

        public override bool Equals(object obj)
        {
            return obj is ColliderPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ColliderIdA * 397 ^ ColliderIdB;
        }

        public static bool operator ==(ColliderPair left, ColliderPair right) => left.Equals(right);

        public static bool operator !=(ColliderPair left, ColliderPair right) =>
            !left.Equals(right);
    }

    /// <summary>
    /// Data associated with an active collision pair, stored for frame-to-frame tracking.
    /// </summary>
    [Serializable]
    [MemoryPackable]
    public partial struct CollisionPairData
    {
        /// <summary>Body ID of the first collider's parent (-1 if orphan).</summary>
        public int BodyIdA;

        /// <summary>Body ID of the second collider's parent (-1 if orphan).</summary>
        public int BodyIdB;

        /// <summary>Whether this is a trigger overlap (vs a solid collision).</summary>
        public bool IsTrigger;

        /// <summary>Collision normal pointing from A to B.</summary>
        public FPVector2 Normal;

        /// <summary>First contact point in world space.</summary>
        public FPVector2 ContactPoint;
    }

    /// <summary>
    /// Represents a collision or trigger event for the current frame.
    /// Similar to Unity's Collision2D/ContactPoint2D but deterministic.
    /// </summary>
    [Serializable]
    public struct CollisionEvent
    {
        /// <summary>The type of event (Enter/Stay/Exit).</summary>
        public CollisionEventType EventType;

        /// <summary>Whether this is a trigger event (vs a collision event).</summary>
        public bool IsTrigger;

        /// <summary>ID of the first collider involved.</summary>
        public int ColliderIdA;

        /// <summary>ID of the second collider involved.</summary>
        public int ColliderIdB;

        /// <summary>ID of the first body involved (-1 if orphan collider).</summary>
        public int BodyIdA;

        /// <summary>ID of the second body involved (-1 if orphan collider).</summary>
        public int BodyIdB;

        /// <summary>
        /// Collision normal pointing from A to B.
        /// Only valid for Enter/Stay events; zero for Exit events.
        /// </summary>
        public FPVector2 Normal;

        /// <summary>
        /// Contact point in world space.
        /// Only valid for Enter/Stay events; zero for Exit events.
        /// </summary>
        public FPVector2 ContactPoint;

        /// <summary>
        /// Creates a collision event.
        /// </summary>
        public static CollisionEvent Create(
            CollisionEventType eventType,
            bool isTrigger,
            int colliderIdA,
            int colliderIdB,
            int bodyIdA,
            int bodyIdB,
            FPVector2 normal,
            FPVector2 contactPoint
        )
        {
            return new CollisionEvent
            {
                EventType = eventType,
                IsTrigger = isTrigger,
                ColliderIdA = colliderIdA,
                ColliderIdB = colliderIdB,
                BodyIdA = bodyIdA,
                BodyIdB = bodyIdB,
                Normal = normal,
                ContactPoint = contactPoint,
            };
        }
    }
}
