using FPMath;
using LockSim;
using System;
using System.Collections.Generic;

namespace GameCore
{
    [Serializable]
    public class GameTile
    {
        public int WorldId;

        public RuntimeObj TileRoot;

        public World Sim;

        public GameTile(int worldId)
        {
            WorldId = worldId;
        }

        public void Load(string levelJSON)
        {
            //Create the TileRoot hierarchy from the gameobjects in the json

            //Spawn the necessary bodies into the simulation if they have collider or rigidbody component
        }

        public void Clear()
        {
            //Clear the Runtime Objects

            //Clear the physics simulation world
        }

        public void Step()
        {
            PhysicsPipeline.Step(Sim, FP.FromFloat(1 / 60f), new WorldSimulationContext());
        }
    }

    [Serializable]
    public class RuntimeObj 
    {
        public string Name;
        public FPVector2 LocalPos;
        public FP LocalRot;
        public FPVector2 LocalScale;

        public List<RuntimeObj> Children;

    }
}

