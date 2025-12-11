using System;
using System.Collections.Generic;
using FPMathLib;
using MemoryPack;
using UnityEditor.Build;

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
        /// Counter for assigning elimination order.
        /// Serialized so it continues correctly after deserialize.
        /// </summary>
        [MemoryPackOrder(9)] // TileBase uses 0-6, GameTileBase uses 7-8, derived starts at 9
        public int NextEliminationOrder = 1;

        /// <summary>
        /// Queue of players waiting to spawn.
        /// </summary>
        [MemoryPackOrder(10)]
        public List<InputEvent.Entrant> SpawnQueue = new List<InputEvent.Entrant>();

        [MemoryPackOrder(11)]
        public int SpawnTickCounter = 0;

        [MemoryPackOrder(12)]
        public int TicksBetweenSpawns = 30;

        [MemoryPackOrder(13)]
        public uint TotalMarblesBid;

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

        /// <summary>
        /// Called when a marble is destroyed (e.g., by Explode effect).
        /// Assigns elimination order and removes from active player list.
        /// </summary>
        protected override void OnMarbleEliminated(MarbleComponent marble)
        {
            marble.EliminationOrder = NextEliminationOrder++;
            Logger.Log($"Player {marble.AccountId} eliminated (order: {marble.EliminationOrder})");

            // Remove from active player list
            PlayerMarbles.Remove(marble);
        }

        public override void StartGameplay(InputEvent.GameplayStartInput gameplayStartInput)
        {
            TotalMarblesBid = gameplayStartInput.TotalMarblesBid;
            PlayerMarbles.Clear();
            SpawnQueue.Clear();
            SpawnTickCounter = 0;
            NextEliminationOrder = 1;

            foreach (var entrant in gameplayStartInput.Entrants)
                SpawnQueue.Add(entrant);

            // Transition to Gameplay state (controlled by server bidding logic)
            SetState(GameTileState.Gameplay);
        }

        public override void Step()
        {
            ProcessSpawnQueue();
            CheckEliminations();
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

        private void CheckEliminations()
        {
            /*             FP eliminationY = FP.FromFloat(-20f);
            
                        foreach (var player in PlayerMarbles)
                        {
                            if (player.IsAlive && player.Transform.Position.Y < eliminationY)
                            {
                                player.IsAlive = false;
                                player.EliminationOrder = NextEliminationOrder++;
                                Logger.Log(
                                    $"Player {player.AccountId} eliminated (order: {player.EliminationOrder})"
                                );
                            }
                        } */

            // Count survivors
            int aliveCount = 0;
            MarbleComponent lastAlive = null;
            foreach (var player in PlayerMarbles)
            {
                if (player.IsAlive)
                {
                    aliveCount++;
                    lastAlive = player;
                }
            }

            // Check for winner
            if (aliveCount == 1 && SpawnQueue.Count == 0 && PlayerMarbles.Count > 1)
            {
                Logger.Log($"Player {lastAlive.AccountId} wins!");
                currentOutputEvents?.Server.Add(
                    new OutputToServerEvent.NewKing { AccountId = lastAlive.AccountId }
                );
            }
            else if (aliveCount == 0 && SpawnQueue.Count == 0 && PlayerMarbles.Count > 0)
            {
                Logger.Log("All players eliminated!");
            }
        }

        #region Query Helpers

        /// <summary>
        /// Get all surviving players.
        /// </summary>
        public IEnumerable<MarbleComponent> GetSurvivors()
        {
            foreach (var p in PlayerMarbles)
                if (p.IsAlive)
                    yield return p;
        }

        /// <summary>
        /// Get eliminated players sorted by elimination order.
        /// </summary>
        public IEnumerable<MarbleComponent> GetEliminatedInOrder()
        {
            // Simple insertion sort since list is small
            var eliminated = new List<MarbleComponent>();
            foreach (var p in PlayerMarbles)
            {
                if (!p.IsAlive)
                    eliminated.Add(p);
            }
            eliminated.Sort((a, b) => a.EliminationOrder.CompareTo(b.EliminationOrder));
            return eliminated;
        }

        /// <summary>
        /// Get final rank for a player (1 = winner, higher = worse).
        /// </summary>
        public int GetFinalRank(ulong accountId)
        {
            foreach (var p in PlayerMarbles)
            {
                if (p.AccountId == accountId)
                {
                    if (p.IsAlive)
                        return 1; // Survivor
                    // Eliminated: invert order (last eliminated = rank 2, first = worst)
                    return PlayerMarbles.Count - p.EliminationOrder + 1;
                }
            }
            return -1;
        }

        #endregion
    }
}
