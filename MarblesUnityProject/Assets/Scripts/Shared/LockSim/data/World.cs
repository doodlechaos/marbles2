using FPMathLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;


namespace LockSim
{
    /// <summary>
    /// Contains only the deterministic state of the physics simulation.
    /// This is the minimal data that needs to be serialized for snapshots.
    /// </summary>
    [Serializable]
    public class World
    {
        // Physics settings
        public FPVector2 Gravity = FPVector2.FromFloats(0f, -9.81f);

        // Bodies storage (using List for deterministic ordering)
        private List<RigidBodyLS> bodies = new List<RigidBodyLS>();
        private int nextBodyId = 0;

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
        }

        /// <summary>
        /// Computes a SHA256 hash of the entire world state for determinism testing.
        /// </summary>
        public string GetWorldHash()
        {
            Snapshot snapshot = TakeSnapshot();

            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, snapshot);
                byte[] serializedBytes = memoryStream.ToArray();

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(serializedBytes);
                    var hashString = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        hashString.Append(b.ToString("x2"));
                    }
                    return hashString.ToString();
                }
            }
        }
    }
}
