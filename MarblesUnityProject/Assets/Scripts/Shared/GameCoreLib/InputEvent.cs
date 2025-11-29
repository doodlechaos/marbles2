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
    [MemoryPackUnion(5, typeof(LoadLevelFile))]
    [MemoryPackUnion(6, typeof(StartGameTile))]
    public abstract partial class InputEvent
    {
        protected InputEvent() { }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class StartGameTile : InputEvent
        {
            [MemoryPackOrder(0)]
            public Entrant[] Entrants { get; set; } = Array.Empty<Entrant>();

            [MemoryPackOrder(1)]
            public uint TotalMarblesBid { get; set; }

            [MemoryPackConstructor]
            public StartGameTile() { }

            public StartGameTile(Entrant[] entrants, uint totalMarblesBid)
            {
                Entrants = entrants;
                TotalMarblesBid = totalMarblesBid;
            }
        }

        [MemoryPackable]
        public partial struct Entrant
        {
            public ulong AccountId;
            public uint TotalBid;
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class Attack : InputEvent
        {
            [MemoryPackOrder(0)]
            public ulong AccountId { get; set; } // u64

            [MemoryPackOrder(1)]
            public uint Points { get; set; } // u32

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
            public byte WorldId { get; set; } // u8

            [MemoryPackOrder(1)]
            public ulong AccountId { get; set; } // u64

            [MemoryPackOrder(2)]
            public ulong SpawnerRuntimeId { get; set; } // u64

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
            public string Value { get; set; } = string.Empty;

            [MemoryPackConstructor]
            public SetIntegrationParameter() { }

            public SetIntegrationParameter(string value) => Value = value;
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class StartCloseDoorAnimation : InputEvent
        {
            [MemoryPackOrder(0)]
            public byte WorldId { get; set; } // u8

            [MemoryPackConstructor]
            public StartCloseDoorAnimation() { }

            public StartCloseDoorAnimation(byte worldId) => WorldId = worldId;
        }

        [MemoryPackable(SerializeLayout.Explicit)]
        public partial class LoadLevelFile : InputEvent
        {
            [MemoryPackOrder(0)]
            public byte WorldId { get; set; } // u8

            [MemoryPackOrder(1)]
            public LevelFile LevelFile { get; set; }

            [MemoryPackConstructor]
            public LoadLevelFile() { }

            public LoadLevelFile(byte worldId, LevelFile levelFile)
            {
                WorldId = worldId;
                LevelFile = levelFile;
            }
        }
    }
}
