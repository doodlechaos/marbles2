using System;
using System.Collections.Generic;
using System.Diagnostics;
using FPMathLib;
using LockSim;
using MemoryPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class GameTile
    {
        [MemoryPackOrder(0)]
        public int WorldId;

        [MemoryPackOrder(1)]
        public RuntimeObj TileRoot;

        [MemoryPackOrder(2)]
        public World Sim;

        // Track mapping between RuntimeId and their physics body IDs
        // Uses stable IDs instead of object references, so it survives serialization
        [MemoryPackOrder(3), MemoryPackInclude]
        private Dictionary<ulong, int> runtimeIdToBodyId = new Dictionary<ulong, int>();

        public GameTile(int worldId)
        {
            WorldId = worldId;
            Sim = new World();
        }

        public void Load(string levelJSON, GameCore gameCore)
        {
            Logger.Log("Loading level JSON...");

            // Clear any existing data
            Clear();

            // Initialize the physics world if needed
            if (Sim == null)
            {
                Sim = new World();
            }

            // Deserialize the JSON into a RuntimeObj hierarchy
            TileRoot = JsonConvert.DeserializeObject<RuntimeObj>(levelJSON);

            if (TileRoot == null)
            {
                Logger.Error("Failed to deserialize level JSON");
                return;
            }

            // Assign stable RuntimeIds to all objects in the hierarchy
            AssignRuntimeIds(TileRoot, gameCore);

            // Process the hierarchy recursively to set up parent references and physics
            ProcessRuntimeObjHierarchy(TileRoot, null);

            Logger.Log($"Level loaded successfully. Bodies in simulation: {Sim.Bodies.Count}");
        }

        /// <summary>
        /// Recursively assign unique RuntimeIds to all RuntimeObjs in the hierarchy.
        /// This ensures stable IDs that persist through serialization.
        /// </summary>
        private void AssignRuntimeIds(RuntimeObj obj, GameCore gameCore)
        {
            // Assign ID and increment counter
            obj.RuntimeId = gameCore.NextRuntimeId;
            gameCore.NextRuntimeId++;

            // Recursively assign IDs to children
            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    AssignRuntimeIds(child, gameCore);
                }
            }
        }

        private void ProcessRuntimeObjHierarchy(RuntimeObj obj, RuntimeObj parent)
        {
            // Set up parent reference
            //  obj.Parent = parent;

            // Process components to create physics bodies
            ProcessComponents(obj);

            // Recursively process children
            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ProcessRuntimeObjHierarchy(child, obj);
                }
            }
        }

        private void ProcessComponents(RuntimeObj obj)
        {
            if (obj.Components == null || obj.Components.Count == 0)
                return;

            // Check if this object has a collider or rigidbody component
            bool hasCollider = false;
            bool hasRigidbody = false;
            ComponentData colliderComponent = null;
            ComponentData rigidbodyComponent = null;

            foreach (var component in obj.Components)
            {
                if (
                    component.type.Contains("BoxCollider2D")
                    || component.type.Contains("CircleCollider2D")
                )
                {
                    hasCollider = true;
                    colliderComponent = component;
                }
                else if (component.type.Contains("Rigidbody2D"))
                {
                    hasRigidbody = true;
                    rigidbodyComponent = component;
                }
            }

            // If there's a collider, create a physics body
            if (hasCollider)
            {
                CreatePhysicsBody(obj, colliderComponent, rigidbodyComponent, hasRigidbody);
            }
        }

        private void CreatePhysicsBody(
            RuntimeObj obj,
            ComponentData colliderComponent,
            ComponentData rigidbodyComponent,
            bool hasRigidbody
        )
        {
            // Get world position and rotation from transform
            FPVector2 position = new FPVector2(obj.Transform.Position.X, obj.Transform.Position.Y);

            // Convert quaternion to 2D rotation (Z-axis rotation in radians)
            FPVector3 euler = obj.Transform.EulerAngles;
            FP rotation = euler.Z; // Z-axis rotation for 2D

            // Determine if static or dynamic
            bool isStatic = !hasRigidbody;

            // Parse rigidbody data if present
            FP mass = FP.One;
            if (hasRigidbody && rigidbodyComponent != null)
            {
                try
                {
                    var rbData = JObject.Parse(rigidbodyComponent.data);

                    // Check bodyType
                    if (rbData["bodyType"] != null)
                    {
                        int bodyTypeValue = rbData["bodyType"].Value<int>();
                        isStatic = (bodyTypeValue == 2); // RigidbodyType2D.Static = 2
                    }

                    // Get mass if dynamic
                    if (!isStatic && rbData["mass"] != null)
                    {
                        mass = FP.FromFloat(rbData["mass"].Value<float>());
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to parse rigidbody data: {e.Message}");
                }
            }

            // Create the body
            RigidBodyLS body;
            if (isStatic)
            {
                body = RigidBodyLS.CreateStatic(0, position, rotation);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotation, mass);
            }

            // Set shape based on collider type
            try
            {
                var colliderData = JObject.Parse(colliderComponent.data);

                if (colliderComponent.type.Contains("BoxCollider2D"))
                {
                    // Get size from collider
                    FP width = FP.One;
                    FP height = FP.One;

                    if (colliderData["size"] != null)
                    {
                        var size = colliderData["size"];
                        width = FP.FromFloat(size["x"].Value<float>());
                        height = FP.FromFloat(size["y"].Value<float>());
                    }

                    // Apply scale from transform
                    width = width * FPMath.Abs(obj.Transform.LossyScale.X);
                    height = height * FPMath.Abs(obj.Transform.LossyScale.Y);

                    body.SetBoxShape(width, height);
                }
                else if (colliderComponent.type.Contains("CircleCollider2D"))
                {
                    // Get radius from collider
                    FP radius = FP.Half;

                    if (colliderData["radius"] != null)
                    {
                        radius = FP.FromFloat(colliderData["radius"].Value<float>());
                    }

                    // Apply scale from transform (use max of x/y scale)
                    FP scaleX = FPMath.Abs(obj.Transform.LossyScale.X);
                    FP scaleY = FPMath.Abs(obj.Transform.LossyScale.Y);
                    radius = radius * FPMath.Max(scaleX, scaleY);

                    body.SetCircleShape(radius);
                }

                // Set material properties if available
                // Note: Unity stores material as a separate asset, so we'll use defaults
                body.Friction = FP.FromFloat(0.5f);
                body.Restitution = FP.FromFloat(0.2f);
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to parse collider data for {obj.Name}: {e.Message}");
                return;
            }

            // Add body to world
            int bodyId = Sim.AddBody(body);

            // Store mapping by RuntimeId (not by reference!)
            runtimeIdToBodyId[obj.RuntimeId] = bodyId;

            Logger.Log(
                $"Created physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, Type={body.BodyType}, Shape={body.ShapeType}"
            );
        }

        public void Clear()
        {
            // Clear the Runtime Objects
            TileRoot = null;
            runtimeIdToBodyId.Clear();

            // Clear the physics simulation world
            if (Sim != null)
            {
                Sim = new World();
                Sim.Gravity = FPVector2.FromFloats(0f, -9.81f);
            }
        }

        public void Step()
        {
            PhysicsPipeline.Step(Sim, FP.FromFloat(1 / 60f), new WorldSimulationContext());

            // Sync physics body transforms back to RuntimeObj transforms
            SyncPhysicsToRuntimeObjs();
        }

        /// <summary>
        /// Update RuntimeObj transforms from physics body positions/rotations.
        /// This method traverses the RuntimeObj tree and syncs physics by RuntimeId,
        /// making it robust across serialization/deserialization.
        /// </summary>
        private void SyncPhysicsToRuntimeObjs()
        {
            if (TileRoot == null)
                return;

            // Recursively sync all RuntimeObjs that have physics bodies
            SyncPhysicsRecursive(TileRoot);
        }

        /// <summary>
        /// Recursively sync physics for a RuntimeObj and its children
        /// </summary>
        private void SyncPhysicsRecursive(RuntimeObj runtimeObj)
        {
            // Check if this RuntimeObj has a physics body (by RuntimeId)
            if (runtimeIdToBodyId.TryGetValue(runtimeObj.RuntimeId, out int bodyId))
            {
                try
                {
                    var body = Sim.GetBody(bodyId);

                    // Update position (2D physics, so only X and Y)
                    FPVector3 newPosition = new FPVector3(
                        body.Position.X,
                        body.Position.Y,
                        runtimeObj.Transform.LocalPosition.Z // Keep Z unchanged
                    );
                    runtimeObj.Transform.LocalPosition = newPosition;

                    // Update rotation (convert 2D rotation to quaternion around Z axis)
                    FP angle = body.Rotation;
                    runtimeObj.Transform.LocalRotation = FPQuaternion.AngleAxis(
                        angle,
                        FPVector3.Forward
                    );
                }
                catch (System.Exception e)
                {
                    Logger.Error(
                        $"Failed to sync physics for RuntimeId {runtimeObj.RuntimeId}: {e.Message}"
                    );
                }
            }

            // Recursively sync children
            if (runtimeObj.Children != null)
            {
                foreach (var child in runtimeObj.Children)
                {
                    SyncPhysicsRecursive(child);
                }
            }
        }
    }
}
