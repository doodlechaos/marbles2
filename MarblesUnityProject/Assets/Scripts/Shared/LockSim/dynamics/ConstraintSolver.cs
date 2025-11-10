using FPMath;
using System.Collections.Generic;

namespace LockSim
{
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
                    
                    RigidBodyLS bodyA = GetBodyById(bodies, contact.BodyAId);
                    RigidBodyLS bodyB = GetBodyById(bodies, contact.BodyBId);

                    // Solve for each contact point
                    for (int cp = 0; cp < contact.ContactCount; cp++)
                    {
                        FPVector2 contactPoint = cp == 0 ? contact.ContactPoint1 : contact.ContactPoint2;
                        
                        SolveContactPoint(ref bodyA, ref bodyB, contactPoint, contact.Normal);
                    }

                    // Write back
                    SetBodyById(bodies, contact.BodyAId, bodyA);
                    SetBodyById(bodies, contact.BodyBId, bodyB);
                }
            }

            // Position correction to prevent sinking
            for (int iteration = 0; iteration < context.PositionIterations; iteration++)
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    ContactManifold contact = contacts[i];
                    
                    RigidBodyLS bodyA = GetBodyById(bodies, contact.BodyAId);
                    RigidBodyLS bodyB = GetBodyById(bodies, contact.BodyBId);

                    PositionCorrection(ref bodyA, ref bodyB, contact.Normal, contact.Penetration);

                    SetBodyById(bodies, contact.BodyAId, bodyA);
                    SetBodyById(bodies, contact.BodyBId, bodyB);
                }
            }
        }

        private static void SolveContactPoint(ref RigidBodyLS bodyA, ref RigidBodyLS bodyB, 
            FPVector2 contactPoint, FPVector2 normal)
        {
            // Contact vectors from center of mass to contact point
            FPVector2 rA = contactPoint - bodyA.Position;
            FPVector2 rB = contactPoint - bodyB.Position;

            // Relative velocity at contact point
            FPVector2 vA = bodyA.LinearVelocity + FPVector2.Perpendicular(rA) * bodyA.AngularVelocity;
            FPVector2 vB = bodyB.LinearVelocity + FPVector2.Perpendicular(rB) * bodyB.AngularVelocity;
            FPVector2 relativeVel = vB - vA;

            // Velocity along normal
            FP velAlongNormal = FPVector2.Dot(relativeVel, normal);

            // Do not resolve if velocities are separating
            if (velAlongNormal > FP.Zero)
                return;

            // Calculate restitution (bounciness)
            FP restitution = FPMath.Min(bodyA.Restitution, bodyB.Restitution);

            // Calculate impulse scalar
            FP rAcrossN = FPVector2.Cross(rA, normal);
            FP rBcrossN = FPVector2.Cross(rB, normal);
            
            FP invMassSum = bodyA.InverseMass + bodyB.InverseMass +
                           rAcrossN * rAcrossN * bodyA.InverseInertia +
                           rBcrossN * rBcrossN * bodyB.InverseInertia;

            if (invMassSum <= FP.Epsilon)
                return;

            FP j = -(FP.One + restitution) * velAlongNormal / invMassSum;

            // Apply impulse
            FPVector2 impulse = normal * j;
            bodyA.ApplyImpulse(-impulse, rA);
            bodyB.ApplyImpulse(impulse, rB);

            // Friction
            // Recalculate relative velocity after normal impulse
            vA = bodyA.LinearVelocity + FPVector2.Perpendicular(rA) * bodyA.AngularVelocity;
            vB = bodyB.LinearVelocity + FPVector2.Perpendicular(rB) * bodyB.AngularVelocity;
            relativeVel = vB - vA;

            // Tangent vector
            FPVector2 tangent = relativeVel - normal * FPVector2.Dot(relativeVel, normal);
            if (tangent.SqrMagnitude > FP.Epsilon)
            {
                tangent = tangent.Normalized;

                FP velAlongTangent = FPVector2.Dot(relativeVel, tangent);

                FP rAcrossT = FPVector2.Cross(rA, tangent);
                FP rBcrossT = FPVector2.Cross(rB, tangent);

                FP invMassSumTangent = bodyA.InverseMass + bodyB.InverseMass +
                                       rAcrossT * rAcrossT * bodyA.InverseInertia +
                                       rBcrossT * rBcrossT * bodyB.InverseInertia;

                if (invMassSumTangent > FP.Epsilon)
                {
                    FP jt = -velAlongTangent / invMassSumTangent;

                    // Coulomb's law: friction impulse should not exceed mu * normal impulse
                    FP friction = FPMath.Max(bodyA.Friction, bodyB.Friction);
                    FP maxFriction = FPMath.Abs(j) * friction;
                    jt = FPMath.Clamp(jt, -maxFriction, maxFriction);

                    FPVector2 frictionImpulse = tangent * jt;
                    bodyA.ApplyImpulse(-frictionImpulse, rA);
                    bodyB.ApplyImpulse(frictionImpulse, rB);
                }
            }
        }

        private static void PositionCorrection(ref RigidBodyLS bodyA, ref RigidBodyLS bodyB, 
            FPVector2 normal, FP penetration)
        {
            const float slop = 0.01f; // Penetration allowance
            const float percent = 0.4f; // Penetration correction percentage

            FP correctionAmount = FPMath.Max(penetration - FP.FromFloat(slop), FP.Zero) * FP.FromFloat(percent);
            
            FP totalInverseMass = bodyA.InverseMass + bodyB.InverseMass;
            if (totalInverseMass <= FP.Epsilon)
                return;

            FPVector2 correction = normal * (correctionAmount / totalInverseMass);

            if (bodyA.BodyType == BodyType.Dynamic)
                bodyA.Position = bodyA.Position - correction * bodyA.InverseMass;
            
            if (bodyB.BodyType == BodyType.Dynamic)
                bodyB.Position = bodyB.Position + correction * bodyB.InverseMass;
        }

        private static RigidBodyLS GetBodyById(List<RigidBodyLS> bodies, int id)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Id == id)
                    return bodies[i];
            }
            throw new System.ArgumentException($"Body with ID {id} not found");
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

