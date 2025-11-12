// InputEvent.cs
using MemoryPack;

namespace GameCoreLib
{
    // Base of the union
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(Attack))]
    [MemoryPackUnion(1, typeof(SpawnMarble))]
    [MemoryPackUnion(2, typeof(Dhash))]
    [MemoryPackUnion(3, typeof(SetIntegrationParameter))]
    [MemoryPackUnion(4, typeof(StartCloseDoorAnimation))]
    [MemoryPackUnion(5, typeof(LoadTileFile))]
    public abstract partial class InputEvent
    {

        [MemoryPackable]
        public partial class Attack : InputEvent
        {
            public ulong AccountId { get; set; }   // u64
            public uint Points { get; set; }       // u32

            public Attack(ulong accountId, uint points)
            {
                AccountId = accountId;
                Points = points;
            }
        }

        [MemoryPackable]
        public partial class SpawnMarble : InputEvent
        {
            public byte WorldId { get; set; }             // u8
            public ulong AccountId { get; set; }          // u64
            public ulong SpawnerRuntimeId { get; set; }   // u64

            public SpawnMarble(byte worldId, ulong accountId, ulong spawnerRuntimeId)
            {
                WorldId = worldId;
                AccountId = accountId;
                SpawnerRuntimeId = spawnerRuntimeId;
            }
        }

        [MemoryPackable]
        public partial class Dhash : InputEvent
        {
            public Dhash() { }
        }

        [MemoryPackable]
        public partial class SetIntegrationParameter : InputEvent
        {
            public string Value { get; set; } = string.Empty;

            public SetIntegrationParameter(string value) => Value = value;
        }

        [MemoryPackable]
        public partial class StartCloseDoorAnimation : InputEvent
        {
            public byte WorldId { get; set; } // u8

            public StartCloseDoorAnimation(byte worldId) => WorldId = worldId;
        }

        [MemoryPackable]
        public partial class LoadTileFile : InputEvent
        {
            public byte WorldId { get; set; }                 // u8
            public ulong NextTileId { get; set; }             // u64
            public string? HydratedNextTileJson { get; set; } // Option<String> → string?
            public LoadTileFile(byte worldId, ulong nextTileId, string? hydratedNextTileJson)
            {
                WorldId = worldId;
                NextTileId = nextTileId;
                HydratedNextTileJson = hydratedNextTileJson;
            }
        }

    }
}
