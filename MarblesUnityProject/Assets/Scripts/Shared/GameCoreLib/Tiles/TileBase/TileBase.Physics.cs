using FPMathLib;
using LockSim;
using System;

namespace GameCoreLib
{
    public abstract partial class TileBase
    {
        protected void AddPhysicsBody(GameCoreObj obj)
        {
            RuntimePhysicsBuilder.AddPhysicsBody(obj, Sim, physicsBindings);
            RebuildBodyIdLookup();
        }

        protected void RebuildBodyIdLookup()
        {
            bodyIdToRuntimeId.Clear();
            foreach (var kvp in physicsBindings)
            {
                bodyIdToRuntimeId[kvp.Value.BodyId] = kvp.Key;
            }
        }

        protected GameCoreObj? FindRuntimeObjByBodyId(int bodyId)
        {
            return bodyIdToRuntimeId.TryGetValue(bodyId, out ulong runtimeId)
                ? TileRoot?.FindByRuntimeId(runtimeId)
                : null;
        }

        protected void RemovePhysicsHierarchy(GameCoreObj obj)
        {
            if (physicsBindings.TryGetValue(obj.RuntimeId, out PhysicsBinding binding))
            {
                Sim.RemoveBody(binding.BodyId);
                physicsBindings.Remove(obj.RuntimeId);
                bodyIdToRuntimeId.Remove(binding.BodyId);
                obj.PhysicsBodyId = -1;
            }

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    RemovePhysicsHierarchy(child);
                }
            }
        }

        protected void SyncPhysicsToRuntimeObjs()
        {
            if (TileRoot == null)
                return;

            SyncPhysicsRecursive(TileRoot, FPVector3.Zero);
        }

        private void SyncPhysicsRecursive(GameCoreObj runtimeObj, FPVector3 parentWorldPos)
        {
            FPVector3 currentWorldPos = parentWorldPos + runtimeObj.Transform.LocalPosition;

            if (physicsBindings.TryGetValue(runtimeObj.RuntimeId, out PhysicsBinding binding))
            {
                try
                {
                    var body = Sim.GetBody(binding.BodyId);

                    FP localX = body.Position.X - parentWorldPos.X;
                    FP localY = body.Position.Y - parentWorldPos.Y;
                    FP localZ = runtimeObj.Transform.LocalPosition.Z;

                    runtimeObj.Transform.LocalPosition = new FPVector3(localX, localY, localZ);

                    currentWorldPos = new FPVector3(
                        body.Position.X,
                        body.Position.Y,
                        parentWorldPos.Z + localZ
                    );

                    if (body.BodyType == BodyType.Dynamic)
                    {
                        FP physicsZDegrees = body.Rotation * FP.Rad2Deg;
                        FPQuaternion physicsTwist =
                            FPQuaternion.AngleAxis(physicsZDegrees, FPVector3.Forward);

                        runtimeObj.Transform.LocalRotation = physicsTwist * binding.BaseSwing;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"Failed to sync physics for RuntimeId {runtimeObj.RuntimeId}: {e.Message}"
                    );
                }
            }

            if (runtimeObj.Children != null)
            {
                foreach (var child in runtimeObj.Children)
                {
                    SyncPhysicsRecursive(child, currentWorldPos);
                }
            }
        }
    }
}
