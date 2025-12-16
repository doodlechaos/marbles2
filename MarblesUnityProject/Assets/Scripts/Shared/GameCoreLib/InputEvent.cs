// InputEvent.cs
using System;
using MemoryPack;

namespace GameCoreLib
{
    // Base of the union
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(Attack))]
    [MemoryPackUnion(1, typeof(SpawnMarble))]
    [MemoryPackUnion(2, typeof(Dhash))]
    [MemoryPackUnion(3, typeof(SetIntegrationParameter))]
    [MemoryPackUnion(4, typeof(StartCloseDoorAnimation))]
    [MemoryPackUnion(5, typeof(SpinToNewGameTile))]
    [MemoryPackUnion(6, typeof(GameplayStartInput))]
    [MemoryPackUnion(7, typeof(LoadThroneTile))]
    [MemoryPackUnion(8, typeof(SetKing))]
    [Serializable]
    public abstract partial class InputEvent
    {
        protected InputEvent() { }

        [MemoryPackable(SerializeLayout.Explicit)]
        [Serializable]
        public partial class GameplayStartInput : InputEvent
        {
            [MemoryPackOrder(0)]
            public Entrant[] Entrants = Array.Empty<Entrant>();

            [MemoryPackOrder(1)]
            public uint TotalMarblesBid;

            [MemoryPackOrder(2)]
            public byte WorldId; // u8

            [MemoryPackConstructor]
            public GameplayStartInput() { }

            public GameplayStartInput(Entrant[] entrants, uint totalMarblesBid, byte worldId)
            {
                Entrants = entrants;
                TotalMarblesBid = totalMarblesBid;
                WorldId = worldId;
            }
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class ForceFinishGameplay : InputEvent
        {
            [MemoryPackOrder(0)]
            public byte WorldId; // u8

            [MemoryPackConstructor]
            public ForceFinishGameplay() { }
        }

        [MemoryPackable]
        [Serializable]
        public partial class Entrant
        {
            public ulong AccountId;
            public uint TotalBid;
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class Attack : InputEvent
        {
            [MemoryPackOrder(0)]
            public ulong AccountId; // u64

            [MemoryPackOrder(1)]
            public uint Points; // u32

            [MemoryPackConstructor]
            public Attack() { }

            public Attack(ulong accountId, uint points)
            {
                AccountId = accountId;
                Points = points;
            }
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class SpawnMarble : InputEvent
        {
            [MemoryPackOrder(0)]
            public byte WorldId; // u8

            [MemoryPackOrder(1)]
            public ulong AccountId; // u64

            [MemoryPackOrder(2)]
            public ulong SpawnerRuntimeId; // u64

            [MemoryPackConstructor]
            public SpawnMarble() { }

            public SpawnMarble(byte worldId, ulong accountId, ulong spawnerRuntimeId)
            {
                WorldId = worldId;
                AccountId = accountId;
                SpawnerRuntimeId = spawnerRuntimeId;
            }
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class Dhash : InputEvent
        {
            public Dhash() { }
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class SetIntegrationParameter : InputEvent
        {
            [MemoryPackOrder(0)]
            public string Value = string.Empty;

            [MemoryPackConstructor]
            public SetIntegrationParameter() { }

            public SetIntegrationParameter(string value) => Value = value;
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class StartCloseDoorAnimation : InputEvent
        {
            [MemoryPackOrder(0)]
            public byte WorldId; // u8

            [MemoryPackConstructor]
            public StartCloseDoorAnimation() { }

            public StartCloseDoorAnimation(byte worldId) => WorldId = worldId;
        }

        /// <summary>
        /// Load a pre-serialized GameTile template into a world slot.
        /// The GameTile is deserialized and initialized with the appropriate TileId.
        /// </summary>
        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class SpinToNewGameTile : InputEvent
        {
            /// <summary>
            /// The GameTile template to load. Already fully constructed, just needs
            /// Initialize(tileId) called to assign RuntimeIds and set up physics.
            /// </summary>
            [MemoryPackOrder(0)]
            public GameTileBase NewGameTile;

            [MemoryPackOrder(1)]
            public byte WorldId; // u8

            [MemoryPackConstructor]
            public SpinToNewGameTile() { }

            public SpinToNewGameTile(GameTileBase newGameTile, byte worldId)
            {
                NewGameTile = newGameTile;
                WorldId = worldId;
            }
        }

        /// <summary>
        /// Load a pre-serialized ThroneTile template.
        /// </summary>
        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class LoadThroneTile : InputEvent
        {
            [MemoryPackOrder(0)]
            public ThroneTile NewThroneTile;

            [MemoryPackConstructor]
            public LoadThroneTile() { }

            public LoadThroneTile(ThroneTile throneTile)
            {
                NewThroneTile = throneTile;
            }
        }

        /// <summary>
        /// Set or update the king on the throne.
        /// </summary>
        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class SetKing : InputEvent
        {
            [MemoryPackOrder(0)]
            public ulong AccountId;

            [MemoryPackConstructor]
            public SetKing() { }

            public SetKing(ulong accountId)
            {
                AccountId = accountId;
            }
        }
    }
}
