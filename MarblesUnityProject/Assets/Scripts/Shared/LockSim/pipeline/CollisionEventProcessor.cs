namespace LockSim
{
    using System.Collections.Generic;
    using FPMathLib;

    /// <summary>
    /// Processes collision detection results to generate Enter/Stay/Exit events.
    /// Maintains deterministic state for snapshot/restore correctness.
    /// </summary>
    public static class CollisionEventProcessor
    {
        /// <summary>
        /// Processes the current frame's contacts and generates collision/trigger events.
        /// Updates the world's active collision pairs for the next frame.
        /// </summary>
        public static void ProcessEvents(World world, WorldSimulationContext context)
        {
            List<ContactManifold> contacts = context.GetContactsMutable();
            List<CollisionEvent> collisionEvents = context.GetCollisionEventsMutable();
            List<CollisionEvent> triggerEvents = context.GetTriggerEventsMutable();

            // Get the previous frame's active pairs
            List<ColliderPair> prevPairs = world.GetActiveCollisionPairsMutable();
            List<CollisionPairData> prevData = world.GetActiveCollisionDataMutable();

            // Build current frame's active pairs from contacts
            List<ColliderPair> currentPairs = new List<ColliderPair>(contacts.Count);
            List<CollisionPairData> currentData = new List<CollisionPairData>(contacts.Count);

            // Clear event buffers
            collisionEvents.Clear();
            triggerEvents.Clear();

            // Process each contact from this frame
            for (int i = 0; i < contacts.Count; i++)
            {
                ContactManifold contact = contacts[i];
                ColliderPair pair = new ColliderPair(contact.ColliderAId, contact.ColliderBId);

                // Get colliders to check trigger status
                world.TryGetCollider(contact.ColliderAId, out ColliderLS colliderA);
                world.TryGetCollider(contact.ColliderBId, out ColliderLS colliderB);
                bool isTrigger = colliderA.IsTrigger || colliderB.IsTrigger;

                // Build pair data
                CollisionPairData pairData = new CollisionPairData
                {
                    BodyIdA = contact.BodyAId,
                    BodyIdB = contact.BodyBId,
                    IsTrigger = isTrigger,
                    Normal = contact.Normal,
                    ContactPoint = contact.ContactPoint1,
                };

                currentPairs.Add(pair);
                currentData.Add(pairData);

                // Check if this pair existed in the previous frame
                bool wasActive = FindPairIndex(prevPairs, pair) >= 0;

                // Determine event type
                CollisionEventType eventType = wasActive
                    ? CollisionEventType.Stay
                    : CollisionEventType.Enter;

                // Create and add the event
                CollisionEvent evt = CollisionEvent.Create(
                    eventType,
                    isTrigger,
                    contact.ColliderAId,
                    contact.ColliderBId,
                    contact.BodyAId,
                    contact.BodyBId,
                    contact.Normal,
                    contact.ContactPoint1
                );

                if (isTrigger)
                {
                    triggerEvents.Add(evt);
                }
                else
                {
                    collisionEvents.Add(evt);
                }
            }

            // Find pairs that existed last frame but not this frame (Exit events)
            for (int i = 0; i < prevPairs.Count; i++)
            {
                ColliderPair pair = prevPairs[i];
                if (FindPairIndex(currentPairs, pair) < 0)
                {
                    // This pair is no longer colliding
                    CollisionPairData data = prevData[i];

                    CollisionEvent exitEvent = CollisionEvent.Create(
                        CollisionEventType.Exit,
                        data.IsTrigger,
                        pair.ColliderIdA,
                        pair.ColliderIdB,
                        data.BodyIdA,
                        data.BodyIdB,
                        FPVector2.Zero, // No valid normal for exit
                        FPVector2.Zero // No valid contact point for exit
                    );

                    if (data.IsTrigger)
                    {
                        triggerEvents.Add(exitEvent);
                    }
                    else
                    {
                        collisionEvents.Add(exitEvent);
                    }
                }
            }

            // Update world's active pairs for the next frame
            prevPairs.Clear();
            prevPairs.AddRange(currentPairs);
            prevData.Clear();
            prevData.AddRange(currentData);
        }

        /// <summary>
        /// Finds the index of a pair in the list, or -1 if not found.
        /// </summary>
        private static int FindPairIndex(List<ColliderPair> pairs, ColliderPair target)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                if (pairs[i] == target)
                    return i;
            }
            return -1;
        }
    }
}
