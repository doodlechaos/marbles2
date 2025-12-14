using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Box-shaped 2D collider component.
    /// Inherits common properties from Collider2DComponent.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class BoxCollider2DComponent : Collider2DComponent
    {
        /// <summary>
        /// Size of the box collider.
        /// </summary>
        [MemoryPackOrder(7)]
        public FPVector2 Size = FPVector2.One;
    }
}
