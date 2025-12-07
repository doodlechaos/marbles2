using System;
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
    [MemoryPackUnion(4, typeof(PlayerMarbleComponent))]
    [MemoryPackUnion(5, typeof(LevelRootComponent))]
    [MemoryPackUnion(6, typeof(TeleportWrapComponent))]
    public abstract partial class RuntimeObjComponent
    {
        [MemoryPackOrder(0)]
        public bool Enabled = true;

        /// <summary>
        /// Reference to the RuntimeObj that owns this component.
        /// Similar to Unity's component.gameObject.
        /// Not serialized - rebuilt after deserialization via RuntimeObj.RebuildComponentReferences()
        /// </summary>
        [MemoryPackIgnore]
        public RuntimeObj RuntimeObj { get; internal set; }

        /// <summary>
        /// Convenience accessor for the owning RuntimeObj's transform.
        /// Similar to Unity's component.transform.
        /// </summary>
        [MemoryPackIgnore]
        public FPTransform3D Transform => RuntimeObj?.Transform;
    }

    #region Physics Components

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class BoxCollider2DComponent : RuntimeObjComponent
    {
        [MemoryPackOrder(1)]
        public FPVector2 Size = FPVector2.One;

        [MemoryPackOrder(2)]
        public FPVector2 Offset = FPVector2.Zero;

        [MemoryPackOrder(3)]
        public bool IsTrigger = false;

        /// <summary>
        /// Physics material friction. Authored from Unity PhysicsMaterial2D.friction.
        /// </summary>
        [MemoryPackOrder(4)]
        public FP Friction = FP.FromFloat(0.5f);

        /// <summary>
        /// Physics material restitution (bounciness). Authored from Unity PhysicsMaterial2D.bounciness.
        /// </summary>
        [MemoryPackOrder(5)]
        public FP Restitution = FP.Zero;
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class CircleCollider2DComponent : RuntimeObjComponent
    {
        [MemoryPackOrder(1)]
        public FP Radius = FP.Half;

        [MemoryPackOrder(2)]
        public FPVector2 Offset = FPVector2.Zero;

        [MemoryPackOrder(3)]
        public bool IsTrigger = false;

        /// <summary>
        /// Physics material friction. Authored from Unity PhysicsMaterial2D.friction.
        /// </summary>
        [MemoryPackOrder(4)]
        public FP Friction = FP.FromFloat(0.5f);

        /// <summary>
        /// Physics material restitution (bounciness). Authored from Unity PhysicsMaterial2D.bounciness.
        /// </summary>
        [MemoryPackOrder(5)]
        public FP Restitution = FP.Zero;
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class Rigidbody2DComponent : RuntimeObjComponent
    {
        [MemoryPackOrder(1)]
        public Rigidbody2DType BodyType = Rigidbody2DType.Dynamic;

        [MemoryPackOrder(2)]
        public FP Mass = FP.One;

        [MemoryPackOrder(3)]
        public FP LinearDrag = FP.Zero;

        [MemoryPackOrder(4)]
        public FP AngularDrag = FP.FromFloat(0.05f);

        [MemoryPackOrder(5)]
        public FP GravityScale = FP.One;

        [MemoryPackOrder(6)]
        public bool FreezeRotation = false;
    }

    public enum Rigidbody2DType
    {
        Dynamic = 0,
        Kinematic = 1,
        Static = 2,
    }

    #endregion

    #region Game-Specific Components

    /// <summary>
    /// Marks an object as a spawn pipe where players can enter the game.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class SpawnPipeComponent : RuntimeObjComponent
    {
        /// <summary>
        /// Delay in seconds between spawning each player
        /// </summary>
        [MemoryPackOrder(1)]
        public FP SpawnDelay = FP.FromFloat(0.5f);
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class TeleportWrapComponent : RuntimeObjComponent
    {
        [MemoryPackOrder(1)]
        public FPVector2 Offset = FPVector2.Zero;
    }

    /// <summary>
    /// Represents a player's marble in the game.
    /// Attached at runtime when spawning players.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class PlayerMarbleComponent : RuntimeObjComponent
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
    }

    /// <summary>
    /// Marker component indicating this RuntimeObj is the root of a level.
    /// Level roots are containers and typically don't have visual representation.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class LevelRootComponent : RuntimeObjComponent
    {
        /// <summary>
        /// The game mode type for this level (e.g., "SimpleBattleRoyale")
        /// </summary>
        [MemoryPackOrder(1)]
        public string GameModeType = "";
    }

    #endregion
}
