/// A query system for performing spatial queries on your physics world (raycasts, shape casts, intersections).
///
/// Think of this as a "search engine" for your physics world. Use it to answer questions like:
/// - "What does this ray hit?"
/// - "What colliders are near this point?"
/// - "If I move this shape, what will it collide with?"
///
/// Get a QueryPipeline from your [`BroadPhaseBvh`] using [`as_query_pipeline()`](BroadPhaseBvh::as_query_pipeline).
///
/// # Example
/// ```
/// # use rapier3d::prelude::*;
/// # let mut bodies = RigidBodySet::new();
/// # let mut colliders = ColliderSet::new();
/// # let broad_phase = BroadPhaseBvh::new();
/// # let narrow_phase = NarrowPhase::new();
/// # let ground = bodies.insert(RigidBodyBuilder::fixed());
/// # colliders.insert_with_parent(ColliderBuilder::cuboid(10.0, 0.1, 10.0), ground, &mut bodies);
/// let query_pipeline = broad_phase.as_query_pipeline(
///     narrow_phase.query_dispatcher(),
///     &bodies,
///     &colliders,
///     QueryFilter::default()
/// );
///
/// // Cast a ray downward
/// let ray = Ray::new(point![0.0, 10.0, 0.0], vector![0.0, -1.0, 0.0]);
/// if let Some((handle, toi)) = query_pipeline.cast_ray(&ray, Real::MAX, false) {
///     println!("Hit collider {:?} at distance {}", handle, toi);
/// }
/// ```
