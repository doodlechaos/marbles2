using System;
using System.Collections.Generic;
using FPMathLib;
using LockSim;
using MemoryPack;


#nullable enable

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
    public abstract partial class GameTileBase : TileBase
    {
        [MemoryPackOrder(7)]
        public GameTileState State = GameTileState.Spinning;

        [MemoryPackOrder(8), MemoryPackInclude]
        protected int stateSteps = 0;

        /// <summary>
        /// Number of simulation steps spent in the current state.
        /// </summary>
        [MemoryPackIgnore]
        public int StateSteps => stateSteps;

        /// <summary>
        /// Marbles queued for destruction this frame. Processed after all collision handling.
        /// </summary>
        [MemoryPackIgnore]
        private List<MarbleComponent> pendingMarbleDestructions = new();

        public GameTileBase()
            : base() { }

        public override void Initialize(byte tileWorldId)
        {
            // Set the state before calling base to ensure the event has the correct WorldId
            SetState(GameTileState.Spinning, null);
            base.Initialize(tileWorldId);
        }

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

        public override void Step(OutputEventBuffer outputEvents)
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

        /// <summary>
        /// Process trigger events from the last physics step.
        /// Handles TeleportWrap, MarbleDetector, and other trigger-based game logic.
        /// </summary>
        protected override void ProcessTriggerEvents()
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
        private void ProcessCollisionEvents()
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
        /// Handle TeleportWrap trigger collisions.
        /// When a dynamic body enters a TeleportWrap trigger, teleport it by the offset.
        /// </summary>
        private void ProcessTeleportWrapEvent(CollisionEvent evt)
        {
            // Find the RuntimeObjs involved
            GameCoreObj? objA = FindRuntimeObjByBodyId(evt.BodyIdA);
            GameCoreObj? objB = FindRuntimeObjByBodyId(evt.BodyIdB);

            if (objA == null || objB == null)
                return;

            // Check which one has the TeleportWrap component
            TeleportWrapComponent? teleportA = objA.GetComponent<TeleportWrapComponent>();
            TeleportWrapComponent? teleportB = objB.GetComponent<TeleportWrapComponent>();

            // Determine which is the teleporter and which is the target
            GameCoreObj? target = null;
            TeleportWrapComponent? teleportComponent = null;

            if (teleportA != null && teleportB == null)
            {
                target = objB;
                teleportComponent = teleportA;
            }
            else if (teleportB != null && teleportA == null)
            {
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
            FPMathLib.FPVector2 currentPhysicsPos = targetBody.Position;

            // Teleport the target by the offset, preserving velocity
            FPMathLib.FPVector3 newWorldPosition = new FPMathLib.FPVector3(
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

        /// <summary>
        /// Handle MarbleDetector collision/trigger events.
        /// When a marble collides with a detector, send signals to registered receivers.
        /// </summary>
        private void ProcessMarbleDetectorEvent(CollisionEvent evt, bool isTrigger, bool isEnter)
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
        private MarbleComponent? FindMarbleComponent(GameCoreObj obj)
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
            OnMarbleEliminated(marble);

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
        /// Called when a marble is eliminated. Override in derived classes to assign elimination order.
        /// </summary>
        protected virtual void OnMarbleEliminated(MarbleComponent marble) { }
    }
}
