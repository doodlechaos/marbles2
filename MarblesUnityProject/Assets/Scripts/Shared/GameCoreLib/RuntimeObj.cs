using FPMathLib;
using LockSim;
using System;
using System.Collections.Generic;

namespace GameCoreLib
{
    [Serializable]
    public class RuntimeObj 
    {
        public string Name;
        public RuntimeObj Parent;
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

    [System.Serializable]
    public class ComponentData
    {
        public string type;
        public bool enabled;
        public string data;
    }

}

