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

    }

    [System.Serializable]
    public class ComponentData
    {
        public string type;
        public bool enabled;
        public string data;
    }

}

