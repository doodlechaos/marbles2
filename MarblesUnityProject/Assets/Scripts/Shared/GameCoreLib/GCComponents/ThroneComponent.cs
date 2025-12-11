using System;
using System.Collections.Generic;
using MemoryPack;

#nullable enable

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class ThroneComponent
        : GCComponent,
            IGCMarbleSignalReceiver,
            IGCComponentReferenceResolver,
            IGCComponentIdRemapper
    {
        [MemoryPackOrder(2)]
        public ulong KingBodyColliderComponentId;

        [MemoryPackIgnore]
        public CircleCollider2DComponent? KingBodyColliderComponent { get; private set; }

        public void OnMarbleSignal(MarbleComponent marble, TileBase tile)
        {
            Logger.Log($"OnMarbleSignal: {marble.AccountId}, {tile.GetType().Name}");
            if (!Enabled || marble == null || !marble.IsAlive)
                return;

            // Only ThroneTile handles throne captures
            if (tile is ThroneTile throneTile)
            {
                // Crown the new king - this will also explode the marble
                throneTile.CrownNewKing(marble);
            }
        }

        public void ResolveReferences(Dictionary<ulong, GCComponent> componentMap)
        {
            KingBodyColliderComponent = null;

            if (KingBodyColliderComponentId != 0)
            {
                if (
                    componentMap.TryGetValue(KingBodyColliderComponentId, out var component)
                    && component is CircleCollider2DComponent collider
                )
                {
                    KingBodyColliderComponent = collider;
                }
            }
        }

        public void RemapComponentIds(Dictionary<ulong, ulong> idMap)
        {
            if (
                KingBodyColliderComponentId != 0
                && idMap.TryGetValue(KingBodyColliderComponentId, out ulong newId)
            )
            {
                KingBodyColliderComponentId = newId;
            }
        }
    }
}
