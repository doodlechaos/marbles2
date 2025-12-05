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

        [MemoryPackOrder(6), MemoryPackInclude]
        protected int stateSteps = 0;

        /// <summary>
        /// Serialized template for the player marble RuntimeObj hierarchy.
        /// Built from the PlayerMarbleAuth prefab on the Unity side and cloned at runtime when spawning.
        /// Shared by all game tile types that spawn player marbles.
        /// </summary>
        [MemoryPackOrder(7)]
        public RuntimeObj PlayerMarbleTemplate;

        /// <summary>
        /// Number of simulation steps spent in the current state.
        /// </summary>
        [MemoryPackIgnore]
        public int StateSteps => stateSteps;

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

            // Set the tile ID BEFORE SetState so the event has the correct WorldId
            TileWorldId = tileWorldId;
            NextLocalId = 1;

            SetState(GameTileState.Spinning, null);

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
            RuntimePhysicsBuilder.BuildPhysics(TileRoot, Sim, runtimeIdToBodyId);

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

        public virtual void StartGameplay(
            InputEvent.GameplayStartInput gameplayStartInput,
            OutputEventBuffer outputEvents
        )
        {
            throw new NotImplementedException();
        }

        const float SPINNING_DURATION_SEC = 5.0f;
        const float OPENING_DOOR_DURATION_SEC = 2.0f;
        const float SCORE_SCREEN_DURATION_SEC = 5.0f;
        const float GAMEPLAY_MAX_DURATION_SEC = 60.0f;

        public virtual void Step(OutputEventBuffer outputEvents)
        {
            float stateDurationSec = stateSteps / 60.0f;
            if (State == GameTileState.Spinning)
            {
                if (stateDurationSec >= SPINNING_DURATION_SEC)
                    SetState(GameTileState.OpeningDoor, outputEvents);
            }
            else if (State == GameTileState.OpeningDoor)
            {
                if (stateDurationSec >= OPENING_DOOR_DURATION_SEC)
                    SetState(GameTileState.Bidding, outputEvents);
            }
            else if (State == GameTileState.Bidding)
            {
                // Bidding → Gameplay transition is controlled by the server via StartGameTile input event.
                // This ensures gameplay only starts when the other tile is ready for bidding
                // and minimum auction spots are filled.
            }
            else if (State == GameTileState.Gameplay)
            {
                if (stateDurationSec >= GAMEPLAY_MAX_DURATION_SEC)
                    SetState(GameTileState.ScoreScreen, outputEvents);
            }
            else if (State == GameTileState.ScoreScreen)
            {
                if (stateDurationSec >= SCORE_SCREEN_DURATION_SEC)
                    SetState(GameTileState.Finished, outputEvents);
            }

            PhysicsPipeline.Step(Sim, FP.FromFloat(1 / 60f), new WorldSimulationContext());
            SyncPhysicsToRuntimeObjs();
            stateSteps++;
        }

        public void SetState(GameTileState state, OutputEventBuffer? outputEvents)
        {
            outputEvents?.Server.Add(
                new OutputToServerEvent.StateUpdatedTo { State = state, WorldId = TileWorldId }
            );
            stateSteps = 0;
            State = state;
        }

        protected internal void AssignRuntimeIds(RuntimeObj obj)
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

        /// <summary>
        /// Deep-clone a RuntimeObj subtree for dynamic spawning.
        /// Uses MemoryPack to preserve all authored components, children, and transform data.
        /// New RuntimeIds are assigned separately via AssignRuntimeIds().
        /// </summary>
        protected RuntimeObj CloneRuntimeObjSubtree(RuntimeObj source)
        {
            if (source == null)
                return null;

            // Serialize + deserialize to get a deep copy of the object graph.
            // RuntimeIds will be overwritten after cloning via AssignRuntimeIds().
            var bytes = MemoryPack.MemoryPackSerializer.Serialize(source);
            return MemoryPack.MemoryPackSerializer.Deserialize<RuntimeObj>(bytes);
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
            RuntimePhysicsBuilder.AddPhysicsBody(obj, Sim, runtimeIdToBodyId);
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

                    // Sync position for all bodies
                    FPVector3 newPosition = new FPVector3(
                        body.Position.X,
                        body.Position.Y,
                        runtimeObj.Transform.LocalPosition.Z
                    );
                    runtimeObj.Transform.LocalPosition = newPosition;

                    // Only sync rotation for dynamic bodies.
                    // Static bodies can't rotate in physics, so preserve their original visual rotation.
                    if (body.BodyType == BodyType.Dynamic)
                    {
                        runtimeObj.Transform.LocalRotation = FPQuaternion.AngleAxis(
                            body.Rotation,
                            FPVector3.Forward
                        );
                    }
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
