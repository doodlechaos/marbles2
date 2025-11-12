/// A collision detection pipeline that can be used without full physics simulation.
///
/// This runs only collision detection (broad-phase + narrow-phase) without dynamics/forces.
/// Use when you want to detect collisions but don't need physics simulation.
///
/// **For full physics**, use [`PhysicsPipeline`](crate::pipeline::PhysicsPipeline) instead which includes this internally.
///
/// ## Use cases
///
/// - Collision detection in a non-physics game
/// - Custom physics integration where you handle forces yourself
/// - Debugging collision detection separately from dynamics
///
/// Like PhysicsPipeline, this only holds temporary buffers. Reuse the same instance for performance.
// NOTE: this contains only workspace data, so there is no point in making this serializable.
