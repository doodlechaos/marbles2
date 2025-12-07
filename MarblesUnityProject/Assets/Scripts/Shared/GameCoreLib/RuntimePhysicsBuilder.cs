using System.Collections.Generic;
using FPMathLib;
using LockSim;

namespace GameCoreLib
{
    /// <summary>
    /// Builds LockSim physics bodies and colliders from GameCore RuntimeObj hierarchies.
    /// Translates authored GameComponents (colliders, rigidbodies, etc.) into physics entities
    /// and maintains the RuntimeId → PhysicsBinding mapping.
    /// </summary>
    public static class RuntimePhysicsBuilder
    {
        /// <summary>
        /// Build physics bodies for an entire RuntimeObj hierarchy.
        /// </summary>
        public static void BuildPhysics(
            GameCoreObj root,
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
        /// </summary>
        public static void AddPhysicsBody(
            GameCoreObj obj,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            if (obj == null || sim == null || physicsBindings == null)
                return;

            ProcessRuntimeObjHierarchy(obj, sim, physicsBindings);
        }

        private static void ProcessRuntimeObjHierarchy(
            GameCoreObj obj,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            ProcessComponents(obj, sim, physicsBindings);

            if (obj.Children != null)
            {
                foreach (var child in obj.Children)
                {
                    ProcessRuntimeObjHierarchy(child, sim, physicsBindings);
                }
            }
        }

        private static void ProcessComponents(
            GameCoreObj obj,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            if (obj.GameComponents == null || obj.GameComponents.Count == 0)
                return;

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

            if (boxCollider != null)
            {
                CreatePhysicsBody(obj, boxCollider, rigidbody, sim, physicsBindings);
            }
            else if (circleCollider != null)
            {
                CreatePhysicsBody(obj, circleCollider, rigidbody, sim, physicsBindings);
            }
        }

        private static void CreatePhysicsBody(
            GameCoreObj obj,
            BoxCollider2DComponent colliderComponent,
            Rigidbody2DComponent rigidbody,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            FPQuaternion currentRotation = obj.Transform.LocalRotation;
            FPQuaternion twist = FPQuaternion.ExtractTwist(currentRotation, FPVector3.Forward);
            FPQuaternion swing = FPQuaternion.ExtractSwing(currentRotation, FPVector3.Forward);
            FP rotationRad = FP.Two * FPMath.Atan2(twist.Z, twist.W);

            FPVector2 rotatedOffset = FPVector2.Rotate(colliderComponent.Offset, rotationRad);
            FPVector2 position = new FPVector2(
                obj.Transform.Position.X + rotatedOffset.X,
                obj.Transform.Position.Y + rotatedOffset.Y
            );

            bool isStatic = rigidbody == null || rigidbody.BodyType == Rigidbody2DType.Static;
            FP mass = rigidbody?.Mass ?? FP.One;

            // Create body
            RigidBodyLS body = isStatic
                ? RigidBodyLS.CreateStatic(0, position, rotationRad)
                : RigidBodyLS.CreateDynamic(0, position, rotationRad, mass);

            int bodyId = sim.AddBody(body);

            // Create and attach collider with authored material properties
            FP width = colliderComponent.Size.X * FPMath.Abs(obj.Transform.LossyScale.X);
            FP height = colliderComponent.Size.Y * FPMath.Abs(obj.Transform.LossyScale.Y);

            var collider = ColliderLS.CreateBox(
                0,
                width,
                height,
                bodyId,
                colliderComponent.IsTrigger
            );
            collider.Friction = colliderComponent.Friction;
            collider.Restitution = colliderComponent.Restitution;

            int colliderId = sim.AddCollider(collider);

            // Update body inertia from collider
            body = sim.GetBody(bodyId);
            body.SetInertiaFromCollider(collider);
            sim.SetBody(bodyId, body);

            obj.PhysicsBodyId = bodyId;
            physicsBindings[obj.RuntimeId] = new PhysicsBinding
            {
                BodyId = bodyId,
                ColliderId = colliderId,
                BaseSwing = isStatic ? FPQuaternion.Identity : swing,
            };

            Logger.Log(
                $"Created box physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, ColliderId={colliderId}, Size=({width}, {height})"
            );
        }

        private static void CreatePhysicsBody(
            GameCoreObj obj,
            CircleCollider2DComponent colliderComponent,
            Rigidbody2DComponent rigidbody,
            World sim,
            Dictionary<ulong, PhysicsBinding> physicsBindings
        )
        {
            FPQuaternion currentRotation = obj.Transform.LocalRotation;
            FPQuaternion twist = FPQuaternion.ExtractTwist(currentRotation, FPVector3.Forward);
            FPQuaternion swing = FPQuaternion.ExtractSwing(currentRotation, FPVector3.Forward);
            FP rotationRad = FP.Two * FPMath.Atan2(twist.Z, twist.W);

            FPVector2 rotatedOffset = FPVector2.Rotate(colliderComponent.Offset, rotationRad);
            FPVector2 position = new FPVector2(
                obj.Transform.Position.X + rotatedOffset.X,
                obj.Transform.Position.Y + rotatedOffset.Y
            );

            bool isStatic = rigidbody == null || rigidbody.BodyType == Rigidbody2DType.Static;
            FP mass = rigidbody?.Mass ?? FP.One;

            // Create body
            RigidBodyLS body = isStatic
                ? RigidBodyLS.CreateStatic(0, position, rotationRad)
                : RigidBodyLS.CreateDynamic(0, position, rotationRad, mass);

            int bodyId = sim.AddBody(body);

            // Create and attach collider with authored material properties
            FP scaleX = FPMath.Abs(obj.Transform.LossyScale.X);
            FP scaleY = FPMath.Abs(obj.Transform.LossyScale.Y);
            FP radius = colliderComponent.Radius * FPMath.Max(scaleX, scaleY);

            var collider = ColliderLS.CreateCircle(0, radius, bodyId, colliderComponent.IsTrigger);
            collider.Friction = colliderComponent.Friction;
            collider.Restitution = colliderComponent.Restitution;

            int colliderId = sim.AddCollider(collider);

            // Update body inertia from collider
            body = sim.GetBody(bodyId);
            body.SetInertiaFromCollider(collider);
            sim.SetBody(bodyId, body);

            obj.PhysicsBodyId = bodyId;
            physicsBindings[obj.RuntimeId] = new PhysicsBinding
            {
                BodyId = bodyId,
                ColliderId = colliderId,
                BaseSwing = isStatic ? FPQuaternion.Identity : swing,
            };

            Logger.Log(
                $"Created circle physics body for {obj.Name}: RuntimeId={obj.RuntimeId}, BodyId={bodyId}, ColliderId={colliderId}, Radius={radius}"
            );
        }
    }
}
