using FPMathLib;
using System.Collections.Generic;

namespace LockSim
{
    public static class Integration
    {
        public static void IntegrateForces(World world, FP deltaTime)
        {
            List<RigidBodyLS> bodies = world.GetBodiesMutable();

            for (int i = 0; i < bodies.Count; i++)
            {
                RigidBodyLS body = bodies[i];

                if (body.BodyType != BodyType.Dynamic)
                    continue;

                // Apply gravity
                body.Force = body.Force + world.Gravity * body.Mass;

                // Integrate velocity: v = v + (F/m) * dt
                body.LinearVelocity = body.LinearVelocity + body.Force * body.InverseMass * deltaTime;
                body.AngularVelocity = body.AngularVelocity + body.Torque * body.InverseInertia * deltaTime;

                // Apply damping
                FP linearDamping = FP.FromFloat(0.99f);
                FP angularDamping = FP.FromFloat(0.98f);
                body.LinearVelocity = body.LinearVelocity * linearDamping;
                body.AngularVelocity = body.AngularVelocity * angularDamping;

                // Clear forces
                body.ClearForces();

                bodies[i] = body;
            }
        }

        public static void IntegrateVelocities(World world, FP deltaTime)
        {
            List<RigidBodyLS> bodies = world.GetBodiesMutable();

            for (int i = 0; i < bodies.Count; i++)
            {
                RigidBodyLS body = bodies[i];

                if (body.BodyType != BodyType.Dynamic)
                    continue;

                // Integrate position: p = p + v * dt
                body.Position = body.Position + body.LinearVelocity * deltaTime;
                body.Rotation = body.Rotation + body.AngularVelocity * deltaTime;

                // Normalize rotation to [-PI, PI]
                while (body.Rotation > FP.Pi)
                    body.Rotation = body.Rotation - FP.Pi * FP.Two;
                while (body.Rotation < -FP.Pi)
                    body.Rotation = body.Rotation + FP.Pi * FP.Two;

                bodies[i] = body;
            }
        }
    }
}

