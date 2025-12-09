using System;
using System.Collections.Generic;
using MemoryPack;

#nullable enable

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class GameCore
    {
        [MemoryPackOrder(0)]
        public ushort Seq;

        [MemoryPackOrder(1)]
        public ThroneTile? ThroneTile;

        [MemoryPackOrder(2)]
        public GameTileBase? GameTile1;

        [MemoryPackOrder(3)]
        public GameTileBase? GameTile2;

        [MemoryPackIgnore]
        public OutputEventBuffer OutputEvents = new();

        /// <summary>
        /// Called by MemoryPack after deserialization.
        /// Ensures OutputEvents is initialized since it's not serialized.
        /// </summary>
        [MemoryPackOnDeserialized]
        private void OnDeserialized()
        {
            OutputEvents ??= new OutputEventBuffer();
        }

        public OutputEventBuffer Step(List<InputEvent> inputEvents)
        {
            OutputEvents.Clear();
            HandleInputEvents(inputEvents);

            ThroneTile?.Step(OutputEvents);
            GameTile1?.Step(OutputEvents);
            GameTile2?.Step(OutputEvents);

            Seq = Seq.WrappingAdd(1);
            return OutputEvents;
        }

        private void HandleInputEvents(List<InputEvent> inputEvents)
        {
            foreach (InputEvent inputEvent in inputEvents)
            {
                Logger.Log($"[{Seq}] Stepping with input event: " + inputEvent.GetType().Name);
                if (inputEvent is InputEvent.SpinToNewGameTile spinLoadGameTile)
                {
                    byte worldId = spinLoadGameTile.WorldId;
                    OutputEvents.Client.Add(
                        new OutputToClientEvent.StartSpinAnimation { WorldId = worldId }
                    );
                    // Load the pre-deserialized GameTile into the appropriate slot
                    LoadGameTileIntoSlot(worldId, spinLoadGameTile.NewGameTile);
                }
                else if (inputEvent is InputEvent.LoadThroneTile loadThroneTile)
                {
                    LoadThroneTile(loadThroneTile.NewThroneTile);
                }
                else if (inputEvent is InputEvent.GameplayStartInput gameplayStartInput)
                {
                    byte worldId = gameplayStartInput.WorldId;
                    var gameTile = worldId == 1 ? GameTile1 : GameTile2;

                    if (gameTile != null)
                    {
                        gameTile.StartGameplay(gameplayStartInput, OutputEvents);
                    }
                }
                else if (inputEvent is InputEvent.FinishGameplay finishGameplay)
                {
                    byte worldId = finishGameplay.WorldId;
                    if (worldId == 1)
                        GameTile2?.SetState(GameTileState.Finished, OutputEvents);
                    else
                        GameTile1?.SetState(GameTileState.Finished, OutputEvents);
                }
                else if (inputEvent is InputEvent.SetKing setKing)
                {
                    ThroneTile?.SetKing(setKing.AccountId);
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

        /// <summary>
        /// Load a ThroneTile and initialize it.
        /// </summary>
        private void LoadThroneTile(ThroneTile throneTile)
        {
            if (throneTile == null)
            {
                Logger.Error("Cannot load null ThroneTile");
                return;
            }

            ThroneTile = throneTile;
            ThroneTile.Initialize(0); // ThroneTile uses world ID 0
        }

        public string GetDeterministicHashHex()
        {
            byte[] data = MemoryPackSerializer.Serialize(this);
            return data.GetDeterministicHashHex();
        }
    }
}
