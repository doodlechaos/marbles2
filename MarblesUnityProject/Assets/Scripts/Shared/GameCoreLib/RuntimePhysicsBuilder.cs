using System.Collections.Generic;
using FPMathLib;
using LockSim;

namespace GameCoreLib
{
    /// <summary>
    /// Builds LockSim physics bodies from GameCore RuntimeObj hierarchies.
    /// Translates authored GameComponents (colliders, rigidbodies, etc.) into physics bodies
    /// and maintains the RuntimeId → PhysicsBinding mapping.
    /// </summary>
    public static class RuntimePhysicsBuilder
    {
        /// <summary>
        /// Build physics bodies for an entire RuntimeObj hierarchy.
        /// </summary>
        /// <param name="root">The root RuntimeObj to process</param>
        /// <param name="sim">The physics world to add bodies to</param>
        /// <param name="physicsBindings">Output: mapping from RuntimeId to PhysicsBinding</param>
        public static void BuildPhysics(
            RuntimeObj root,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            if (root == null || sim == null || physicsBindings == null)
                return;

            ProcessRuntimeObjHierarchy(root, sim, physicsBindings);
        }

        /// <summary>
        /// Add physics bodies for a RuntimeObj and its entire subtree.
        /// Mirrors Unity's behaviour where a Rigidbody + colliders can live
        /// on children of a spawned prefab.
        /// </summary>
        /// <param name="obj">The RuntimeObj to add physics for</param>
        /// <param name="sim">The physics world to add bodies to</param>
        /// <param name="physicsBindings">Output: mapping from RuntimeId to PhysicsBinding</param>
        public static void AddPhysicsBody(
            RuntimeObj obj,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            if (obj == null || sim == null || physicsBindings == null)
                return;

            // Re‑use the same hierarchical traversal that BuildPhysics uses so that
            // spawned RuntimeObj trees (like player marbles) get all of their
            // child colliders/rigidbodies picked up.
            ProcessRuntimeObjHierarchy(obj, sim, physicsBindings);
        }

