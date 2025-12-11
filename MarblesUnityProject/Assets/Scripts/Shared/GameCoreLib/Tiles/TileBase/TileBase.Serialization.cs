using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    public abstract partial class TileBase
    {
        /// <summary>
        /// Called by derived classes from their [MemoryPackOnDeserialized] callback.
        /// MemoryPack union types don't automatically call base class callbacks,
        /// so each concrete type must have its own callback that calls this method.
        /// </summary>
        protected void HandleDeserialization()
        {
            TileRoot?.RebuildComponentReferences();
            RefreshComponentIdCounter();
            RebuildBodyIdLookup();
            OnAfterDeserialize();
        }

        /// <summary>
        /// Forces the derived class to implement a method for the callback.
        /// IMPORTANT: You MUST add [MemoryPackOnDeserialized] to your override!
        /// </summary>
        protected abstract void OnMemoryPackDeserialized();

        private void RefreshComponentIdCounter()
        {
            ulong maxComponentId = GetMaxComponentId(TileRoot);

            if (NextComponentId <= maxComponentId)
            {
                NextComponentId = maxComponentId + 1;
            }
        }

        private ulong GetMaxComponentId(GameCoreObj? obj)
        {
            if (obj == null)
                return 0;

            ulong maxId = 0;

            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    if (component.ComponentId > maxId)
                        maxId = component.ComponentId;
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ulong childMax = GetMaxComponentId(child);
                    if (childMax > maxId)
                        maxId = childMax;
                }
            }

            return maxId;
        }
    }
}
