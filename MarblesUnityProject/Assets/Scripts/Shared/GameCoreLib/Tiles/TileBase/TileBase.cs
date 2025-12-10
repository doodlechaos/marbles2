/* using System;
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

        /// <summary>
        /// Maps RuntimeId to physics binding (body ID + base swing rotation).
        /// Uses stable IDs instead of object references, so it survives serialization.
        /// </summary>
        [MemoryPackOrder(2), MemoryPackInclude]
        protected Dictionary<ulong, PhysicsBinding> physicsBindings = new();

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

        /// <summary>
        /// Serialized template for the player marble RuntimeObj hierarchy.
        /// Built from the PlayerMarbleAuth prefab on the Unity side and cloned at runtime when spawning.
        /// Shared by all tile types that spawn player marbles.
        /// </summary>
        [MemoryPackOrder(5)]
        public GameCoreObj? PlayerMarbleTemplate;

        /// <summary>
        /// Counter for globally unique component IDs within this tile instance.
        /// Used to remap component references when cloning authored templates.
        /// </summary>
        [MemoryPackOrder(6)]
        public ulong NextComponentId = 1;

        /// <summary>
        /// Reusable simulation context for physics stepping.
        /// Contains collision/trigger events from the last step.
        /// </summary>
        [MemoryPackIgnore]
        protected WorldSimulationContext simContext = new();

        /// <summary>
        /// Reverse lookup: BodyId → RuntimeId.
        /// Rebuilt from physicsBindings when needed, cleared on Initialize.
        /// </summary>
        [MemoryPackIgnore]
        protected Dictionary<int, ulong> bodyIdToRuntimeId = new();

        /// <summary>
        /// Marbles queued for destruction this frame. Processed after all collision handling.
        /// </summary>
        [MemoryPackIgnore]
        private List<MarbleComponent> pendingMarbleDestructions = new();

        /// <summary>
        /// Current output event buffer for the frame. Set during Step() for use by signal receivers.
        /// </summary>
        [MemoryPackIgnore]
        protected OutputEventBuffer? currentOutputEvents;

        public TileBase()
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

            RefreshComponentIdCounter();

            // Let derived classes rebuild their game-specific references
            OnAfterDeserialize();
        }

        /// <summary>
        /// Override in derived classes to rebuild game-specific component references
        /// after deserialization. Called after RebuildComponentReferences().
        /// </summary>
        protected virtual void OnAfterDeserialize() { }

        /// <summary>
        /// Initialize the Tile for use in a specific tile slot.
        /// This assigns RuntimeIds, rebuilds component references, and sets up physics.
        /// Called after deserializing a Tile template from storage.
        /// </summary>
        /// <param name="tileWorldId">The tile slot ID for unique RuntimeId generation</param>
        public virtual void Initialize(byte tileWorldId)
        {
            Logger.Log($"Initializing {GetType().Name} with TileId={tileWorldId}...");

            TileWorldId = tileWorldId;
            NextLocalId = 1;

            RefreshComponentIdCounter();

            // Clear and recreate physics world
            physicsBindings.Clear();
            bodyIdToRuntimeId.Clear();
            simContext.Clear();
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

            // Set up transform parent-child relationships so Transform.Position
            // and Transform.LossyScale return correct world-space values
            TileRoot.SetupTransformHierarchy();

            // Process the hierarchy recursively to set up physics
            // Also stores base rotations (swing component) for gimbal-lock-free sync
            RuntimePhysicsBuilder.BuildPhysics(TileRoot, Sim, physicsBindings);

            // Build reverse lookup from BodyId to RuntimeId
            RebuildBodyIdLookup();

            // Post-load hook for derived classes
            OnLevelLoaded();

            Logger.Log(
                $"{GetType().Name} initialized successfully. Bodies in simulation: {Sim.Bodies.Count}"
            );
        }

        /// <summary>
        /// Override in derived classes to perform additional setup after level is loaded
        /// </summary>
        protected virtual void OnLevelLoaded() { }

        public void SetOutputEventsBufferReference(OutputEventBuffer outputEvents)
        {
            currentOutputEvents = outputEvents;
        }

        /// <summary>
        /// Step the physics simulation. Override in derived classes to add game-specific logic.
        /// </summary>
        public virtual void Step()
        {
            // Store output events for use by signal receivers during this frame
            //            currentOutputEvents = outputEvents;

            // Clear previous frame's events before stepping
            simContext.Clear();

            // Step physics with reusable context to capture collision/trigger events
            PhysicsPipeline.Step(Sim, FP.FromFloat(1 / 60f), simContext);

            // Process trigger events (e.g., TeleportWrap, MarbleDetector)
            ProcessTriggerEvents();

            // Process collision events (e.g., MarbleDetector for solid collisions)
            ProcessCollisionEvents();

            // Process any pending marble destructions after all collision processing
            ProcessPendingMarbleDestructions();

            SyncPhysicsToRuntimeObjs();
        }

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

        /// <summary>
        /// Deep-clone a RuntimeObj subtree for dynamic spawning.
        /// Uses MemoryPack to preserve all authored components, children, and transform data.
        /// New RuntimeIds are assigned separately via AssignRuntimeIds().
        /// </summary>
        protected GameCoreObj? CloneRuntimeObjSubtree(GameCoreObj? source)
        {
            if (source == null)
                return null;

            // Serialize + deserialize to get a deep copy of the object graph.
            // RuntimeIds will be overwritten after cloning via AssignRuntimeIds().
            var bytes = MemoryPack.MemoryPackSerializer.Serialize(source);
            var clone = MemoryPack.MemoryPackSerializer.Deserialize<GameCoreObj>(bytes);

            if (clone != null)
            {
                AssignComponentIdsForSpawn(clone);
            }

            return clone;
        }

        /// <summary>
        /// Assigns fresh component IDs to a spawned subtree and remaps any cross-component
        /// references to the new IDs.
        /// </summary>
        protected void AssignComponentIdsForSpawn(GameCoreObj? obj)
        {
            if (obj == null)
                return;

            var idRemap = new Dictionary<ulong, ulong>();
            AssignComponentIdsRecursive(obj, idRemap);
            RemapComponentIdReferences(obj, idRemap);
        }

        private void RefreshComponentIdCounter()
        {
            ulong maxComponentId = GetMaxComponentId(TileRoot);

            if (NextComponentId <= maxComponentId)
            {
                NextComponentId = maxComponentId + 1;
            }
        }

        private ulong GetMaxComponentId(GameCoreObj? obj)
        {
            if (obj == null)
                return 0;

            ulong maxId = 0;

            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    if (component.ComponentId > maxId)
                        maxId = component.ComponentId;
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ulong childMax = GetMaxComponentId(child);
                    if (childMax > maxId)
                        maxId = childMax;
                }
            }

            return maxId;
        }

        private void AssignComponentIdsRecursive(GameCoreObj obj, Dictionary<ulong, ulong> idRemap)
        {
            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    ulong originalId = component.ComponentId;
                    component.ComponentId = NextComponentId++;

                    if (originalId != 0)
                    {
                        idRemap[originalId] = component.ComponentId;
                    }
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    AssignComponentIdsRecursive(child, idRemap);
                }
            }
        }

        private void RemapComponentIdReferences(GameCoreObj obj, Dictionary<ulong, ulong> idRemap)
        {
            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    if (component is IGCComponentIdRemapper remapper)
                    {
                        remapper.RemapComponentIds(idRemap);
                    }
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    RemapComponentIdReferences(child, idRemap);
                }
            }
        }

        /// <summary>
        /// Spawn a RuntimeObj dynamically at runtime and add it to the hierarchy.
        /// Components added via AddComponent will have their RuntimeObj reference set automatically.
        /// </summary>
        protected GameCoreObj SpawnRuntimeObj(string name, FPVector3 position)
        {
            var obj = new GameCoreObj
            {
                RuntimeId = GenerateRuntimeId(),
                Name = name,
                Children = new List<GameCoreObj>(),
                Transform = new FPTransform3D(position, FPQuaternion.Identity, FPVector3.One),
                GameComponents = new List<GCComponent>(),
            };

            // Add to root's children
            TileRoot.Children.Add(obj);

            return obj;
        }

        /// <summary>
        /// Add a physics body to an existing RuntimeObj.
        /// After calling this, use obj.SetWorldPos() or obj.SetHierarchyWorldPos() to move the object.
        /// </summary>
        protected void AddPhysicsBody(GameCoreObj obj)
        {
            RuntimePhysicsBuilder.AddPhysicsBody(obj, Sim, physicsBindings);
            // Update reverse lookup for the new body
            RebuildBodyIdLookup();
        }

        /// <summary>
        /// Rebuild the BodyId → RuntimeId reverse lookup from physicsBindings.
        /// Called after physics setup or when bodies are added/removed.
        /// </summary>
        protected void RebuildBodyIdLookup()
        {
            bodyIdToRuntimeId.Clear();
            foreach (var kvp in physicsBindings)
            {
                bodyIdToRuntimeId[kvp.Value.BodyId] = kvp.Key;
            }
        }

        /// <summary>
        /// Find a RuntimeObj by its physics body ID.
        /// Returns null if the body ID is not associated with any RuntimeObj.
        /// </summary>
        protected GameCoreObj FindRuntimeObjByBodyId(int bodyId)
        {
            if (bodyIdToRuntimeId.TryGetValue(bodyId, out ulong runtimeId))
            {
                return TileRoot?.FindByRuntimeId(runtimeId);
            }
            return null;
        }

        /// <summary>
        /// Process trigger events from the last physics step.
        /// Handles TeleportWrap, MarbleDetector, and other trigger-based game logic.
        /// Override in derived classes to add game-specific trigger handling.
        /// </summary>
        protected virtual void ProcessTriggerEvents()
        {
            foreach (var evt in simContext.TriggerEvents)
            {
                bool isEnter = evt.EventType == CollisionEventType.Enter;
                bool isStay = evt.EventType == CollisionEventType.Stay;

                if (isEnter)
                {
                    ProcessTeleportWrapEvent(evt);
                }

                // Process marble detection for triggers
                if (isEnter || isStay)
                {
                    ProcessMarbleDetectorEvent(evt, isTrigger: true, isEnter: isEnter);
                }
            }
        }

        /// <summary>
        /// Process collision events (solid colliders) from the last physics step.
        /// </summary>
        protected virtual void ProcessCollisionEvents()
        {
            foreach (var evt in simContext.CollisionEvents)
            {
                bool isEnter = evt.EventType == CollisionEventType.Enter;
                bool isStay = evt.EventType == CollisionEventType.Stay;

                // Process marble detection for solid collisions
                if (isEnter || isStay)
                {
                    ProcessMarbleDetectorEvent(evt, isTrigger: false, isEnter: isEnter);
                }
            }
        }

        /// <summary>
        /// Handle MarbleDetector collision/trigger events.
        /// When a marble collides with a detector, send signals to registered receivers.
        /// </summary>
        protected void ProcessMarbleDetectorEvent(CollisionEvent evt, bool isTrigger, bool isEnter)
        {
            GameCoreObj? objA = FindRuntimeObjByBodyId(evt.BodyIdA);
            GameCoreObj? objB = FindRuntimeObjByBodyId(evt.BodyIdB);

            if (objA == null || objB == null)
                return;

            // Check which one has the detector and which has the marble
            MarbleDetectorComponent? detectorA = objA.GetComponent<MarbleDetectorComponent>();
            MarbleDetectorComponent? detectorB = objB.GetComponent<MarbleDetectorComponent>();

            // Find marble - could be on the object itself or its parent (marble root)
            MarbleComponent? marbleA = FindMarbleComponent(objA);
            MarbleComponent? marbleB = FindMarbleComponent(objB);

            // Process detector A with marble B
            if (detectorA != null && marbleB != null)
            {
                if (ShouldProcessDetection(detectorA, isTrigger, isEnter))
                {
                    detectorA.SendSignal(marbleB, this);
                }
            }

            // Process detector B with marble A
            if (detectorB != null && marbleA != null)
            {
                if (ShouldProcessDetection(detectorB, isTrigger, isEnter))
                {
                    detectorB.SendSignal(marbleA, this);
                }
            }
        }

        /// <summary>
        /// Find the MarbleComponent for an object. Checks the object and its parent hierarchy.
        /// Marbles have their rigidbody on a child object, so we need to search up the tree.
        /// </summary>
        protected MarbleComponent? FindMarbleComponent(GameCoreObj obj)
        {
            var marble = obj.GetComponent<MarbleComponent>();
            if (marble != null)
                return marble;

            // Search parent hierarchy (for cases where rigidbody is on child)
            return TileRoot?.FindComponentByPredicate<MarbleComponent>(m =>
                m.RigidbodyRuntimeObj == obj
            );
        }

        /// <summary>
        /// Check if the detector's settings allow this type of detection.
        /// </summary>
        private static bool ShouldProcessDetection(
            MarbleDetectorComponent detector,
            bool isTrigger,
            bool isEnter
        )
        {
            if (isTrigger)
            {
                return isEnter ? detector.TriggerEnterDetection : detector.TriggerStayDetection;
            }
            else
            {
                return isEnter ? detector.CollisionEnterDetection : detector.CollisionStayDetection;
            }
        }

        /// <summary>
        /// Queue a marble for destruction. Called by MarbleEffectComponent with Explode effect.
        /// The marble is destroyed after all collision processing is complete for this frame.
        /// </summary>
        public void ExplodeMarble(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            // Prevent duplicate queuing
            if (!pendingMarbleDestructions.Contains(marble))
            {
                pendingMarbleDestructions.Add(marble);
            }
        }

        /// <summary>
        /// Process all pending marble destructions after collision handling is complete.
        /// </summary>
        private void ProcessPendingMarbleDestructions()
        {
            foreach (var marble in pendingMarbleDestructions)
            {
                DestroyMarble(marble);
            }
            pendingMarbleDestructions.Clear();
        }

        /// <summary>
        /// Destroy a marble and remove it from the game.
        /// Marks the player as eliminated and removes the physics body and hierarchy.
        /// </summary>
        protected virtual void DestroyMarble(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            // Mark as eliminated
            marble.IsAlive = false;

            // Find the marble's root RuntimeObj (the one with MarbleComponent, not the rigidbody child)
            GameCoreObj? marbleRoot = marble.GCObj;
            if (marbleRoot == null)
            {
                Logger.Error($"Could not find RuntimeObj for marble {marble.AccountId}");
                return;
            }

            Logger.Log($"Destroying marble: {marbleRoot.Name} (AccountId: {marble.AccountId})");

            // Remove physics bodies for the entire marble hierarchy
            RemovePhysicsHierarchy(marbleRoot);

            // Remove from parent's children list
            if (TileRoot?.Children != null)
            {
                TileRoot.Children.Remove(marbleRoot);
            }
        }

        /// <summary>
        /// Recursively remove physics bodies for an entire hierarchy.
        /// </summary>
        protected void RemovePhysicsHierarchy(GameCoreObj obj)
        {
            if (obj == null)
                return;

            // Remove physics body if present
            if (physicsBindings.TryGetValue(obj.RuntimeId, out PhysicsBinding binding))
            {
                Sim.RemoveBody(binding.BodyId);
                physicsBindings.Remove(obj.RuntimeId);
                bodyIdToRuntimeId.Remove(binding.BodyId);
                obj.PhysicsBodyId = -1;
            }

            // Recursively process children
            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    RemovePhysicsHierarchy(child);
                }
            }
        }

        /// <summary>
        /// Handle TeleportWrap trigger collisions.
        /// When a dynamic body enters a TeleportWrap trigger, teleport it by the offset.
        /// </summary>
        private void ProcessTeleportWrapEvent(CollisionEvent evt)
        {
            // Find the RuntimeObjs involved
            GameCoreObj objA = FindRuntimeObjByBodyId(evt.BodyIdA);
            GameCoreObj objB = FindRuntimeObjByBodyId(evt.BodyIdB);

            if (objA == null || objB == null)
                return;

            // Check which one has the TeleportWrap component
            TeleportWrapComponent teleportA = objA.GetComponent<TeleportWrapComponent>();
            TeleportWrapComponent teleportB = objB.GetComponent<TeleportWrapComponent>();

            // Determine which is the teleporter and which is the target
            GameCoreObj teleporter = null;
            GameCoreObj target = null;
            TeleportWrapComponent teleportComponent = null;

            if (teleportA != null && teleportB == null)
            {
                teleporter = objA;
                target = objB;
                teleportComponent = teleportA;
            }
            else if (teleportB != null && teleportA == null)
            {
                teleporter = objB;
                target = objA;
                teleportComponent = teleportB;
            }
            else
            {
                // Both have TeleportWrap or neither does - ignore
                return;
            }

            // Only teleport dynamic bodies (not static/kinematic)
            if (!target.HasPhysicsBody)
                return;

            if (!Sim.TryGetBody(target.PhysicsBodyId, out RigidBodyLS targetBody))
                return;

            if (targetBody.BodyType != BodyType.Dynamic)
                return;

            // Get the current world position from the physics body itself.
            // We cannot rely on target.Transform.Position because RuntimeObj transforms
            // don't have parent relationships set up, so Position returns LocalPosition
            // which may be relative to a parent that has moved.
            FPVector2 currentPhysicsPos = targetBody.Position;

            // Teleport the target by the offset, preserving velocity
            FPVector3 newWorldPosition = new FPVector3(
                currentPhysicsPos.X + teleportComponent.Offset.X,
                currentPhysicsPos.Y + teleportComponent.Offset.Y,
                target.Transform.LocalPosition.Z
            );

            // Use SetWorldPos with resetVelocity=false to preserve momentum
            target.SetWorldPos(newWorldPosition, Sim, resetVelocity: false);

            Logger.Log(
                $"TeleportWrap: Teleported '{target.Name}' from ({currentPhysicsPos.X}, {currentPhysicsPos.Y}) by offset ({teleportComponent.Offset.X}, {teleportComponent.Offset.Y})"
            );
        }

        public void Clear()
        {
            TileRoot = null;
            physicsBindings.Clear();
            bodyIdToRuntimeId.Clear();
            simContext.Clear();

            if (Sim != null)
            {
                Sim = new World();
                Sim.Gravity = FPVector2.FromFloats(0f, -9.81f);
            }
        }

        protected void SyncPhysicsToRuntimeObjs()
        {
            if (TileRoot == null)
                return;

            // Start with zero parent offset (TileRoot is the origin of the tile-local coordinate space)
            SyncPhysicsRecursive(TileRoot, FPVector3.Zero);
        }

        private void SyncPhysicsRecursive(GameCoreObj runtimeObj, FPVector3 parentWorldPos)
        {
            // Compute this object's current world position (before physics sync)
            // This is needed to properly compute child world positions
            FPVector3 currentWorldPos = parentWorldPos + runtimeObj.Transform.LocalPosition;

            if (physicsBindings.TryGetValue(runtimeObj.RuntimeId, out PhysicsBinding binding))
            {
                try
                {
                    var body = Sim.GetBody(binding.BodyId);

                    // Physics is 2D, so only X and Y are tracked by the physics body.
                    // We compute local X,Y from physics world position and parent world position.
                    // Z is not affected by physics, so we preserve the original LocalPosition.Z.
                    FP localX = body.Position.X - parentWorldPos.X;
                    FP localY = body.Position.Y - parentWorldPos.Y;
                    FP localZ = runtimeObj.Transform.LocalPosition.Z; // Preserve original Z

                    runtimeObj.Transform.LocalPosition = new FPVector3(localX, localY, localZ);

                    // Update current world position to match physics (for children to use)
                    // Note: World Z uses parent's world Z + local Z since physics doesn't track Z
                    currentWorldPos = new FPVector3(
                        body.Position.X,
                        body.Position.Y,
                        parentWorldPos.Z + localZ
                    );

                    // Only sync rotation for dynamic bodies.
                    // Static bodies can't rotate in physics, so preserve their original visual rotation.
                    if (body.BodyType == BodyType.Dynamic)
                    {
                        // Use quaternion math to avoid gimbal lock at 90° X rotation.
                        // The base rotation (swing) contains the X/Y tilt without Z rotation.
                        // We apply the physics Z rotation (twist) to this base.
                        // Create twist quaternion from physics Z rotation (in radians)
                        FP physicsZDegrees = body.Rotation * FP.Rad2Deg;
                        FPQuaternion physicsTwist = FPQuaternion.AngleAxis(
                            physicsZDegrees,
                            FPVector3.Forward
                        );

                        // Combine: twist first (world Z rotation), then swing (visual tilt)
                        // This preserves the X/Y tilt while applying physics Z rotation
                        runtimeObj.Transform.LocalRotation = physicsTwist * binding.BaseSwing;
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
                    SyncPhysicsRecursive(child, currentWorldPos);
                }
            }
        }
    }
}
 */