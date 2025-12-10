using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    public abstract partial class TileBase
    {
        [MemoryPackOnDeserialized]
        private void OnMemoryPackDeserialized()
        {
            TileRoot?.RebuildComponentReferences();
            RefreshComponentIdCounter();
            OnAfterDeserialize();
        }

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
