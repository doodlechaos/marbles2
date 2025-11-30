using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class GameCore
    {
        [MemoryPackOrder(0)]
        public ushort Seq;

        [MemoryPackOrder(1)]
        public GameTileBase? GameTile1;

        [MemoryPackOrder(2)]
        public GameTileBase? GameTile2;

        [MemoryPackOrder(3)]
        public int BiddingWorldId;

        [MemoryPackIgnore]
        public readonly OutputEventBuffer OutputEvents = new();

        public OutputEventBuffer Step(List<InputEvent> inputEvents)
        {
            OutputEvents.Clear();
            HandleInputEvents(inputEvents);

            GameTile1?.Step(OutputEvents);
            GameTile2?.Step(OutputEvents);

            //TODO: Drain all the events from the gametiles

            Seq = Seq.WrappingAdd(1);
            return OutputEvents;
        }

        private void HandleInputEvents(List<InputEvent> inputEvents)
        {
            foreach (InputEvent inputEvent in inputEvents)
            {
                Logger.Log($"[{Seq}] Stepping with input event: " + inputEvent.GetType().Name);
                if (inputEvent is InputEvent.LoadGameTile loadGameTile)
                {
                    // Load the pre-deserialized GameTile into the appropriate slot
                    LoadGameTileIntoSlot(loadGameTile.WorldId, loadGameTile.GameTile);
                }
                else if (inputEvent is InputEvent.StartGameTile startGameTile)
                {
                    var gameTile = BiddingWorldId == 1 ? GameTile1 : GameTile2;

                    if (gameTile != null)
                    {
                        gameTile.StartGameplay(
                            startGameTile.Entrants,
                            startGameTile.TotalMarblesBid
                        );
                    }

                    // Toggle bidding world
                    BiddingWorldId = BiddingWorldId == 1 ? 2 : 1;
                }
            }
        }

        /// <summary>
        /// Load a GameTile template into the specified slot and initialize it.
        /// </summary>
        private void LoadGameTileIntoSlot(byte worldId, GameTileBase gameTile)
        {
            if (gameTile == null)
            {
                Logger.Error($"Cannot load null GameTile into slot {worldId}");
                return;
            }

            if (worldId == 1)
            {
                GameTile1 = gameTile;
                GameTile1.Initialize(1);
            }
            else if (worldId == 2)
            {
                GameTile2 = gameTile;
                GameTile2.Initialize(2);
            }
            else
            {
                Logger.Error($"Invalid world id: {worldId}");
            }
        }

        public string GetDeterministicHashHex()
        {
            byte[] data = MemoryPackSerializer.Serialize(this);
            return data.GetDeterministicHashHex();
        }
    }

    public abstract class OutputToClientEvent
    {
        public class NewKing : OutputToClientEvent
        {
            public ulong AccountId;
        }
    }

    public abstract class OutputToServerEvent
    {
        public class DeterminismHash : OutputToServerEvent
        {
            public ushort Seq;
            public string HashString;
        }

        public class AddPointsToAccount : OutputToServerEvent
        {
            public ulong AccountId;
            public uint Points;
        }

        public class NewKing : OutputToServerEvent
        {
            public ulong AccountId;
        }

        public class GameplayFinished : OutputToServerEvent { }
    }

    public sealed class OutputEventBuffer
    {
        public readonly List<OutputToClientEvent> Client = new();
        public readonly List<OutputToServerEvent> Server = new();

        public void Clear()
        {
            Client.Clear();
            Server.Clear();
        }
    }
}
