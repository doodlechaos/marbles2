using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Manages a collection of DefenseBrickComponents as a wall.
    /// Provides functionality to rebuild the wall by reinitializing all child bricks.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class DefenseWallComponent : GCComponent
    {
        /// <summary>
        /// The starting health for each brick when the wall is rebuilt.
        /// </summary>
        [MemoryPackOrder(2)]
        public int BrickStartingHealth = 1;

        /// <summary>
        /// Cached list of child brick components. Not serialized - rebuilt as needed.
        /// </summary>
        [MemoryPackIgnore]
        private List<DefenseBrickComponent> cachedBricks;

        /// <summary>
        /// Gets all DefenseBrickComponents from children of this GCObj.
        /// Caches the result for performance.
        /// </summary>
        public List<DefenseBrickComponent> GetBricks()
        {
            if (cachedBricks == null)
            {
                cachedBricks = new List<DefenseBrickComponent>();
            }
            else
            {
                cachedBricks.Clear();
            }

            GCObj?.FindAllComponentsInChildren(cachedBricks);
            return cachedBricks;
        }

        /// <summary>
        /// Rebuilds the wall by reinitializing all child DefenseBrickComponents.
        /// Resets their health and re-enables their colliders.
        /// </summary>
        public void RebuildWall(TileBase tile)
        {
            var bricks = GetBricks();

            foreach (var brick in bricks)
            {
                brick.Reinitialize(BrickStartingHealth, tile);
            }

            Logger.Log(
                $"DefenseWall rebuilt: {bricks.Count} bricks reinitialized with {BrickStartingHealth} health"
            );
        }
    }
}
