    using FPMathLib;
    using LockSim;
    using System;
    using System.Collections.Generic;
using System.Diagnostics;

namespace GameCoreLib
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
                Logger.Log("Loading: " +  levelJSON); 
                //Deserialize the leveljson into a RuntimeObj hierarchy which mirrors the gameobject hierarchy in unity

                //Handle authoring components. I.E. colliders or rigidbodies must create LockSim bodies
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

    }

