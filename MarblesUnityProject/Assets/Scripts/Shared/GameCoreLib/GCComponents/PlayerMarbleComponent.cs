using System;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Represents a player's marble in the game.
    /// Attached at runtime when spawning players.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class PlayerMarbleComponent : GCComponent
    {
        [MemoryPackOrder(1)]
        public ulong AccountId;

        [MemoryPackOrder(2)]
        public uint BidAmount;

        /// <summary>
        /// Whether this player is still alive in the game
        /// </summary>
        [MemoryPackOrder(3)]
        public bool IsAlive = true;

        /// <summary>
        /// When this player was eliminated (0 = not eliminated yet, 1 = first eliminated, etc.)
        /// Higher number = eliminated later = better rank.
        /// </summary>
        [MemoryPackOrder(4)]
        public int EliminationOrder = 0;

        /// <summary>
        /// Name of the child RuntimeObj that contains the Rigidbody2DComponent.
        /// Set during authoring from PlayerMarbleAuth.RB2D reference.
        /// Used to find the rigidbody reference after deserialization.
        /// </summary>
        [MemoryPackOrder(5)]
        public string RigidbodyChildName = "";

        /// <summary>
        /// Reference to the RuntimeObj that contains the rigidbody/physics body.
        /// Not serialized - rebuilt after deserialization via FindRigidbodyReference().
        /// Use this to get the physics body position and for teleportation.
        /// Can be null if no rigidbody is found in the hierarchy.
        /// </summary>
        [MemoryPackIgnore]
        public GameCoreObj? RigidbodyRuntimeObj { get; private set; }

        /// <summary>
        /// Finds and caches the rigidbody RuntimeObj reference.
        /// Call this after RebuildComponentReferences() or after cloning.
        /// </summary>
        public void FindRigidbodyReference()
        {
            if (GCObj == null)
                return;

            // First try to find by name if specified
            if (!string.IsNullOrEmpty(RigidbodyChildName))
            {
                RigidbodyRuntimeObj = FindChildByName(GCObj, RigidbodyChildName);
                if (RigidbodyRuntimeObj != null)
                    return;
            }

            // Fallback: search for first child with Rigidbody2DComponent
            RigidbodyRuntimeObj = GCObj.FindChildWithComponent<Rigidbody2DComponent>();

            // If still null, check if self has the rigidbody
            if (RigidbodyRuntimeObj == null && GCObj.HasComponent<Rigidbody2DComponent>())
            {
                RigidbodyRuntimeObj = GCObj;
            }
        }

        private static GameCoreObj FindChildByName(GameCoreObj parent, string name)
        {
            if (parent.Name == name)
                return parent;

            if (parent.Children != null)
            {
                foreach (var child in parent.Children)
                {
                    var found = FindChildByName(child, name);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
    }
}
