using System;
using FPMathLib;
using LockSim;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Component that teleports marbles by an offset when triggered.
    /// Requires a MarbleDetectorComponent on the same GameObject to receive marble signals.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class TeleportWrapComponent : GCComponent, IGCMarbleSignalReceiver
    {
        [MemoryPackOrder(2)]
        public FPVector2 Offset = FPVector2.Zero;

        public void OnMarbleSignal(MarbleComponent marble, TileBase tile)
        {
            if (!Enabled || marble == null || !marble.IsAlive)
                return;

            var target = marble.RigidbodyRuntimeObj;
            if (target == null || !target.HasPhysicsBody)
                return;

            if (!tile.Sim.TryGetBody(target.PhysicsBodyId, out RigidBodyLS body))
                return;

            if (body.BodyType != BodyType.Dynamic)
                return;

            FPVector2 currentPos = body.Position;
            FPVector3 newPosition = new FPVector3(
                currentPos.X + Offset.X,
                currentPos.Y + Offset.Y,
                target.Transform.LocalPosition.Z
            );

            target.SetWorldPos(newPosition, tile.Sim, resetVelocity: false);
        }
    }
}
