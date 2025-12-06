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

        [MemoryPackIgnore]
        public IReadOnlyList<RigidBodyLS> Bodies => bodies;

        [MemoryPackIgnore]
        public IReadOnlyList<ColliderLS> Colliders => colliders;

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
                    colliders.RemoveAt(i);
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
            nextBodyId = 0;
            nextColliderId = 0;
        }

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
