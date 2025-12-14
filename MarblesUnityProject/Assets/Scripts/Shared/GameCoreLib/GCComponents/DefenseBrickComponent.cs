using System;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// A brick breaker style defense brick that takes damage from marbles.
    /// When hit, subtracts 1 point from the marble and 1 health from itself.
    /// Disables itself when health reaches 0.
    /// Requires a MarbleDetectorComponent on the same GameObject to receive marble signals.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class DefenseBrickComponent : GCComponent, IGCMarbleSignalReceiver
    {
        /// <summary>
        /// Current health of this brick. When it reaches 0, the brick is disabled.
        /// </summary>
        [MemoryPackOrder(2)]
        public int Health = 1;

        public void OnMarbleSignal(MarbleComponent marble, TileBase tile)
        {
            if (!Enabled || marble == null || !marble.IsAlive || Health <= 0)
                return;

            // Subtract 1 point from the marble
            if (marble.Points > 0)
            {
                marble.Points--;
            }

            // If marble's points reached 0, explode it
            if (marble.Points == 0)
            {
                tile.ExplodeMarble(marble);
            }

            // Subtract 1 health from the brick
            Health--;

            // If brick's health reached 0, disable this object
            if (Health <= 0)
            {
                GCObj?.SetActive(false, tile.Sim);
            }
        }

        /// <summary>
        /// Reinitializes the brick with the specified health and re-enables it.
        /// Called by DefenseWallComponent when rebuilding the wall.
        /// </summary>
        public void Reinitialize(int startingHealth, TileBase tile)
        {
            Health = startingHealth;
            Enabled = true;
            GCObj?.SetActive(true, tile.Sim);
        }
    }
}
