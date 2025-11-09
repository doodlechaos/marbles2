namespace LockSim
{
    /// <summary>
    /// Main physics engine that steps the simulation forward
    /// </summary>
    public static class PhysicsEngine
    {
        public static void Step(World world, FP deltaTime)
        {
            // 1. Integrate forces (apply gravity and forces to velocities)
            Integration.IntegrateForces(world, deltaTime);

            // 2. Detect collisions
            CollisionDetection.DetectCollisions(world);

            // 3. Solve collision constraints (resolve contacts)
            ConstraintSolver.SolveContacts(world, deltaTime);

            // 4. Integrate velocities (update positions based on velocities)
            Integration.IntegrateVelocities(world, deltaTime);
        }
    }
}

