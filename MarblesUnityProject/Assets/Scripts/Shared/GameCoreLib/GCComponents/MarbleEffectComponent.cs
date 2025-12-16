using System;
using MemoryPack;

namespace GameCoreLib
{
    [Flags]
    public enum MarbleEffect
    {
        None = 0,
        Add = 1,
        Multiply = 2,
        Explode = 4,
        Divide = 8,
        Subtract = 16,
        Zero = 32,
    }

    /// <summary>
    /// Interface for GameCore components that can receive marble signals.
    /// Implemented by components that need to react when a marble is detected.
    /// </summary>
    public interface IGCMarbleSignalReceiver
    {
        /// <summary>
        /// Called when a marble signal is received from a MarbleDetectorComponent.
        /// </summary>
        /// <param name="marble">The marble component that triggered the signal</param>
        /// <param name="tile">The tile context for applying effects</param>
        void OnMarbleSignal(MarbleComponent marble, TileBase tile);
    }

    /// <summary>
    /// Component that applies effects to marbles when triggered.
    /// Activated by MarbleDetectorComponent when a marble enters/stays in the detector.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class MarbleEffectComponent : GCComponent, IGCMarbleSignalReceiver
    {
        /// <summary>
        /// The effect to apply when a marble signal is received.
        /// </summary>
        [MemoryPackOrder(2)]
        public MarbleEffect Effect;

        public void OnMarbleSignal(MarbleComponent marble, TileBase tile)
        {
            if (!Enabled || marble == null || marble.IsExploding)
                return;

            if ((Effect & MarbleEffect.Explode) != 0)
            {
                tile.FlagMarbleToExplode(marble);
            }

            // Future effects can be added here:
            // if ((Effect & MarbleEffect.Add) != 0) { ... }
            // if ((Effect & MarbleEffect.Multiply) != 0) { ... }
        }
    }
}
