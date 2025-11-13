using System;
using System.Collections.Generic;
using FPMathLib;
using LockSim;
using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class RuntimeObj
    {
        /// <summary>
        /// Stable, unique ID that persists through serialization.
        /// Used for tracking RuntimeObj-to-GameObject mappings across save/load cycles.
        /// </summary>
        [MemoryPackOrder(0)]
        public ulong RuntimeId;

        [MemoryPackOrder(1)]
        public string Name = "";

        //[MemoryPackIgnore]
        // public RuntimeObj Parent;
        [MemoryPackOrder(2)]
        public List<RuntimeObj> Children = new List<RuntimeObj>();

        [MemoryPackOrder(3)]
        public FPTransform3D Transform = new FPTransform3D();

        [MemoryPackOrder(4)]
        public List<ComponentData> Components = new List<ComponentData>();

        /// <summary>
        /// ID referencing which prefab to use for rendering.
        /// This is automatically assigned during export based on the RuntimeRenderer's renderPrefabs list.
        /// -1 = no prefab (empty GameObject)
        /// 0+ = 0-based index into RuntimeRenderer.renderPrefabs list (0 = first prefab, 1 = second, etc.)
        /// </summary>
        [MemoryPackOrder(5)]
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
