using System;
using System.Collections.Generic;
using FPMathLib;
using LockSim;
using MemoryPack;

namespace GameCoreLib
{
    public enum GameTileState
    {
        Spinning,
        OpeningDoor,
        Bidding,
        Gameplay,
        ScoreScreen,
        Finished,
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(SimpleBattleRoyale))]
    public abstract partial class GameTileBase
    {
        [MemoryPackOrder(0)]
        public RuntimeObj TileRoot;

        [MemoryPackOrder(1)]
        public World Sim;

        /// <summary>
        /// Track mapping between RuntimeId and their physics body IDs.
        /// Uses stable IDs instead of object references, so it survives serialization.
        /// </summary>
        [MemoryPackOrder(2), MemoryPackInclude]
        protected Dictionary<ulong, int> runtimeIdToBodyId = new Dictionary<ulong, int>();

        /// <summary>
        /// GameCore Unique identifier for this tile. Used as the upper 16 bits of RuntimeIds
        /// to ensure global uniqueness across all tiles without needing GameCore reference.
        /// </summary>
        [MemoryPackOrder(3)]
        public byte TileWorldId;

        /// <summary>
        /// Counter for the local portion of RuntimeIds.
        /// Combined with TileId to form globally unique RuntimeIds.
        /// </summary>
        [MemoryPackOrder(4)]
        public ulong NextLocalId = 1;

        [MemoryPackOrder(5)]
        public GameTileState State = GameTileState.Spinning;

        [MemoryPackOrder(6)]
        protected int stateSteps = 0;

        [MemoryPackIgnore]
        public List<OutputToClientEvent> OutputToClientEvents = new List<OutputToClientEvent>();

        [MemoryPackIgnore]
        public List<OutputToServerEvent> OutputToServerEvents = new List<OutputToServerEvent>();

        public GameTileBase()
        {
            Sim = new World();
        }

        /// <summary>
        /// Generate a globally unique RuntimeId for this tile.
        /// Upper 16 bits = TileId, Lower 48 bits = local counter.
        /// </summary>
        protected ulong GenerateRuntimeId()
        {
            return ((ulong)TileWorldId << 48) | (NextLocalId++ & 0xFFFFFFFFFFFF);
        }

        /// <summary>
        /// Called by MemoryPack after deserialization.
        /// Rebuilds component references that aren't serialized.
        /// </summary>
        [MemoryPackOnDeserialized]
        private void OnMemoryPackDeserialized()
        {
            // Rebuild component -> RuntimeObj references in the hierarchy
            TileRoot?.RebuildComponentReferences();

            // Let derived classes rebuild their game-specific references
            OnAfterDeserialize();
        }

        /// <summary>
        /// Override in derived classes to rebuild game-specific component references
        /// after deserialization. Called after RebuildComponentReferences().
        /// </summary>
        protected virtual void OnAfterDeserialize() { }

        /// <summary>
        /// Initialize the GameTile for use in a specific tile slot.
        /// This assigns RuntimeIds, rebuilds component references, and sets up physics.
        /// Called after deserializing a GameTile template from storage.
        /// </summary>
        /// <param name="tileWorldId">The tile slot ID (1 or 2) for unique RuntimeId generation</param>
        public void Initialize(byte tileWorldId)
        {
            Logger.Log($"Initializing GameTile with TileId={tileWorldId}...");
            SetState(GameTileState.Spinning);

            // Set the tile ID for unique RuntimeId generation
            TileWorldId = tileWorldId;
            NextLocalId = 1;

            // Clear and recreate physics world
            runtimeIdToBodyId.Clear();
            Sim = new World();
            Sim.Gravity = FPVector2.FromFloats(0f, -9.81f);

            if (TileRoot == null)
            {
                Logger.Error("TileRoot is null - cannot initialize");
                return;
            }

            // Assign stable RuntimeIds to all objects in the hierarchy
            AssignRuntimeIds(TileRoot);

            // Rebuild component -> RuntimeObj references
            TileRoot.RebuildComponentReferences();

            // Process the hierarchy recursively to set up physics
            ProcessRuntimeObjHierarchy(TileRoot);

            // Post-load hook for derived classes
            OnLevelLoaded();

            Logger.Log(
                $"GameTile initialized successfully. Bodies in simulation: {Sim.Bodies.Count}"
            );
        }

        /// <summary>
        /// Override in derived classes to perform additional setup after level is loaded
        /// </summary>
        protected virtual void OnLevelLoaded() { }

        public virtual void StartGameplay(InputEvent.Entrant[] entrants, uint totalMarblesBid)
        {
            throw new NotImplementedException();
        }

        const float SPINNING_DURATION_SEC = 5.0f;
        const float OPENING_DOOR_DURATION_SEC = 2.0f;
        const float SCORE_SCREEN_DURATION_SEC = 5.0f;

        public virtual void Step(OutputEventBuffer outputEvents)
        {
            float stateDurationSec = stateSteps / 60.0f;
            if (State == GameTileState.Spinning)
            {
                if (stateDurationSec >= SPINNING_DURATION_SEC)
                    SetState(GameTileState.OpeningDoor);
                if (stateDurationSec >= OPENING_DOOR_DURATION_SEC)
                    SetState(GameTileState.Bidding);
                if (stateDurationSec >= SCORE_SCREEN_DURATION_SEC)
                    SetState(GameTileState.Finished);
            }
            PhysicsPipeline.Step(Sim, FP.FromFloat(1 / 60f), new WorldSimulationContext());
            SyncPhysicsToRuntimeObjs();
        }

        public void SetState(GameTileState state)
        {
            OutputToServerEvents.Add(
                new OutputToServerEvent.StateUpdatedTo { State = state, WorldId = TileWorldId }
            );
            stateSteps = 0;
            State = state;
        }

        private void AssignRuntimeIds(RuntimeObj obj)
        {
            obj.RuntimeId = GenerateRuntimeId();

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    AssignRuntimeIds(child);
                }
            }
        }

        private void ProcessRuntimeObjHierarchy(RuntimeObj obj)
        {
            // Process components to create physics bodies
            ProcessComponents(obj);

            // Recursively process children
            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ProcessRuntimeObjHierarchy(child);
                }
            }
        }

        /// <summary>
        /// Process strongly-typed components on a RuntimeObj to create physics bodies
        /// </summary>
        private void ProcessComponents(RuntimeObj obj)
        {
            if (obj.GameComponents == null || obj.GameComponents.Count == 0)
                return;

            // Find collider and rigidbody components
            BoxCollider2DComponent boxCollider = null;
            CircleCollider2DComponent circleCollider = null;
            Rigidbody2DComponent rigidbody = null;

            foreach (var component in obj.GameComponents)
            {
                switch (component)
                {
                    case BoxCollider2DComponent box:
                        boxCollider = box;
                        break;
                    case CircleCollider2DComponent circle:
                        circleCollider = circle;
                        break;
                    case Rigidbody2DComponent rb:
                        rigidbody = rb;
                        break;
                }
            }

            // Create physics body if there's a collider
            if (boxCollider != null)
            {
                CreatePhysicsBody(obj, boxCollider, rigidbody);
            }
            else if (circleCollider != null)
            {
                CreatePhysicsBody(obj, circleCollider, rigidbody);
            }
        }

        /// <summary>
        /// Create a physics body from a BoxCollider2DComponent
        /// </summary>
        private void CreatePhysicsBody(
            RuntimeObj obj,
            BoxCollider2DComponent collider,
            Rigidbody2DComponent rigidbody
        )
        {
            FPVector2 position = new FPVector2(obj.Transform.Position.X, obj.Transform.Position.Y);
            FP rotation = obj.Transform.EulerAngles.Z;

            // Determine if static or dynamic
            bool isStatic = rigidbody == null || rigidbody.BodyType == Rigidbody2DType.Static;
            FP mass = rigidbody?.Mass ?? FP.One;

            // Create body
            RigidBodyLS body;
            if (isStatic)
            {
                body = RigidBodyLS.CreateStatic(0, position, rotation);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotation, mass);
            }

            // Apply scale to size
            FP width = collider.Size.X * FPMath.Abs(obj.Transform.LossyScale.X);
            FP height = collider.Size.Y * FPMath.Abs(obj.Transform.LossyScale.Y);
            body.SetBoxShape(width, height);

            // Set material properties
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);

            // Add body to world and store mapping
            int bodyId = Sim.AddBody(body);
            runtimeIdToBodyId[obj.RuntimeId] = bodyId;

            Logger.Log(
                $"Created box physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, Size=({width}, {height})"
            );
        }

        /// <summary>
        /// Create a physics body from a CircleCollider2DComponent
        /// </summary>
        private void CreatePhysicsBody(
            RuntimeObj obj,
            CircleCollider2DComponent collider,
            Rigidbody2DComponent rigidbody
        )
        {
            FPVector2 position = new FPVector2(obj.Transform.Position.X, obj.Transform.Position.Y);
            FP rotation = obj.Transform.EulerAngles.Z;

            // Determine if static or dynamic
            bool isStatic = rigidbody == null || rigidbody.BodyType == Rigidbody2DType.Static;
            FP mass = rigidbody?.Mass ?? FP.One;

            // Create body
            RigidBodyLS body;
            if (isStatic)
            {
                body = RigidBodyLS.CreateStatic(0, position, rotation);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotation, mass);
            }

            // Apply scale to radius (use max of x/y scale)
            FP scaleX = FPMath.Abs(obj.Transform.LossyScale.X);
            FP scaleY = FPMath.Abs(obj.Transform.LossyScale.Y);
            FP radius = collider.Radius * FPMath.Max(scaleX, scaleY);
            body.SetCircleShape(radius);

            // Set material properties
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);

            // Add body to world and store mapping
            int bodyId = Sim.AddBody(body);
            runtimeIdToBodyId[obj.RuntimeId] = bodyId;

            Logger.Log(
                $"Created circle physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, Radius={radius}"
            );
        }

        /// <summary>
        /// Spawn a RuntimeObj dynamically at runtime and add it to the hierarchy.
        /// Components added via AddComponent will have their RuntimeObj reference set automatically.
        /// </summary>
        protected RuntimeObj SpawnRuntimeObj(string name, FPVector3 position)
        {
            var obj = new RuntimeObj
            {
                RuntimeId = GenerateRuntimeId(),
                Name = name,
                Children = new List<RuntimeObj>(),
                Transform = new FPTransform3D(position, FPQuaternion.Identity, FPVector3.One),
                GameComponents = new List<GameComponent>(),
            };

            // Add to root's children
            TileRoot.Children.Add(obj);

            return obj;
        }

        /// <summary>
        /// Add a physics body to an existing RuntimeObj
        /// </summary>
        protected void AddPhysicsBody(RuntimeObj obj)
        {
            ProcessComponents(obj);
        }

        public void Clear()
        {
            TileRoot = null;
            runtimeIdToBodyId.Clear();

            if (Sim != null)
            {
                Sim = new World();
                Sim.Gravity = FPVector2.FromFloats(0f, -9.81f);
            }
        }

        private void SyncPhysicsToRuntimeObjs()
        {
            if (TileRoot == null)
                return;

            SyncPhysicsRecursive(TileRoot);
        }

        private void SyncPhysicsRecursive(RuntimeObj runtimeObj)
        {
            if (runtimeIdToBodyId.TryGetValue(runtimeObj.RuntimeId, out int bodyId))
            {
                try
                {
                    var body = Sim.GetBody(bodyId);

                    FPVector3 newPosition = new FPVector3(
                        body.Position.X,
                        body.Position.Y,
                        runtimeObj.Transform.LocalPosition.Z
                    );
                    runtimeObj.Transform.LocalPosition = newPosition;

                    FP angle = body.Rotation;
                    runtimeObj.Transform.LocalRotation = FPQuaternion.AngleAxis(
                        angle,
                        FPVector3.Forward
                    );
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"Failed to sync physics for RuntimeId {runtimeObj.RuntimeId}: {e.Message}"
                    );
                }
            }

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
