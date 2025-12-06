/// The main physics simulation engine that runs your physics world forward in time.
///
/// Think of this as the "game loop" for your physics simulation. Each frame, you call
/// [`PhysicsPipeline::step`] to advance the simulation by one timestep. This structure
/// handles all the complex physics calculations: detecting collisions between objects,
/// resolving contacts so objects don't overlap, and updating positions and velocities.
///
/// ## Performance note
/// This structure only contains temporary working memory (scratch buffers). You can create
/// a new one anytime, but it's more efficient to reuse the same instance across frames
/// since Rapier can reuse allocated memory.
///
/// ## How it works (simplified)
/// Rapier uses a time-stepping approach where each step involves:
/// 1. **Collision detection**: Find which objects are touching or overlapping
/// 2. **Constraint solving**: Calculate forces to prevent overlaps and enforce joint constraints
/// 3. **Integration**: Update object positions and velocities based on forces and gravity
/// 4. **Position correction**: Fix any remaining overlaps that might have occurred
// NOTE: this contains only workspace data, so there is no point in making this serializable.

using FPMathLib;

namespace LockSim
{
    /// <summary>
    /// Main physics pipeline that steps the simulation forward.
    /// Mirrors Rapier's PhysicsPipeline role.
    /// </summary>
    public static class PhysicsPipeline
    {
        public static void Step(World world, FP deltaTime, WorldSimulationContext context = null)
        {
            // Create default context if none provided (for backwards compatibility)
            if (context == null)
            {
                context = new WorldSimulationContext();
            }

            //GameCoreLib.Logger.Log($"[PhysicsPipeline] Stepping with deltaTime: {deltaTime}");
            //GameCoreLib.Logger.Log($"[PhysicsPipeline] World has {world.Bodies.Count} bodies");

            // 1. Integrate forces (apply gravity and forces to velocities)
            Integration.IntegrateForces(world, deltaTime);

            // 2. Detect collisions (broad + narrow handled inside NarrowPhase)
            NarrowPhase.DetectCollisions(world, context);

            // 3. Process collision events (Enter/Stay/Exit for both collisions and triggers)
            CollisionEventProcessor.ProcessEvents(world, context);

            // 4. Solve collision constraints (resolve contacts, skips triggers)
            ConstraintSolver.SolveContacts(world, deltaTime, context);

            // 5. Integrate velocities (update positions based on velocities)
            Integration.IntegrateVelocities(world, deltaTime);
        }
    }
}
