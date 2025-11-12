using System;
using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

namespace LockSim
{
    /// <summary>
    /// Contains only the deterministic state of the physics simulation.
    /// This is the minimal data that needs to be serialized for snapshots.
    /// </summary>
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class World
    {
        // Physics settings
        [MemoryPackOrder(0)]
        public FPVector2 Gravity = FPVector2.FromFloats(0f, -9.81f);

        // Bodies storage (using List for deterministic ordering)
        [MemoryPackOrder(1), MemoryPackInclude]
        private List<RigidBodyLS> bodies = new List<RigidBodyLS>();

        [MemoryPackOrder(2), MemoryPackInclude]
        private int nextBodyId = 0;

        [MemoryPackIgnore]
        public IReadOnlyList<RigidBodyLS> Bodies => bodies;

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
                    return;
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

        public void Clear()
        {
            bodies.Clear();
            nextBodyId = 0;
        }

        // Internal access for physics pipeline
        internal List<RigidBodyLS> GetBodiesMutable() => bodies;

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
