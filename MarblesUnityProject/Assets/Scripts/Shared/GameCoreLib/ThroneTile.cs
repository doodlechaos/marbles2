using System;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// The ThroneTile displays the current king and throne room environment.
    /// Unlike GameTiles, it doesn't have game states or competitive gameplay,
    /// but it still has a physics-enabled hierarchy for environmental effects.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class ThroneTile : TileBase
    {
        /// <summary>
        /// The account ID of the current king sitting on the throne.
        /// 0 means the throne is empty.
        /// </summary>
        [MemoryPackOrder(7)] // TileBase uses 0-6
        public ulong KingAccountId;

        public ThroneTile()
            : base() { }

        /// <summary>
        /// Set a new king on the throne.
        /// </summary>
        public void SetKing(ulong accountId)
        {
            KingAccountId = accountId;
            Logger.Log($"New king crowned: {accountId}");
        }

        /// <summary>
        /// Remove the current king from the throne.
        /// </summary>
        public void RemoveKing()
        {
            Logger.Log($"King {KingAccountId} dethroned");
            KingAccountId = 0;
        }

        /// <summary>
        /// Check if there is currently a king on the throne.
        /// </summary>
        [MemoryPackIgnore]
        public bool HasKing => KingAccountId != 0;
    }
}
