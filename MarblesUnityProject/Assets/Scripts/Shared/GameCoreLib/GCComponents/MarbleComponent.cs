using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Represents a player's marble in the game.
    /// Attached at runtime when spawning players.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class MarbleComponent
        : GCComponent,
            IGCComponentReferenceResolver,
            IGCComponentIdRemapper
    {
        [MemoryPackOrder(2)]
        public ulong AccountId;

        [MemoryPackOrder(3)]
        public uint BidAmount;

        [MemoryPackOrder(4)]
        public int Points;

        /// <summary>
        /// Whether this player is still alive in the game
        /// </summary>
        [MemoryPackOrder(5)]
        public bool IsAlive = true;


        /// <summary>
        /// Component ID of the Rigidbody2DComponent associated with this player marble.
        /// Assigned during authoring using the PlayerMarbleAuth reference.
        /// </summary>
        [MemoryPackOrder(6)]
        public ulong RigidbodyComponentId;

        /// <summary>
        /// Reference to the RuntimeObj that contains the rigidbody/physics body.
        /// Not serialized - rebuilt after deserialization via ResolveReferences().
        /// Use this to get the physics body position and for teleportation.
        /// Can be null if no rigidbody is found in the hierarchy.
        /// </summary>
        [MemoryPackIgnore]
        public Rigidbody2DComponent? RigidbodyComponent { get; private set; }

        [MemoryPackIgnore]
        public GameCoreObj? RigidbodyRuntimeObj => RigidbodyComponent?.GCObj;

        /// <summary>
        /// Finds and caches the rigidbody RuntimeObj reference.
        /// Call this after RebuildComponentReferences() or after cloning.
        /// </summary>
        public void ResolveReferences(Dictionary<ulong, GCComponent> componentMap)
        {
            RigidbodyComponent = null;

            if (RigidbodyComponentId != 0)
            {
                if (
                    componentMap.TryGetValue(RigidbodyComponentId, out var component)
                    && component is Rigidbody2DComponent rigidbody
                )
                {
                    RigidbodyComponent = rigidbody;
                    return;
                }
            }

            // Fallback: search for first child with Rigidbody2DComponent
            if (GCObj != null)
            {
                var runtimeObj = GCObj.FindChildWithComponent<Rigidbody2DComponent>();
                if (runtimeObj != null)
                    RigidbodyComponent = runtimeObj.GetComponent<Rigidbody2DComponent>();
            }
        }

        public void RemapComponentIds(Dictionary<ulong, ulong> idMap)
        {
            if (
                RigidbodyComponentId != 0
                && idMap.TryGetValue(RigidbodyComponentId, out ulong newId)
            )
            {
                RigidbodyComponentId = newId;
            }
        }
    }
}
