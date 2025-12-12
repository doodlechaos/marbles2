using System;
using System.Collections.Generic;
using FPMathLib;
using LockSim;
using MemoryPack;

#nullable enable

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(GameTileBase))]
    [MemoryPackUnion(1, typeof(ThroneTile))]
    public abstract partial class TileBase
    {
        [MemoryPackOrder(0)]
        public GameCoreObj? TileRoot;

        [MemoryPackOrder(1)]
        public World Sim;

        [MemoryPackOrder(2), MemoryPackInclude]
        protected Dictionary<ulong, PhysicsBinding> physicsBindings = new();

        [MemoryPackOrder(3)]
        public byte TileWorldId;

        [MemoryPackOrder(4)]
        public ulong NextLocalId = 1;

        [MemoryPackOrder(5)]
        public GameCoreObj? PlayerMarbleTemplate;

        [MemoryPackOrder(6)]
        public ulong NextComponentId = 1;

        [MemoryPackIgnore]
        protected WorldSimulationContext simContext = new();

        [MemoryPackIgnore]
        protected Dictionary<int, ulong> bodyIdToRuntimeId = new();

        [MemoryPackIgnore]
        private List<MarbleComponent> pendingMarbleDestructions = new();

        [MemoryPackIgnore]
        protected OutputEventBuffer? currentOutputEvents;

        protected TileBase()
        {
            Sim = new World();
        }

        protected ulong GenerateRuntimeId()
        {
            return ((ulong)TileWorldId << 48) | (NextLocalId++ & 0xFFFFFFFFFFFF);
        }

        public virtual void Initialize(byte tileWorldId)
        {
            Logger.Log($"Initializing {GetType().Name} with TileId={tileWorldId}...");

            TileWorldId = tileWorldId;
            NextLocalId = 1;

            RefreshComponentIdCounter();

            physicsBindings.Clear();
            bodyIdToRuntimeId.Clear();
            simContext.Clear();
            Sim = new World { Gravity = FPVector2.FromFloats(0f, -9.81f) };

            if (TileRoot == null)
            {
                Logger.Error("TileRoot is null - cannot initialize");
                return;
            }

            AssignRuntimeIds(TileRoot);
            TileRoot.RebuildComponentReferences();
            TileRoot.SetupTransformHierarchy();
            RuntimePhysicsBuilder.BuildPhysics(TileRoot, Sim, physicsBindings);
            RebuildBodyIdLookup();
            OnLevelLoaded();

            Logger.Log(
                $"{GetType().Name} initialized successfully. Bodies in simulation: {Sim.Bodies.Count}"
            );
        }

        public void SetOutputEventsBufferReference(OutputEventBuffer outputEvents)
        {
            currentOutputEvents = outputEvents;
        }

        public virtual void Step()
        {
            simContext.Clear();

            StepComponents(TileRoot);

            PhysicsPipeline.Step(Sim, FP.FromFloat(1 / 60f), simContext);

            ProcessTriggerEvents();
            ProcessCollisionEvents();
            ProcessPendingMarbleDestructions();
            SyncPhysicsToRuntimeObjs();
        }

        private void StepComponents(GameCoreObj? obj)
        {
            if (obj == null)
                return;

            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    if (component.Enabled)
                        component.Step();
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                    StepComponents(child);
            }
        }

        protected virtual void OnLevelLoaded() { }

        protected virtual void OnAfterDeserialize() { }

        protected internal void AssignRuntimeIds(GameCoreObj obj)
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

        public void Clear()
        {
            TileRoot = null;
            physicsBindings.Clear();
            bodyIdToRuntimeId.Clear();
            simContext.Clear();

            Sim = new World { Gravity = FPVector2.FromFloats(0f, -9.81f) };
        }
    }
}
