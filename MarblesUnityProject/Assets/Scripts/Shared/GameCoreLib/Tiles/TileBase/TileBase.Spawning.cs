using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

#nullable enable

namespace GameCoreLib
{
    public abstract partial class TileBase
    {
        /// <summary>
        /// Instantiate a player marble from the PlayerMarbleTemplate.
        /// Handles cloning, RuntimeId assignment, component references, transform hierarchy, and physics setup.
        /// The marble is added to the tile hierarchy but NOT positioned - caller must position it.
        /// </summary>
        /// <param name="accountId">The account ID for this marble</param>
        /// <param name="bidAmount">The bid amount (optional, defaults to 0)</param>
        /// <returns>The MarbleComponent, or null if instantiation failed</returns>
        protected MarbleComponent? InstantiatePlayerMarble(
            ulong accountId,
            int points,
            uint bidAmount
        )
        {
            if (PlayerMarbleTemplate == null)
            {
                Logger.Error(
                    "PlayerMarbleTemplate is null – cannot spawn player marble. "
                        + "Ensure PlayerMarblePrefab is assigned on the tile auth and export was successful."
                );
                return null;
            }

            if (TileRoot == null)
            {
                Logger.Error("TileRoot is null – cannot spawn player marble.");
                return null;
            }

            // Clone the serialized template hierarchy
            GameCoreObj? marble = CloneRuntimeObjSubtree(PlayerMarbleTemplate);

            if (marble == null)
            {
                Logger.Error("Failed to clone PlayerMarbleTemplate – spawn aborted.");
                return null;
            }

            // Customize name
            marble.Name = $"PlayerMarble_{accountId}";

            // Attach to tile hierarchy
            if (TileRoot.Children == null)
                TileRoot.Children = new List<GameCoreObj>();
            TileRoot.Children.Add(marble);

            // Assign fresh RuntimeIds to the new subtree
            AssignRuntimeIds(marble);

            // Rebuild component → RuntimeObj references just for this subtree
            marble.RebuildComponentReferences();

            // Set up transform hierarchy for the marble subtree with TileRoot as parent
            // This enables Transform.Position and Transform.LossyScale to work correctly
            marble.SetupTransformHierarchy(TileRoot.Transform);

            // Fetch the MarbleComponent that was authored on the prefab template
            var marbleComp = marble.GetComponent<MarbleComponent>();
            if (marbleComp == null)
            {
                Logger.Error(
                    "PlayerMarbleTemplate is missing MarbleComponent – cannot spawn player marble."
                );
                return null;
            }

            // Apply per-player data
            marbleComp.AccountId = accountId;
            marbleComp.BidAmount = bidAmount;
            marbleComp.Points = points;
            marbleComp.IsAlive = true;
            marbleComp.EliminationOrder = 0;

            // Create physics bodies from authored collider/rigidbody components.
            // This sets PhysicsBodyId on each RuntimeObj that gets a physics body.
            AddPhysicsBody(marble);

            Logger.Log($"Player marble instantiated for account {accountId}");
            return marbleComp;
        }

        protected GameCoreObj? CloneRuntimeObjSubtree(GameCoreObj? source)
        {
            if (source == null)
                return null;

            var bytes = MemoryPackSerializer.Serialize(source);
            var clone = MemoryPackSerializer.Deserialize<GameCoreObj>(bytes);

            if (clone != null)
            {
                AssignComponentIdsForSpawn(clone);
            }

            return clone;
        }

        protected void AssignComponentIdsForSpawn(GameCoreObj? obj)
        {
            if (obj == null)
                return;

            var idRemap = new Dictionary<ulong, ulong>();
            AssignComponentIdsRecursive(obj, idRemap);
        }

        private void AssignComponentIdsRecursive(GameCoreObj obj, Dictionary<ulong, ulong> idRemap)
        {
            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    ulong originalId = component.ComponentId;
                    component.ComponentId = NextComponentId++;

                    if (originalId != 0)
                    {
                        idRemap[originalId] = component.ComponentId;
                    }
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    AssignComponentIdsRecursive(child, idRemap);
                }
            }

            RemapComponentIdReferences(obj, idRemap);
        }

        private void RemapComponentIdReferences(GameCoreObj obj, Dictionary<ulong, ulong> idRemap)
        {
            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    if (component is IGCComponentIdRemapper remapper)
                    {
                        remapper.RemapComponentIds(idRemap);
                    }
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    RemapComponentIdReferences(child, idRemap);
                }
            }
        }
    }
}
