using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
            GameTile1.Step();
            GameTile2.Step();

            Seq = Seq.WrappingAdd(1);
        }

        public string GetHash()
        {
            byte[] data = MemoryPackSerializer.Serialize(this);

            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
