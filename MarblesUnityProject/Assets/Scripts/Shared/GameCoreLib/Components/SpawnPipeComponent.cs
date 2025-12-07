using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Marks an object as a spawn pipe where players can enter the game.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class SpawnPipeComponent : RuntimeObjComponent
    {
        /// <summary>
        /// Delay in seconds between spawning each player
        /// </summary>
        [MemoryPackOrder(1)]
        public FP SpawnDelay = FP.FromFloat(0.5f);
    }
}
