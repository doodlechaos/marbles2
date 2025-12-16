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
            ThroneTile?.SetOutputEventsBufferReference(OutputEvents);
            GameTile1?.SetOutputEventsBufferReference(OutputEvents);
            GameTile2?.SetOutputEventsBufferReference(OutputEvents);

            HandleInputEvents(inputEvents);

            ThroneTile?.Step();
            GameTile1?.Step();
            GameTile2?.Step();

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
                    OutputEvents.Events.Add(
                        new OutputEvent.StartSpinAnimation { WorldId = worldId }
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
                        gameTile.StartGameplay(gameplayStartInput);
                    }
                }
                else if (inputEvent is InputEvent.ForceFinishGameplay finishGameplay)
                {
                    byte worldId = finishGameplay.WorldId;
                    if (worldId == 1)
                        GameTile2?.FinishGameplay();
                    else
                        GameTile1?.FinishGameplay();
                }
                else if (inputEvent is InputEvent.SetKing setKing) //This wouldn't happen from regular gameplay, it would be an admin action to force setting a king manually
                {
                    ThroneTile?.SetKingServerId(setKing.AccountId);
                }
                else if (inputEvent is InputEvent.Attack attack)
                {
                    ThroneTile?.SpawnAttackMarble(attack.AccountId, (int)attack.Points);
                }
                else if (inputEvent is InputEvent.Dhash dhash)
                {
                    string hashString = GetDeterministicHashHex();
                    OutputEvents.Events.Add(
                        new OutputEvent.DeterminismHash { Seq = Seq, HashString = hashString }
                    );
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
                GameTile1.InitTile(1);
            }
            else if (worldId == 2)
            {
                GameTile2 = gameTile;
                GameTile2.InitTile(2);
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
            ThroneTile.InitTile(0); // ThroneTile uses world ID 0
        }

        public string GetDeterministicHashHex()
        {
            byte[] data = MemoryPackSerializer.Serialize(this);
            return data.GetDeterministicHashHex();
        }
    }
}
