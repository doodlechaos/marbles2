using FPMathLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;


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
        /// 
        static readonly JsonSerializerSettings HashJsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            // If you don't want to add attributes to your types, see Option B below for a resolver.
            // ContractResolver = new FieldsOnlyContractResolver()  // optional (see below)
        };

        public string GetWorldHash()
        {
            var snapshot = TakeSnapshot();

            string json = JsonConvert.SerializeObject(snapshot, HashJsonSettings);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
