using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Circle-shaped 2D collider component.
    /// Inherits common properties from Collider2DComponent.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class CircleCollider2DComponent : Collider2DComponent
    {
        /// <summary>
        /// Radius of the circle collider.
        /// </summary>
        [MemoryPackOrder(7)]
        public FP Radius = FP.Half;
    }
}
