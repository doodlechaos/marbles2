using System;
using System.Collections.Generic;
using FPMathLib;
using LockSim;
using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable]
    public partial class RuntimeObj
    {
        public string Name;
        //[MemoryPackIgnore]
        // public RuntimeObj Parent;
        public List<RuntimeObj> Children;

        public FPTransform3D Transform;

        public List<ComponentData> Components;

        /// <summary>
        /// ID referencing which prefab to use for rendering.
        /// This is automatically assigned during export based on the RuntimeRenderer's renderPrefabs list.
        /// -1 = no prefab (empty GameObject)
        /// 0+ = 0-based index into RuntimeRenderer.renderPrefabs list (0 = first prefab, 1 = second, etc.)
        /// </summary>
        public int RenderPrefabID;

    }

    [MemoryPackable]
    public partial class ComponentData
    {
        public string type;
        public bool enabled;
        public string data;
    }

}