        private static void ProcessRuntimeObjHierarchy(
            RuntimeObj obj,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            // Process components to create physics bodies
            ProcessComponents(obj, sim, physicsBindings);

            // Recursively process children
            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ProcessRuntimeObjHierarchy(child, sim, physicsBindings);
                }
            }
        }

        /// <summary>
        /// Process strongly-typed components on a RuntimeObj to create physics bodies.
        /// </summary>
        private static void ProcessComponents(
            RuntimeObj obj,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            if (obj.GameComponents == null || obj.GameComponents.Count == 0)
                return;

            // Find collider and rigidbody components
            BoxCollider2DComponent boxCollider = null;
            CircleCollider2DComponent circleCollider = null;
            Rigidbody2DComponent rigidbody = null;

            foreach (var component in obj.GameComponents)
            {
                switch (component)
                {
                    case BoxCollider2DComponent box:
                        boxCollider = box;
                        break;
                    case CircleCollider2DComponent circle:
                        circleCollider = circle;
                        break;
                    case Rigidbody2DComponent rb:
                        rigidbody = rb;
                        break;
                }
            }

            // Create physics body if there's a collider
            if (boxCollider != null)
            {
                CreatePhysicsBody(obj, boxCollider, rigidbody, sim, physicsBindings);
            }
            else if (circleCollider != null)
            {
                CreatePhysicsBody(obj, circleCollider, rigidbody, sim, physicsBindings);
            }
        }

        /// <summary>
        /// Create a physics body from a BoxCollider2DComponent.
        /// </summary>
        private static void CreatePhysicsBody(
            RuntimeObj obj,
            BoxCollider2DComponent collider,
            Rigidbody2DComponent rigidbody,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            // Extract the Z rotation for physics (twist component around Z axis)
            // We use the twist for physics initialization, and store the swing for sync
            FPQuaternion currentRotation = obj.Transform.LocalRotation;
            FPQuaternion twist = FPQuaternion.ExtractTwist(currentRotation, FPVector3.Forward);
            FPQuaternion swing = FPQuaternion.ExtractSwing(currentRotation, FPVector3.Forward);

            // Get the Z angle from the twist quaternion for physics
            // twist = (0, 0, sin(θ/2), cos(θ/2)) for rotation around Z
            // θ = 2 * atan2(z, w)
            FP rotationRad = FP.Two * FPMath.Atan2(twist.Z, twist.W);

            // Apply collider offset rotated by Z rotation
            FPVector2 rotatedOffset = FPVector2.Rotate(collider.Offset, rotationRad);
            FPVector2 position = new FPVector2(
                obj.Transform.Position.X + rotatedOffset.X,
                obj.Transform.Position.Y + rotatedOffset.Y
            );

            // Determine if static or dynamic
            bool isStatic = rigidbody == null || rigidbody.BodyType == Rigidbody2DType.Static;
            FP mass = rigidbody?.Mass ?? FP.One;

            // Create body
            RigidBodyLS body;
            if (isStatic)
            {
                body = RigidBodyLS.CreateStatic(0, position, rotationRad);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotationRad, mass);
            }

            // Apply scale to size
            FP width = collider.Size.X * FPMath.Abs(obj.Transform.LossyScale.X);
            FP height = collider.Size.Y * FPMath.Abs(obj.Transform.LossyScale.Y);
            body.SetBoxShape(width, height);

            // Set material properties
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);

            // Add body to world and store binding
            int bodyId = sim.AddBody(body);
            obj.PhysicsBodyId = bodyId; // Store on RuntimeObj for safe teleporting
            physicsBindings[obj.RuntimeId] = new PhysicsBinding
            {
                BodyId = bodyId,
                // Store swing for dynamic bodies; static bodies get identity
                BaseSwing = isStatic ? FPQuaternion.Identity : swing,
            };

            Logger.Log(
                $"Created box physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, Size=({width}, {height})"
            );
        }

        /// <summary>
        /// Create a physics body from a CircleCollider2DComponent.
        /// </summary>
        private static void CreatePhysicsBody(
            RuntimeObj obj,
            CircleCollider2DComponent collider,
            Rigidbody2DComponent rigidbody,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            // Extract the Z rotation for physics (twist component around Z axis)
            // We use the twist for physics initialization, and store the swing for sync
            FPQuaternion currentRotation = obj.Transform.LocalRotation;
            FPQuaternion twist = FPQuaternion.ExtractTwist(currentRotation, FPVector3.Forward);
            FPQuaternion swing = FPQuaternion.ExtractSwing(currentRotation, FPVector3.Forward);

            // Get the Z angle from the twist quaternion for physics
            // twist = (0, 0, sin(θ/2), cos(θ/2)) for rotation around Z
            // θ = 2 * atan2(z, w)
            FP rotationRad = FP.Two * FPMath.Atan2(twist.Z, twist.W);

            // Apply collider offset rotated by Z rotation
            FPVector2 rotatedOffset = FPVector2.Rotate(collider.Offset, rotationRad);
            FPVector2 position = new FPVector2(
                obj.Transform.Position.X + rotatedOffset.X,
                obj.Transform.Position.Y + rotatedOffset.Y
            );

            // Determine if static or dynamic
            bool isStatic = rigidbody == null || rigidbody.BodyType == Rigidbody2DType.Static;
            FP mass = rigidbody?.Mass ?? FP.One;

            // Create body
            RigidBodyLS body;
            if (isStatic)
            {
                body = RigidBodyLS.CreateStatic(0, position, rotationRad);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotationRad, mass);
            }

            // Apply scale to radius (use max of x/y scale)
            FP scaleX = FPMath.Abs(obj.Transform.LossyScale.X);
            FP scaleY = FPMath.Abs(obj.Transform.LossyScale.Y);
            FP radius = collider.Radius * FPMath.Max(scaleX, scaleY);
            body.SetCircleShape(radius);

            // Set material properties
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);

            // Add body to world and store binding
            int bodyId = sim.AddBody(body);
            obj.PhysicsBodyId = bodyId; // Store on RuntimeObj for safe teleporting
            physicsBindings[obj.RuntimeId] = new PhysicsBinding
            {
                BodyId = bodyId,
                // Store swing for dynamic bodies; static bodies get identity
                BaseSwing = isStatic ? FPQuaternion.Identity : swing,
            };

            Logger.Log(
                $"Created circle physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, Radius={radius}"
            );
        }
    }
}
