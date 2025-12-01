using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// A simple battle royale game mode where players spawn through a pipe
    /// and fall/roll down a course. Last marble standing wins.
    /// </summary>
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
        public List<PlayerMarbleComponent> PlayerMarbles = new List<PlayerMarbleComponent>();

        /// <summary>
        /// Counter for assigning elimination order.
        /// Serialized so it continues correctly after deserialize.
        /// </summary>
        [MemoryPackOrder(7)] // Base class uses 0-6, derived must continue from 7
        public int NextEliminationOrder = 1;

        /// <summary>
        /// Queue of players waiting to spawn.
        /// </summary>
        [MemoryPackOrder(8)]
        public List<InputEvent.Entrant> SpawnQueue = new List<InputEvent.Entrant>();

        [MemoryPackOrder(9)]
        public int SpawnTickCounter = 0;

        [MemoryPackOrder(10)]
        public int TicksBetweenSpawns = 30;

        [MemoryPackOrder(11)]
        public uint TotalMarblesBid;

        public SimpleBattleRoyale() { }

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

            // Find all player marbles
            PlayerMarbles.Clear();
            TileRoot?.FindAllComponentsInChildren(PlayerMarbles);
        }

        public override void StartGameplay(InputEvent.Entrant[] entrants, uint totalMarblesBid)
        {
            TotalMarblesBid = totalMarblesBid;
            PlayerMarbles.Clear();
            SpawnQueue.Clear();
            SpawnTickCounter = 0;
            NextEliminationOrder = 1;

            foreach (var entrant in entrants)
                SpawnQueue.Add(entrant);

            // Transition to Gameplay state (controlled by server bidding logic)
            SetState(GameTileState.Gameplay);
        }

        public override void Step(OutputEventBuffer outputEvents)
        {
            ProcessSpawnQueue();
            CheckEliminations(outputEvents);
            base.Step(outputEvents);
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
            var marble = SpawnRuntimeObj(
                $"PlayerMarble_{entrant.AccountId}",
                SpawnPipe.Transform.Position
            );

            var playerComp = marble.AddComponent(
                new PlayerMarbleComponent
                {
                    AccountId = entrant.AccountId,
                    BidAmount = entrant.TotalBid,
                    IsAlive = true,
                    EliminationOrder = 0,
                }
            );

            marble.AddComponent(
                new CircleCollider2DComponent { Radius = FP.FromFloat(0.5f), IsTrigger = false }
            );

            marble.AddComponent(
                new Rigidbody2DComponent
                {
                    BodyType = Rigidbody2DType.Dynamic,
                    Mass = FP.One,
                    GravityScale = FP.One,
                }
            );

            AddPhysicsBody(marble);
            PlayerMarbles.Add(playerComp);
        }

        private void CheckEliminations(OutputEventBuffer outputEvents)
        {
            FP eliminationY = FP.FromFloat(-20f);

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
            }

            // Count survivors
            int aliveCount = 0;
            PlayerMarbleComponent lastAlive = null;
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
                outputEvents.Server.Add(
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
        public IEnumerable<PlayerMarbleComponent> GetSurvivors()
        {
            foreach (var p in PlayerMarbles)
                if (p.IsAlive)
                    yield return p;
        }

        /// <summary>
        /// Get eliminated players sorted by elimination order.
        /// </summary>
        public IEnumerable<PlayerMarbleComponent> GetEliminatedInOrder()
        {
            // Simple insertion sort since list is small
            var eliminated = new List<PlayerMarbleComponent>();
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
