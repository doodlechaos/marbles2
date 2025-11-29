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
        public GameTile GameTile1 = new GameTile(1);

        [MemoryPackOrder(2)]
        public GameTile GameTile2 = new GameTile(2);

        [MemoryPackOrder(3)]
        public int BiddingWorldId;

        /// <summary>
        /// Counter for assigning unique RuntimeIds to RuntimeObj instances.
        /// Ensures deterministic, stable IDs across the entire GameCore.
        /// </summary>
        [MemoryPackOrder(4)]
        public ulong NextRuntimeId = 1;

        [MemoryPackIgnore]
        public readonly OutputEventBuffer OutputEvents = new();

        public OutputEventBuffer Step(List<InputEvent> inputEvents)
        {
            OutputEvents.Clear();
            HandleInputEvents(inputEvents);

            GameTile1.Step(OutputEvents);
            GameTile2.Step(OutputEvents);

            //Drain all the events from the gametiles

            Seq = Seq.WrappingAdd(1);
            return OutputEvents;
        }

        private void HandleInputEvents(List<InputEvent> inputEvents)
        {
            foreach (InputEvent inputEvent in inputEvents)
            {
                Logger.Log($"[{Seq}] Stepping with input event: " + inputEvent.GetType().Name);
                if (inputEvent is InputEvent.LoadLevelFile loadTileFile)
                {
                    if (loadTileFile.WorldId == 1)
                    {
                        GameTile1.Load(loadTileFile.LevelFile, this);
                    }
                    else if (loadTileFile.WorldId == 2)
                    {
                        GameTile2.Load(loadTileFile.LevelFile, this);
                    }
                    else
                    {
                        Logger.Error("Invalid world id: " + loadTileFile.WorldId);
                    }
                }
                else if (inputEvent is InputEvent.StartGameTile startGameTile)
                {
                    //TODO: Pass the data to the correct gametile
                }
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
        public class NewKing : OutputToClientEvent //I might not need this, if the client is just rendering hte gamecore state now
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
