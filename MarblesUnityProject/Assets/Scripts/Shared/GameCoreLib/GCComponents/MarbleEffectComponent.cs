using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    [Flags]
    public enum MarbleEffect
    {
        None = 0,
        Add = 1,
        Multiply = 2,
        Explode = 4,
        Divide = 8,
        Subtract = 16,
        Zero = 32,
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class MarbleEffectComponent : GCComponent {

        //TODO: Can be activated by whatever I configure (usually a collision or a trigger enter)
        //Apply the confugred effect to the marble. 

        

     }
}
