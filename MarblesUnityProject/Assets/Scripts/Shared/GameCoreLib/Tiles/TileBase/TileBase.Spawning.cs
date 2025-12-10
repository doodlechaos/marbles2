using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    public abstract partial class TileBase
    {
        protected GameCoreObj? CloneRuntimeObjSubtree(GameCoreObj? source)
        {
            if (source == null)
                return null;

            var bytes = MemoryPackSerializer.Serialize(source);
            var clone = MemoryPackSerializer.Deserialize<GameCoreObj>(bytes);

            if (clone != null)
            {
                AssignComponentIdsForSpawn(clone);
            }

            return clone;
        }

        protected void AssignComponentIdsForSpawn(GameCoreObj? obj)
        {
            if (obj == null)
                return;

            var idRemap = new Dictionary<ulong, ulong>();
            AssignComponentIdsRecursive(obj, idRemap);
        }

        private void AssignComponentIdsRecursive(GameCoreObj obj, Dictionary<ulong, ulong> idRemap)
        {
            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    ulong originalId = component.ComponentId;
                    component.ComponentId = NextComponentId++;

                    if (originalId != 0)
                    {
                        idRemap[originalId] = component.ComponentId;
                    }
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    AssignComponentIdsRecursive(child, idRemap);
                }
            }

            RemapComponentIdReferences(obj, idRemap);
        }

        private void RemapComponentIdReferences(GameCoreObj obj, Dictionary<ulong, ulong> idRemap)
        {
            if (obj.GameComponents != null)
            {
                foreach (var component in obj.GameComponents)
                {
                    if (component is IGCComponentIdRemapper remapper)
                    {
                        remapper.RemapComponentIds(idRemap);
                    }
                }
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    RemapComponentIdReferences(child, idRemap);
                }
            }
        }

        protected GameCoreObj SpawnRuntimeObj(string name, FPVector3 position)
        {
            var obj = new GameCoreObj
            {
                RuntimeId = GenerateRuntimeId(),
                Name = name,
                Children = new List<GameCoreObj>(),
                Transform = new FPTransform3D(position, FPQuaternion.Identity, FPVector3.One),
                GameComponents = new List<GCComponent>(),
            };

            TileRoot!.Children.Add(obj);
            return obj;
        }
    }
}
