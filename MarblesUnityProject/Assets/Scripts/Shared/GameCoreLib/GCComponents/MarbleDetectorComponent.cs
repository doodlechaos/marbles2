using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Detects when marbles enter/stay in contact with this object's collider.
    /// When detected, signals are sent to the configured receiver components.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class MarbleDetectorComponent
        : GCComponent,
            IGCComponentReferenceResolver,
            IGCComponentIdRemapper
    {
        /// <summary>
        /// Component IDs of IGCMarbleSignalReceiver components to notify.
        /// </summary>
        [MemoryPackOrder(2)]
        public List<ulong> ReceiverComponentIds = new();

        /// <summary>
        /// Whether to detect collision enter events (solid colliders).
        /// </summary>
        [MemoryPackOrder(3)]
        public bool CollisionEnterDetection = true;

        /// <summary>
        /// Whether to detect trigger enter events.
        /// </summary>
        [MemoryPackOrder(4)]
        public bool TriggerEnterDetection = true;

        /// <summary>
        /// Whether to detect collision stay events (solid colliders).
        /// </summary>
        [MemoryPackOrder(5)]
        public bool CollisionStayDetection = false;

        /// <summary>
        /// Whether to detect trigger stay events.
        /// </summary>
        [MemoryPackOrder(6)]
        public bool TriggerStayDetection = false;

        /// <summary>
        /// Cached receiver component references. Rebuilt after deserialization.
        /// </summary>
        [MemoryPackIgnore]
        public List<IGCMarbleSignalReceiver> Receivers = new();

        public void ResolveReferences(Dictionary<ulong, GCComponent> componentMap)
        {
            Receivers.Clear();

            foreach (var receiverId in ReceiverComponentIds)
            {
                if (
                    componentMap.TryGetValue(receiverId, out var component)
                    && component is IGCMarbleSignalReceiver receiver
                )
                {
                    Receivers.Add(receiver);
                }
            }
        }

        public void RemapComponentIds(Dictionary<ulong, ulong> idMap)
        {
            for (int i = 0; i < ReceiverComponentIds.Count; i++)
            {
                if (idMap.TryGetValue(ReceiverComponentIds[i], out var newId))
                {
                    ReceiverComponentIds[i] = newId;
                }
            }
        }

        /// <summary>
        /// Send marble signal to all registered receivers.
        /// </summary>
        public void SendSignal(MarbleComponent marble, GameTileBase tile)
        {
            foreach (var receiver in Receivers)
            {
                receiver.OnMarbleSignal(marble, tile);
            }
        }
    }
}
