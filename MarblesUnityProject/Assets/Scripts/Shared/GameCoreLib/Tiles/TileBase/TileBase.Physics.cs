using System;
using FPMathLib;
using LockSim;

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

        /// <summary>
        /// Syncs transform changes from RuntimeObjs TO physics bodies.
        /// Called BEFORE physics step to apply game logic changes (like spinning platforms).
        /// Only updates static/kinematic bodies - dynamic bodies are controlled by physics.
        /// </summary>
        protected void SyncRuntimeObjsToPhysics()
        {
            if (TileRoot == null)
                return;

            SyncToPhysicsRecursive(TileRoot, FPVector3.Zero);
        }

        private void SyncToPhysicsRecursive(GameCoreObj runtimeObj, FPVector3 parentWorldPos)
        {
            FPVector3 currentWorldPos = parentWorldPos + runtimeObj.Transform.LocalPosition;

            if (physicsBindings.TryGetValue(runtimeObj.RuntimeId, out PhysicsBinding binding))
            {
                try
                {
                    var body = Sim.GetBody(binding.BodyId);

                    // Only sync TO physics for static bodies (game logic controls them)
                    // Dynamic bodies are controlled by physics simulation
                    if (body.BodyType == BodyType.Static)
                    {
                        // Update position from transform
                        body.Position = new FPVector2(currentWorldPos.X, currentWorldPos.Y);

                        // Extract Z rotation from quaternion for 2D physics
                        FPQuaternion twist = FPQuaternion.ExtractTwist(
                            runtimeObj.Transform.LocalRotation,
                            FPVector3.Forward
                        );
                        FP rotationRad = FP.Two * FPMath.Atan2(twist.Z, twist.W);
                        body.Rotation = rotationRad;

                        Sim.SetBody(binding.BodyId, body);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"Failed to sync to physics for RuntimeId {runtimeObj.RuntimeId}: {e.Message}"
                    );
                }
            }

            if (runtimeObj.Children != null)
            {
                foreach (var child in runtimeObj.Children)
                {
                    SyncToPhysicsRecursive(child, currentWorldPos);
                }
            }
        }

        /// <summary>
        /// Syncs physics simulation results FROM physics bodies TO RuntimeObjs.
        /// Called AFTER physics step to apply physics results to transforms.
        /// Only updates dynamic bodies - static bodies are controlled by game logic.
        /// </summary>
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
                        FPQuaternion physicsTwist = FPQuaternion.AngleAxis(
                            physicsZDegrees,
                            FPVector3.Forward
                        );

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
