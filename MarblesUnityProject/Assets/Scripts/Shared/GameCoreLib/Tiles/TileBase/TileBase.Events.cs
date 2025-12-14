using FPMathLib;
using LockSim;

namespace GameCoreLib
{
    public abstract partial class TileBase
    {
        protected virtual void ProcessTriggerEvents()
        {
            foreach (var evt in simContext.TriggerEvents)
            {
                bool isEnter = evt.EventType == CollisionEventType.Enter;
                bool isStay = evt.EventType == CollisionEventType.Stay;

                if (isEnter || isStay)
                {
                    ProcessMarbleDetectorEvent(evt, isTrigger: true, isEnter: isEnter);
                }
            }
        }

        protected virtual void ProcessCollisionEvents()
        {
            foreach (var evt in simContext.CollisionEvents)
            {
                bool isEnter = evt.EventType == CollisionEventType.Enter;
                bool isStay = evt.EventType == CollisionEventType.Stay;

                if (isEnter || isStay)
                {
                    ProcessMarbleDetectorEvent(evt, isTrigger: false, isEnter: isEnter);
                }
            }
        }

        protected void ProcessMarbleDetectorEvent(CollisionEvent evt, bool isTrigger, bool isEnter)
        {
            var objA = FindRuntimeObjByBodyId(evt.BodyIdA);
            var objB = FindRuntimeObjByBodyId(evt.BodyIdB);

            if (objA == null || objB == null)
                return;

            var detectorA = objA.GetComponent<MarbleDetectorComponent>();
            var detectorB = objB.GetComponent<MarbleDetectorComponent>();

            var marbleA = FindMarbleComponent(objA);
            var marbleB = FindMarbleComponent(objB);

            if (
                detectorA != null
                && marbleB != null
                && ShouldProcessDetection(detectorA, isTrigger, isEnter)
            )
            {
                detectorA.SendSignal(marbleB, this);
            }

            if (
                detectorB != null
                && marbleA != null
                && ShouldProcessDetection(detectorB, isTrigger, isEnter)
            )
            {
                detectorB.SendSignal(marbleA, this);
            }
        }

        protected MarbleComponent? FindMarbleComponent(GameCoreObj obj)
        {
            var marble = obj.GetComponent<MarbleComponent>();
            if (marble != null)
                return marble;

            return TileRoot?.FindComponentByPredicate<MarbleComponent>(m =>
                m.RigidbodyRuntimeObj == obj
            );
        }

        private static bool ShouldProcessDetection(
            MarbleDetectorComponent detector,
            bool isTrigger,
            bool isEnter
        )
        {
            if (isTrigger)
            {
                return isEnter ? detector.TriggerEnterDetection : detector.TriggerStayDetection;
            }

            return isEnter ? detector.CollisionEnterDetection : detector.CollisionStayDetection;
        }

        public void ExplodeMarble(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            if (!pendingMarbleDestructions.Contains(marble))
            {
                pendingMarbleDestructions.Add(marble);
            }
        }

        private void ProcessPendingMarbleDestructions()
        {
            foreach (var marble in pendingMarbleDestructions)
            {
                DestroyMarble(marble);
            }

            pendingMarbleDestructions.Clear();
        }

        protected virtual void DestroyMarble(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            marble.IsAlive = false;

            var marbleRoot = marble.GCObj;
            if (marbleRoot == null)
            {
                Logger.Error($"Could not find RuntimeObj for marble {marble.AccountId}");
                return;
            }

            Logger.Log($"Destroying marble: {marbleRoot.Name} (AccountId: {marble.AccountId})");

            RemovePhysicsHierarchy(marbleRoot);

            TileRoot?.Children?.Remove(marbleRoot);
        }
    }
}
