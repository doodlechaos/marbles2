using System;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Marker component indicating this RuntimeObj is the root of a level.
    /// Level roots are containers and typically don't have visual representation.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class LevelRootComponent : GCComponent
    {
        /// <summary>
        /// The game mode type for this level (e.g., "SimpleBattleRoyale")
        /// </summary>
        [MemoryPackOrder(2)]
        public string GameModeType = "";
    }
}
