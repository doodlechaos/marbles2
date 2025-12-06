using System;
using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

namespace LockSim
{
    /// <summary>
    /// Contains the deterministic state of the physics simulation.
    /// This is the minimal data that needs to be serialized for snapshots.
    /// </summary>
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class World
    {
        // Physics settings
        [MemoryPackOrder(0)]
        public FPVector2 Gravity = FPVector2.FromFloats(0f, -9.81f);

        // Bodies storage
        [MemoryPackOrder(1), MemoryPackInclude]
        private List<RigidBodyLS> bodies = new List<RigidBodyLS>();

        [MemoryPackOrder(2), MemoryPackInclude]
        private int nextBodyId = 0;

        // Colliders storage
        [MemoryPackOrder(3), MemoryPackInclude]
        private List<ColliderLS> colliders = new List<ColliderLS>();

        [MemoryPackOrder(4), MemoryPackInclude]
        private int nextColliderId = 0;

        // Active collision pairs from the previous frame (for Enter/Stay/Exit detection)
        // Using parallel lists instead of Dictionary for deterministic serialization order
        [MemoryPackOrder(5), MemoryPackInclude]
        private List<ColliderPair> activeCollisionPairs = new List<ColliderPair>();

        [MemoryPackOrder(6), MemoryPackInclude]
        private List<CollisionPairData> activeCollisionData = new List<CollisionPairData>();

        [MemoryPackIgnore]
        public IReadOnlyList<RigidBodyLS> Bodies => bodies;

        [MemoryPackIgnore]
        public IReadOnlyList<ColliderLS> Colliders => colliders;

        /// <summary>
        /// Active collision pairs from the previous frame.
        /// Exposed as read-only for debug/visualization. Serialized via the backing list.
        /// </summary>
        [MemoryPackIgnore]
        public IReadOnlyList<ColliderPair> ActiveCollisionPairs => activeCollisionPairs;

        /// <summary>
        /// Active collision pair data (normals/contact points) from the previous frame.
        /// Exposed as read-only for debug/visualization. Serialized via the backing list.
        /// </summary>
        [MemoryPackIgnore]
        public IReadOnlyList<CollisionPairData> ActiveCollisionData => activeCollisionData;

        #region Body Management

        public int AddBody(RigidBodyLS body)
        {
            body.Id = nextBodyId++;
            bodies.Add(body);
            return body.Id;
        }

        public void RemoveBody(int bodyId)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == bodyId)
                {
                    bodies.RemoveAt(i);
                    break;
                }
            }
            // Also remove colliders attached to this body
            for (int i = colliders.Count - 1; i >= 0; i--)
            {
                if (colliders[i].ParentBodyId == bodyId)
                {
                    int colliderId = colliders[i].Id;
                    colliders.RemoveAt(i);
                    // Clean up any active collision pairs referencing this collider
                    RemoveCollisionPairsForCollider(colliderId);
                }
            }
        }

        public RigidBodyLS GetBody(int bodyId)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == bodyId)
                {
                    return bodies[i];
                }
            }
            throw new ArgumentException($"Body with ID {bodyId} not found");
        }

        public bool TryGetBody(int bodyId, out RigidBodyLS body)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == bodyId)
                {
                    body = bodies[i];
                    return true;
                }
            }
            body = default;
            return false;
        }

        public void SetBody(int bodyId, RigidBodyLS body)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == bodyId)
                {
                    bodies[i] = body;
                    return;
                }
            }
            throw new ArgumentException($"Body with ID {bodyId} not found");
        }

        #endregion

        #region Collider Management

        public int AddCollider(ColliderLS collider)
        {
            collider.Id = nextColliderId++;
            colliders.Add(collider);
            return collider.Id;
        }

        /// <summary>
        /// Adds a collider and attaches it to a body, updating the body's inertia.
        /// </summary>
        public int AddColliderToBody(ColliderLS collider, int bodyId)
        {
            collider.ParentBodyId = bodyId;
            collider.Id = nextColliderId++;
            colliders.Add(collider);

            // Update body's inertia from collider
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == bodyId)
                {
                    RigidBodyLS body = bodies[i];
                    body.SetInertiaFromCollider(collider);
                    bodies[i] = body;
                    break;
                }
            }

            return collider.Id;
        }

        public void RemoveCollider(int colliderId)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].Id == colliderId)
                {
                    colliders.RemoveAt(i);
                    // Clean up any active collision pairs referencing this collider
                    RemoveCollisionPairsForCollider(colliderId);
                    return;
                }
            }
        }

        public ColliderLS GetCollider(int colliderId)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].Id == colliderId)
                {
                    return colliders[i];
                }
            }
            throw new ArgumentException($"Collider with ID {colliderId} not found");
        }

        public bool TryGetCollider(int colliderId, out ColliderLS collider)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].Id == colliderId)
                {
                    collider = colliders[i];
                    return true;
                }
            }
            collider = default;
            return false;
        }

        public void SetCollider(int colliderId, ColliderLS collider)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].Id == colliderId)
                {
                    colliders[i] = collider;
                    return;
                }
            }
            throw new ArgumentException($"Collider with ID {colliderId} not found");
        }

        #endregion

        public void Clear()
        {
            bodies.Clear();
            colliders.Clear();
            activeCollisionPairs.Clear();
            activeCollisionData.Clear();
            nextBodyId = 0;
            nextColliderId = 0;
        }

        #region Collision Pair Tracking

        /// <summary>
        /// Checks if a collision pair is currently active from the previous frame.
        /// </summary>
        public bool HasActiveCollisionPair(ColliderPair pair, out CollisionPairData data)
        {
            for (int i = 0; i < activeCollisionPairs.Count; i++)
            {
                if (activeCollisionPairs[i] == pair)
                {
                    data = activeCollisionData[i];
                    return true;
                }
            }
            data = default;
            return false;
        }

        /// <summary>
        /// Removes all active collision pairs that reference the given collider.
        /// Called when a collider is removed to keep state consistent.
        /// </summary>
        private void RemoveCollisionPairsForCollider(int colliderId)
        {
            for (int i = activeCollisionPairs.Count - 1; i >= 0; i--)
            {
                ColliderPair pair = activeCollisionPairs[i];
                if (pair.ColliderIdA == colliderId || pair.ColliderIdB == colliderId)
                {
                    activeCollisionPairs.RemoveAt(i);
                    activeCollisionData.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Gets all active collision pairs from the previous frame (mutable for physics pipeline).
        /// </summary>
        internal List<ColliderPair> GetActiveCollisionPairsMutable() => activeCollisionPairs;

        /// <summary>
        /// Gets all active collision pair data from the previous frame (mutable for physics pipeline).
        /// </summary>
        internal List<CollisionPairData> GetActiveCollisionDataMutable() => activeCollisionData;

        #endregion

        // Internal access for physics pipeline
        internal List<RigidBodyLS> GetBodiesMutable() => bodies;

        internal List<ColliderLS> GetCollidersMutable() => colliders;

        public byte[] ToSnapshot()
        {
            return MemoryPackSerializer.Serialize(this);
        }

        public string GetDeterministicHashHex()
        {
            byte[] snapshotData = ToSnapshot();
            return snapshotData.GetDeterministicHashHex();
        }
    }
}
