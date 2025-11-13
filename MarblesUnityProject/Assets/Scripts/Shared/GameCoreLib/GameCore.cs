using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class GameCore
    {
        [MemoryPackOrder(0)]
        public ushort Seq;

        [MemoryPackOrder(1)]
        public GameTile GameTile1 = new GameTile(1);

        [MemoryPackOrder(2)]
        public GameTile GameTile2 = new GameTile(2);

        /// <summary>
        /// Counter for assigning unique RuntimeIds to RuntimeObj instances.
        /// Ensures deterministic, stable IDs across the entire GameCore.
        /// </summary>
        [MemoryPackOrder(3)]
        public ulong NextRuntimeId = 1;

        public void Step(List<InputEvent> inputEvents)
        {
            HandleInputEvents(inputEvents);

            GameTile1.Step();
            GameTile2.Step();

            Seq = Seq.WrappingAdd(1);
        }

        private void HandleInputEvents(List<InputEvent> inputEvents)
        {
            foreach (InputEvent inputEvent in inputEvents)
            {
                Logger.Log($"[{Seq}] Stepping with input event: " + inputEvent.GetType().Name);
                if (inputEvent is InputEvent.LoadTileFile loadTileFile)
                {
                    if (loadTileFile.WorldId == 1)
                    {
                        GameTile1.Load(loadTileFile.HydratedNextTileJson, this);
                    }
                    else if (loadTileFile.WorldId == 2)
                    {
                        GameTile2.Load(loadTileFile.HydratedNextTileJson, this);
                    }
                    else
                    {
                        Logger.Error("Invalid world id: " + loadTileFile.WorldId);
                    }
                }
            }
        }

        public string GetDeterministicHashHex()
        {
            byte[] data = MemoryPackSerializer.Serialize(this);
            return data.GetDeterministicHashHex();
        }
    }
}
