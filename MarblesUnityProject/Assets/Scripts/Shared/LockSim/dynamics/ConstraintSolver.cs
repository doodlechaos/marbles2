namespace LockSim
{
    using System.Collections.Generic;
    using FPMathLib;

    public static class ConstraintSolver
    {
        public static void SolveContacts(World world, FP deltaTime, WorldSimulationContext context)
        {
            List<RigidBodyLS> bodies = world.GetBodiesMutable();
            List<ContactManifold> contacts = context.GetContactsMutable();

            // Iterative impulse solver
            for (int iteration = 0; iteration < context.VelocityIterations; iteration++)
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    ContactManifold contact = contacts[i];

                    if (
                        !TryGetBodyById(bodies, contact.BodyAId, out RigidBodyLS bodyA)
                        || !TryGetBodyById(bodies, contact.BodyBId, out RigidBodyLS bodyB)
                    )
                        continue;

                    // Get material properties from colliders
                    world.TryGetCollider(contact.ColliderAId, out ColliderLS colliderA);
                    world.TryGetCollider(contact.ColliderBId, out ColliderLS colliderB);

                    // Skip physics resolution for trigger colliders
                    if (colliderA.IsTrigger || colliderB.IsTrigger)
                        continue;

                    FP restitution = FPMath.Min(colliderA.Restitution, colliderB.Restitution);
                    FP friction = FPMath.Max(colliderA.Friction, colliderB.Friction);

                    // Solve for each contact point
                    for (int cp = 0; cp < contact.ContactCount; cp++)
                    {
                        FPVector2 contactPoint =
                            cp == 0 ? contact.ContactPoint1 : contact.ContactPoint2;
                        SolveContactPoint(
                            ref bodyA,
                            ref bodyB,
                            contactPoint,
                            contact.Normal,
                            restitution,
                            friction
                        );
                    }

                    SetBodyById(bodies, contact.BodyAId, bodyA);
                    SetBodyById(bodies, contact.BodyBId, bodyB);
                }
            }

            // Position correction
            for (int iteration = 0; iteration < context.PositionIterations; iteration++)
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    ContactManifold contact = contacts[i];

                    if (
                        !TryGetBodyById(bodies, contact.BodyAId, out RigidBodyLS bodyA)
                        || !TryGetBodyById(bodies, contact.BodyBId, out RigidBodyLS bodyB)
                    )
                        continue;

                    // Skip position correction for trigger colliders
                    world.TryGetCollider(contact.ColliderAId, out ColliderLS colliderA);
                    world.TryGetCollider(contact.ColliderBId, out ColliderLS colliderB);
                    if (colliderA.IsTrigger || colliderB.IsTrigger)
                        continue;

                    PositionCorrection(ref bodyA, ref bodyB, contact.Normal, contact.Penetration);

                    SetBodyById(bodies, contact.BodyAId, bodyA);
                    SetBodyById(bodies, contact.BodyBId, bodyB);
                }
            }
        }

        private static void SolveContactPoint(
            ref RigidBodyLS bodyA,
            ref RigidBodyLS bodyB,
            FPVector2 contactPoint,
            FPVector2 normal,
            FP restitution,
            FP friction
        )
        {
            FPVector2 rA = contactPoint - bodyA.Position;
            FPVector2 rB = contactPoint - bodyB.Position;

            FPVector2 vA =
                bodyA.LinearVelocity + FPVector2.Perpendicular(rA) * bodyA.AngularVelocity;
            FPVector2 vB =
                bodyB.LinearVelocity + FPVector2.Perpendicular(rB) * bodyB.AngularVelocity;
            FPVector2 relativeVel = vB - vA;

            FP velAlongNormal = FPVector2.Dot(relativeVel, normal);

            if (velAlongNormal > FP.Zero)
                return;

            FP rAcrossN = FPVector2.Cross(rA, normal);
            FP rBcrossN = FPVector2.Cross(rB, normal);

            FP invMassSum =
                bodyA.InverseMass
                + bodyB.InverseMass
                + rAcrossN * rAcrossN * bodyA.InverseInertia
                + rBcrossN * rBcrossN * bodyB.InverseInertia;

            if (invMassSum <= FP.Epsilon)
                return;

            FP j = -(FP.One + restitution) * velAlongNormal / invMassSum;

            FPVector2 impulse = normal * j;
            bodyA.ApplyImpulse(-impulse, rA);
            bodyB.ApplyImpulse(impulse, rB);

            // Friction
            vA = bodyA.LinearVelocity + FPVector2.Perpendicular(rA) * bodyA.AngularVelocity;
            vB = bodyB.LinearVelocity + FPVector2.Perpendicular(rB) * bodyB.AngularVelocity;
            relativeVel = vB - vA;

            FPVector2 tangent = relativeVel - normal * FPVector2.Dot(relativeVel, normal);
            if (tangent.SqrMagnitude > FP.Epsilon)
            {
                tangent = tangent.Normalized;

                FP velAlongTangent = FPVector2.Dot(relativeVel, tangent);

                FP rAcrossT = FPVector2.Cross(rA, tangent);
                FP rBcrossT = FPVector2.Cross(rB, tangent);

                FP invMassSumTangent =
                    bodyA.InverseMass
                    + bodyB.InverseMass
                    + rAcrossT * rAcrossT * bodyA.InverseInertia
                    + rBcrossT * rBcrossT * bodyB.InverseInertia;

                if (invMassSumTangent > FP.Epsilon)
                {
                    FP jt = -velAlongTangent / invMassSumTangent;

                    FP maxFriction = FPMath.Abs(j) * friction;
                    jt = FPMath.Clamp(jt, -maxFriction, maxFriction);

                    FPVector2 frictionImpulse = tangent * jt;
                    bodyA.ApplyImpulse(-frictionImpulse, rA);
                    bodyB.ApplyImpulse(frictionImpulse, rB);
                }
            }
        }

        private static void PositionCorrection(
            ref RigidBodyLS bodyA,
            ref RigidBodyLS bodyB,
            FPVector2 normal,
            FP penetration
        )
        {
            const float slop = 0.01f;
            const float percent = 0.4f;

            FP correctionAmount =
                FPMath.Max(penetration - FP.FromFloat(slop), FP.Zero) * FP.FromFloat(percent);

            FP totalInverseMass = bodyA.InverseMass + bodyB.InverseMass;
            if (totalInverseMass <= FP.Epsilon)
                return;

            FPVector2 correction = normal * (correctionAmount / totalInverseMass);

            if (bodyA.BodyType == BodyType.Dynamic)
                bodyA.Position = bodyA.Position - correction * bodyA.InverseMass;

            if (bodyB.BodyType == BodyType.Dynamic)
                bodyB.Position = bodyB.Position + correction * bodyB.InverseMass;
        }

        private static bool TryGetBodyById(List<RigidBodyLS> bodies, int id, out RigidBodyLS body)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == id)
                {
                    body = bodies[i];
                    return true;
                }
            }
            body = default;
            return false;
        }

        private static void SetBodyById(List<RigidBodyLS> bodies, int id, RigidBodyLS body)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == id)
                {
                    bodies[i] = body;
                    return;
                }
            }
        }
    }
}
