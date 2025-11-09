using System;
using System.Collections.Generic;

namespace LockSim
{
    [Serializable]
    public class World
    {
        // Physics settings
        public FPVector2 Gravity = FPVector2.FromFloats(0f, -9.81f);
        public int VelocityIterations = 8;
        public int PositionIterations = 3;

        // Bodies storage (using List for deterministic ordering)
        private List<RigidBodyLS> bodies = new List<RigidBodyLS>();
        private int nextBodyId = 0;

        // Contacts from last step
        private List<ContactManifold> contacts = new List<ContactManifold>();

        public IReadOnlyList<RigidBodyLS> Bodies => bodies;
        public IReadOnlyList<ContactManifold> Contacts => contacts;

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
            contacts.Clear();
            nextBodyId = 0;
        }

        // Internal access for physics pipeline
        internal List<RigidBodyLS> GetBodiesMutable() => bodies;
        internal List<ContactManifold> GetContactsMutable() => contacts;

        // Snapshot and restore for deterministic replay
        [Serializable]
        public class Snapshot
        {
            public RigidBodyLS[] Bodies;
            public int NextBodyId;
            public FPVector2 Gravity;

            public Snapshot(World world)
            {
                Bodies = world.bodies.ToArray();
                NextBodyId = world.nextBodyId;
                Gravity = world.Gravity;
            }
        }

        public Snapshot TakeSnapshot()
        {
            return new Snapshot(this);
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            bodies.Clear();
            bodies.AddRange(snapshot.Bodies);
            nextBodyId = snapshot.NextBodyId;
            Gravity = snapshot.Gravity;
            contacts.Clear();
        }
    }
}
