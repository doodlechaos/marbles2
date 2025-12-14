using System;
using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Base class for all strongly-typed GameCore components.
    /// These are pure data classes that work on both client and server.
    ///
    /// To add a new component:
    /// 1. Create a new class inheriting from GameComponent
    /// 2. Add MemoryPackUnion attribute here with next available index
    /// 3. Create corresponding Unity Auth component (if needed for authoring)
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(BoxCollider2DComponent))]
    [MemoryPackUnion(1, typeof(CircleCollider2DComponent))]
    [MemoryPackUnion(2, typeof(Rigidbody2DComponent))]
    [MemoryPackUnion(3, typeof(SpawnPipeComponent))]
    [MemoryPackUnion(4, typeof(MarbleComponent))]
    [MemoryPackUnion(5, typeof(TileRootComponent))]
    [MemoryPackUnion(6, typeof(TeleportWrapComponent))]
    [MemoryPackUnion(7, typeof(MarbleDetectorComponent))]
    [MemoryPackUnion(8, typeof(MarbleEffectComponent))]
    [MemoryPackUnion(9, typeof(ThroneComponent))]
    [MemoryPackUnion(10, typeof(AutoSpinComponent))]
    [MemoryPackUnion(11, typeof(DefenseBrickComponent))]
    [MemoryPackUnion(12, typeof(DefenseWallComponent))]
    public abstract partial class GCComponent
    {
        [MemoryPackOrder(0)]
        public bool Enabled = true;

        /// <summary>
        /// Stable identifier used for cross-component references.
        /// Assigned during authoring conversion and preserved through serialization.
        /// </summary>
        [MemoryPackOrder(1)]
        public ulong ComponentId;

        /// <summary>
        /// Reference to the RuntimeObj that owns this component.
        /// Similar to Unity's component.gameObject.
        /// Not serialized - rebuilt after deserialization via RuntimeObj.RebuildComponentReferences()
        /// </summary>
        [MemoryPackIgnore]
        public GameCoreObj GCObj { get; internal set; }

        /// <summary>
        /// Convenience accessor for the owning RuntimeObj's transform.
        /// Similar to Unity's component.transform.
        /// </summary>
        [MemoryPackIgnore]
        public FPTransform3D Transform => GCObj?.Transform;

        /// <summary>
        /// Called every simulation step. Override in derived components to implement per-step logic.
        /// </summary>
        public virtual void Step() { }
    }

    public interface IGCComponentReferenceResolver
    {
        void ResolveReferences(Dictionary<ulong, GCComponent> componentMap);
    }

    public interface IGCComponentIdRemapper
    {
        void RemapComponentIds(Dictionary<ulong, ulong> idMap);
    }
}
