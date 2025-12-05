using System.Collections.Generic;
using FPMathLib;
using LockSim;

namespace GameCoreLib
{
    /// <summary>
    /// Builds LockSim physics bodies from GameCore RuntimeObj hierarchies.
    /// Translates authored GameComponents (colliders, rigidbodies, etc.) into physics bodies
    /// and maintains the RuntimeId → BodyId mapping.
    /// </summary>
    public static class RuntimePhysicsBuilder
    {
        /// <summary>
        /// Build physics bodies for an entire RuntimeObj hierarchy.
        /// </summary>
        public static void BuildPhysics(
            RuntimeObj root,
            World sim,
            Dictionary<ulong, int> runtimeIdToBodyId
        )
        {
            if (root == null || sim == null || runtimeIdToBodyId == null)
                return;

            ProcessRuntimeObjHierarchy(root, sim, runtimeIdToBodyId);
        }

        /// <summary>
        /// Add physics bodies for a single RuntimeObj (and optionally its children, if you call BuildPhysics).
        /// </summary>
        public static void AddPhysicsBody(
            RuntimeObj obj,
            World sim,
            Dictionary<ulong, int> runtimeIdToBodyId
        )
        {
            if (obj == null || sim == null || runtimeIdToBodyId == null)
                return;

            ProcessComponents(obj, sim, runtimeIdToBodyId);
        }

        private static void ProcessRuntimeObjHierarchy(
            RuntimeObj obj,
            World sim,
            Dictionary<ulong, int> runtimeIdToBodyId
        )
        {
            // Process components to create physics bodies
            ProcessComponents(obj, sim, runtimeIdToBodyId);

            // Recursively process children
            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ProcessRuntimeObjHierarchy(child, sim, runtimeIdToBodyId);
                }
            }
        }

        /// <summary>
        /// Process strongly-typed components on a RuntimeObj to create physics bodies.
        /// </summary>
        private static void ProcessComponents(
            RuntimeObj obj,
            World sim,
            Dictionary<ulong, int> runtimeIdToBodyId
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
                CreatePhysicsBody(obj, boxCollider, rigidbody, sim, runtimeIdToBodyId);
            }
            else if (circleCollider != null)
            {
                CreatePhysicsBody(obj, circleCollider, rigidbody, sim, runtimeIdToBodyId);
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
            Dictionary<ulong, int> runtimeIdToBodyId
        )
        {
            FP rotation = obj.Transform.EulerAngles.Z;

            // Apply collider offset rotated by Z rotation
            FPVector2 rotatedOffset = FPVector2.Rotate(collider.Offset, rotation);
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
                body = RigidBodyLS.CreateStatic(0, position, rotation);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotation, mass);
            }

            // Apply scale to size
            FP width = collider.Size.X * FPMath.Abs(obj.Transform.LossyScale.X);
            FP height = collider.Size.Y * FPMath.Abs(obj.Transform.LossyScale.Y);
            body.SetBoxShape(width, height);

            // Set material properties
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);

            // Add body to world and store mapping
            int bodyId = sim.AddBody(body);
            runtimeIdToBodyId[obj.RuntimeId] = bodyId;

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
            Dictionary<ulong, int> runtimeIdToBodyId
        )
        {
            FP rotation = obj.Transform.EulerAngles.Z;

            // Apply collider offset rotated by Z rotation
            FPVector2 rotatedOffset = FPVector2.Rotate(collider.Offset, rotation);
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
                body = RigidBodyLS.CreateStatic(0, position, rotation);
            }
            else
            {
                body = RigidBodyLS.CreateDynamic(0, position, rotation, mass);
            }

            // Apply scale to radius (use max of x/y scale)
            FP scaleX = FPMath.Abs(obj.Transform.LossyScale.X);
            FP scaleY = FPMath.Abs(obj.Transform.LossyScale.Y);
            FP radius = collider.Radius * FPMath.Max(scaleX, scaleY);
            body.SetCircleShape(radius);

            // Set material properties
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);

            // Add body to world and store mapping
            int bodyId = sim.AddBody(body);
            runtimeIdToBodyId[obj.RuntimeId] = bodyId;

            Logger.Log(
                $"Created circle physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, Radius={radius}"
            );
        }
    }
}
