using System;
using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

#nullable enable

namespace GameCoreLib
{
    /// <summary>
    /// The ThroneTile displays the current king and throne room environment.
    /// Handles throne capture when marbles collide with the king body.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class ThroneTile : TileBase
    {
        /// <summary>
        /// Reference to the spawn pipe component for attack marble spawning.
        /// Rebuilt from hierarchy after load/deserialize.
        /// </summary>
        [MemoryPackIgnore]
        public SpawnPipeComponent? SpawnPipe;

        /// <summary>
        /// Active attack marbles currently in the throne room.
        /// </summary>
        [MemoryPackIgnore]
        public List<MarbleComponent> AttackMarbles = new List<MarbleComponent>();

        public ThroneTile()
            : base() { }

        protected override void OnLevelLoaded()
        {
            RebuildReferences();
        }

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            RebuildReferences();
        }

        private void RebuildReferences()
        {
            // Find spawn pipe
            SpawnPipe = TileRoot?.FindComponentInChildren<SpawnPipeComponent>();

            // Find all active attack marbles
            AttackMarbles.Clear();
            TileRoot?.FindAllComponentsInChildren(AttackMarbles);
        }

        /// <summary>
        /// Crown a new king when a marble captures the throne.
        /// Explodes the capturing marble and sends the NewKing event to the server.
        /// </summary>
        public void CrownNewKing(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            ulong accountId = marble.AccountId;

            Logger.Log($"New king crowned: {accountId}");

            // Explode the capturing marble
            ExplodeMarble(marble);

            SetKingServerId(accountId);
        }

        public void SetKingServerId(ulong accountId)
        {
            // Send the NewKing event to the server
            if (currentOutputEvents != null)
            {
                currentOutputEvents.Server.Add(
                    new OutputToServerEvent.NewKing { AccountId = accountId }
                );
            }
        }

        /// <summary>
        /// Spawn an attack marble for the given account that falls upward toward the king.
        /// Called from GameCore input events when a player attacks the throne.
        /// </summary>
        public void SpawnAttackMarble(ulong accountId)
        {
            if (SpawnPipe == null)
            {
                Logger.Error("SpawnPipe is null – cannot spawn attack marble.");
                return;
            }

            // Use shared instantiation logic from TileBase
            var marbleComp = InstantiatePlayerMarble(accountId);
            if (marbleComp == null)
                return;

            // Set gravity scale to -1 so the marble falls upward toward the king
            SetMarbleGravityScale(marbleComp, FP.FromInt(-1));

            // Position the marble at the spawn pipe
            marbleComp.GCObj?.SetHierarchyWorldPos(
                SpawnPipe.Transform.Position,
                Sim,
                resetVelocity: false
            );
            //PositionMarbleAt(marbleComp, SpawnPipe.Transform.Position);

            // Track this attack marble
            AttackMarbles.Add(marbleComp);

            Logger.Log($"Attack marble spawned for account {accountId} at spawn pipe");
        }

        /// <summary>
        /// Set the gravity scale for a marble's physics body. TODO, could we move this method into the marblecomponent script?
        /// </summary>
        private void SetMarbleGravityScale(MarbleComponent marble, FP gravityScale)
        {
            if (marble?.RigidbodyRuntimeObj == null)
                return;

            var targetObj = marble.RigidbodyRuntimeObj;
            if (!targetObj.HasPhysicsBody)
                return;

            if (Sim.TryGetBody(targetObj.PhysicsBodyId, out var body))
            {
                body.GravityScale = gravityScale;
                Sim.SetBody(targetObj.PhysicsBodyId, body);
            }
        }

        /// <summary>
        /// Called when a marble is destroyed (e.g., by explosion).
        /// Removes it from the attack marbles list.
        /// </summary>
        protected override void DestroyMarble(MarbleComponent marble)
        {
            AttackMarbles.Remove(marble);
            base.DestroyMarble(marble);
        }
    }
}
