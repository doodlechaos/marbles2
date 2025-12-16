using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// A simple battle royale game mode where players spawn through a pipe
    /// and fall/roll down a course. Last marble standing wins.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class SimpleBattleRoyale : GameTileBase
    {
        /// <summary>
        /// Reference to the spawn pipe component.
        /// Rebuilt from hierarchy after load/deserialize.
        /// </summary>
        [MemoryPackIgnore]
        public SpawnPipeComponent SpawnPipe;

        /// <summary>
        /// All player marbles in the game.
        /// Rebuilt from hierarchy after load/deserialize.
        /// Query IsAlive/EliminationOrder for filtering.
        /// </summary>
        [MemoryPackIgnore]
        public List<MarbleComponent> PlayerMarbles = new List<MarbleComponent>();

        /// <summary>
        /// Queue of players waiting to spawn.
        /// </summary>
        [MemoryPackOrder(14)]
        public List<InputEvent.Entrant> SpawnQueue = new List<InputEvent.Entrant>();

        [MemoryPackOrder(15)]
        public int SpawnTickCounter = 0;

        [MemoryPackOrder(16)]
        public int TicksBetweenSpawns = 30;

        public SimpleBattleRoyale() { }

        /// <summary>
        /// MemoryPack callback - required because union types don't call base class callbacks.
        /// </summary>
        [MemoryPackOnDeserialized]
        protected override void OnMemoryPackDeserialized()
        {
            HandleDeserialization();
        }

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
            if (SpawnPipe != null)
            {
                TicksBetweenSpawns = (int)(SpawnPipe.SpawnDelay.ToFloat() * 60);
            }

            // Find all player marbles (only alive ones after deserialize)
            PlayerMarbles.Clear();
            TileRoot?.FindAllComponentsInChildren(PlayerMarbles);
        }

        public override void StartGameplay(InputEvent.GameplayStartInput gameplayStartInput)
        {
            PlayerMarbles.Clear();
            SpawnQueue.Clear();
            SpawnTickCounter = 0;

            foreach (var entrant in gameplayStartInput.Entrants)
                SpawnQueue.Add(entrant);

            base.StartGameplay(gameplayStartInput);
        }

        public override void Step()
        {
            ProcessSpawnQueue();
            base.Step();
        }

        private void ProcessSpawnQueue()
        {
            if (SpawnQueue.Count == 0 || SpawnPipe == null)
                return;

            SpawnTickCounter++;
            if (SpawnTickCounter >= TicksBetweenSpawns)
            {
                SpawnTickCounter = 0;
                var entrant = SpawnQueue[0];
                SpawnQueue.RemoveAt(0);
                SpawnPlayerMarble(entrant);
            }
        }

        private void SpawnPlayerMarble(InputEvent.Entrant entrant)
        {
            if (SpawnPipe == null)
            {
                Logger.Error("SpawnPipe is null – cannot spawn player marble.");
                return;
            }

            // Use shared instantiation logic from TileBase
            MarbleComponent? playerComp = InstantiatePlayerMarble(
                entrant.AccountId,
                0,
                entrant.TotalBid
            );
            if (playerComp == null)
                return;

            // Position the marble at the spawn pipe
            playerComp.GCObj?.SetHierarchyWorldPos(
                SpawnPipe.Transform.Position,
                Sim,
                resetVelocity: false
            );
            //PositionMarbleAt(playerComp, SpawnPipe.Transform.Position);

            PlayerMarbles.Add(playerComp);
            Logger.Log($"Player {entrant.AccountId} spawned via template");
        }


    }
}
