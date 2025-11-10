/// The narrow-phase collision detector that computes precise contact points between colliders.
///
/// After the broad-phase quickly filters out distant object pairs, the narrow-phase performs
/// detailed geometric computations to find exact:
/// - Contact points (where surfaces touch)
/// - Contact normals (which direction surfaces face)
/// - Penetration depths (how much objects overlap)
///
/// You typically don't interact with this directly - it's managed by [`PhysicsPipeline::step`](crate::pipeline::PhysicsPipeline::step).
/// However, you can access it to query contact information or intersection state between specific colliders.
///
/// **For spatial queries** (raycasts, shape casts), use [`QueryPipeline`](crate::pipeline::QueryPipeline) instead.

using System.Collections.Generic;

using FPMath;

namespace LockSim
{
    /// <summary>
    /// Performs collision detection. For simplicity, this currently includes both broad-phase and narrow-phase.
    /// </summary>
    public static class NarrowPhase
    {
        public static void DetectCollisions(World world, WorldSimulationContext context)
        {
            List<RigidBodyLS> bodies = world.GetBodiesMutable();
            List<ContactManifold> contacts = context.GetContactsMutable();
            contacts.Clear();

            // Broad phase + narrow phase (O(n²) for simplicity)
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    RigidBodyLS bodyA = bodies[i];
                    RigidBodyLS bodyB = bodies[j];

                    // Skip if both are static
                    if (bodyA.BodyType == BodyType.Static && bodyB.BodyType == BodyType.Static)
                        continue;

                    // Broad phase: AABB test
                    AABB aabbA = bodyA.ComputeAABB();
                    AABB aabbB = bodyB.ComputeAABB();

                    if (!aabbA.Overlaps(aabbB))
                        continue;

                    // Narrow phase: shape-specific collision
                    ContactManifold manifold = new ContactManifold(bodyA.Id, bodyB.Id);

                    bool colliding = false;
                    if (bodyA.ShapeType == ShapeType.Box && bodyB.ShapeType == ShapeType.Box)
                    {
                        colliding = BoxVsBox(ref bodyA, ref bodyB, ref manifold);
                    }
                    else if (bodyA.ShapeType == ShapeType.Circle && bodyB.ShapeType == ShapeType.Circle)
                    {
                        colliding = CircleVsCircle(ref bodyA, ref bodyB, ref manifold);
                    }
                    else if (bodyA.ShapeType == ShapeType.Box && bodyB.ShapeType == ShapeType.Circle)
                    {
                        colliding = BoxVsCircle(ref bodyA, ref bodyB, ref manifold);
                    }
                    else if (bodyA.ShapeType == ShapeType.Circle && bodyB.ShapeType == ShapeType.Box)
                    {
                        colliding = CircleVsBox(ref bodyA, ref bodyB, ref manifold);
                    }

                    if (colliding)
                    {
                        contacts.Add(manifold);
                    }
                }
            }
        }

        private static bool CircleVsCircle(ref RigidBodyLS bodyA, ref RigidBodyLS bodyB, ref ContactManifold manifold)
        {
            FP radiusA = bodyA.CircleShape.Radius;
            FP radiusB = bodyB.CircleShape.Radius;

            FPVector2 delta = bodyB.Position - bodyA.Position;
            FP distSqr = delta.SqrMagnitude;
            FP radiusSum = radiusA + radiusB;

            if (distSqr >= radiusSum * radiusSum)
                return false;

            FP dist = FPMath.Sqrt(distSqr);
            
            if (dist > FP.Epsilon)
            {
                manifold.Normal = delta / dist;
            }
            else
            {
                manifold.Normal = FPVector2.Right;
                dist = FP.Zero;
            }

            manifold.Penetration = radiusSum - dist;
            
            // Contact point is on the line between centers
            FPVector2 contactPoint = bodyA.Position + manifold.Normal * radiusA;
            manifold.AddContact(contactPoint);

            return true;
        }

        private static bool BoxVsBox(ref RigidBodyLS bodyA, ref RigidBodyLS bodyB, ref ContactManifold manifold)
        {
            // Separating Axis Theorem (SAT) for oriented boxes
            // Test axes: A's edges (2) and B's edges (2)

            FP cosA = FPMath.Cos(bodyA.Rotation);
            FP sinA = FPMath.Sin(bodyA.Rotation);
            FP cosB = FPMath.Cos(bodyB.Rotation);
            FP sinB = FPMath.Sin(bodyB.Rotation);

            // Axes to test (box edge normals)
            FPVector2[] axes = new FPVector2[4];
            axes[0] = new FPVector2(cosA, sinA);  // A's X axis
            axes[1] = new FPVector2(-sinA, cosA); // A's Y axis
            axes[2] = new FPVector2(cosB, sinB);  // B's X axis
            axes[3] = new FPVector2(-sinB, cosB); // B's Y axis

            FP minPenetration = FP.MaxValue;
            FPVector2 minAxis = FPVector2.Zero;
            int minAxisIndex = -1;

            for (int i = 0; i < 4; i++)
            {
                FPVector2 axis = axes[i];
                
                // Project boxes onto axis
                FP penetration = ProjectAndCheckOverlap(ref bodyA, ref bodyB, axis);
                
                if (penetration <= FP.Zero)
                    return false; // Separating axis found

                if (penetration < minPenetration)
                {
                    minPenetration = penetration;
                    minAxis = axis;
                    minAxisIndex = i;
                }
            }

            // Ensure normal points from A to B
            FPVector2 delta = bodyB.Position - bodyA.Position;
            if (FPVector2.Dot(minAxis, delta) < FP.Zero)
            {
                minAxis = -minAxis;
            }

            manifold.Normal = minAxis;
            manifold.Penetration = minPenetration;

            // Generate a proper contact point on the reference edge instead of midpoint between centers.
            // Choose reference box based on which set of axes produced the minimum penetration.
            bool referenceIsA = (minAxisIndex >= 0 && minAxisIndex < 2);
            RigidBodyLS referenceBody = referenceIsA ? bodyA : bodyB;
            RigidBodyLS incidentBody = referenceIsA ? bodyB : bodyA;

            // The normal for contact generation should point from reference to incident
            // manifold.Normal always points from A to B, so flip it when reference is B
            FPVector2 refToIncNormal = referenceIsA ? manifold.Normal : -manifold.Normal;

            // Get the reference edge (segment) whose outward normal best matches the collision normal.
            GetReferenceEdge(ref referenceBody, refToIncNormal, out FPVector2 refP1, out FPVector2 refP2);

            // Find the incident support point deepest into reference (most negative along refToIncNormal)
            FPVector2[] incidentCorners = GetBoxCorners(ref incidentBody);
            FP bestDot = FP.MaxValue;
            FPVector2 incidentPoint = incidentCorners[0];
            for (int k = 0; k < 4; k++)
            {
                FP d = FPVector2.Dot(incidentCorners[k], refToIncNormal);
                if (d < bestDot)
                {
                    bestDot = d;
                    incidentPoint = incidentCorners[k];
                }
            }

            // Project the incident point onto the reference edge segment
            FPVector2 edge = refP2 - refP1;
            FP edgeLenSqr = FPVector2.Dot(edge, edge);
            FPVector2 contactPoint;
            if (edgeLenSqr > FP.Epsilon)
            {
                FP t = FPVector2.Dot(incidentPoint - refP1, edge) / edgeLenSqr;
                t = FPMath.Clamp(t, FP.Zero, FP.One);
                contactPoint = refP1 + edge * t;
            }
            else
            {
                // Degenerate edge; fall back to reference point
                contactPoint = refP1;
            }

            manifold.AddContact(contactPoint);

            return true;
        }

        private static FP ProjectAndCheckOverlap(ref RigidBodyLS boxA, ref RigidBodyLS boxB, FPVector2 axis)
        {
            // Get the corners of both boxes and project onto axis
            FPVector2[] cornersA = GetBoxCorners(ref boxA);
            FPVector2[] cornersB = GetBoxCorners(ref boxB);

            FP minA = FP.MaxValue;
            FP maxA = FP.MinValue;
            FP minB = FP.MaxValue;
            FP maxB = FP.MinValue;

            for (int i = 0; i < 4; i++)
            {
                FP projA = FPVector2.Dot(cornersA[i], axis);
                minA = FPMath.Min(minA, projA);
                maxA = FPMath.Max(maxA, projA);

                FP projB = FPVector2.Dot(cornersB[i], axis);
                minB = FPMath.Min(minB, projB);
                maxB = FPMath.Max(maxB, projB);
            }

            // Check overlap and return penetration
            if (maxA < minB || maxB < minA)
                return FP.Zero; // No overlap

            return FPMath.Min(maxA - minB, maxB - minA);
        }

        private static FPVector2[] GetBoxCorners(ref RigidBodyLS body)
        {
            FP cos = FPMath.Cos(body.Rotation);
            FP sin = FPMath.Sin(body.Rotation);

            FP hw = body.BoxShape.HalfWidth;
            FP hh = body.BoxShape.HalfHeight;

            FPVector2[] corners = new FPVector2[4];
            
            // Local corners
            FPVector2 c0 = new FPVector2(-hw, -hh);
            FPVector2 c1 = new FPVector2(hw, -hh);
            FPVector2 c2 = new FPVector2(hw, hh);
            FPVector2 c3 = new FPVector2(-hw, hh);

            // Rotate and translate to world space
            corners[0] = body.Position + new FPVector2(c0.X * cos - c0.Y * sin, c0.X * sin + c0.Y * cos);
            corners[1] = body.Position + new FPVector2(c1.X * cos - c1.Y * sin, c1.X * sin + c1.Y * cos);
            corners[2] = body.Position + new FPVector2(c2.X * cos - c2.Y * sin, c2.X * sin + c2.Y * cos);
            corners[3] = body.Position + new FPVector2(c3.X * cos - c3.Y * sin, c3.X * sin + c3.Y * cos);

            return corners;
        }

        private static void GetReferenceEdge(ref RigidBodyLS body, FPVector2 normal, out FPVector2 p1, out FPVector2 p2)
        {
            // Body axes in world space
            FP cos = FPMath.Cos(body.Rotation);
            FP sin = FPMath.Sin(body.Rotation);
            FPVector2 axisX = new FPVector2(cos, sin);
            FPVector2 axisY = new FPVector2(-sin, cos);

            FP hw = body.BoxShape.HalfWidth;
            FP hh = body.BoxShape.HalfHeight;

            // Determine which face normal (±axisX or ±axisY) best aligns with the collision normal
            FP dotX = FPVector2.Dot(axisX, normal);
            FP dotY = FPVector2.Dot(axisY, normal);

            // Build the edge in local space, then transform to world
            if (FPMath.Abs(dotX) > FPMath.Abs(dotY))
            {
                // Use ±X face
                FP x = dotX > FP.Zero ? hw : -hw;
                FPVector2 l1 = new FPVector2(x, -hh);
                FPVector2 l2 = new FPVector2(x, hh);
                p1 = body.Position + new FPVector2(l1.X * cos - l1.Y * sin, l1.X * sin + l1.Y * cos);
                p2 = body.Position + new FPVector2(l2.X * cos - l2.Y * sin, l2.X * sin + l2.Y * cos);
            }
            else
            {
                // Use ±Y face
                FP y = dotY > FP.Zero ? hh : -hh;
                FPVector2 l1 = new FPVector2(-hw, y);
                FPVector2 l2 = new FPVector2(hw, y);
                p1 = body.Position + new FPVector2(l1.X * cos - l1.Y * sin, l1.X * sin + l1.Y * cos);
                p2 = body.Position + new FPVector2(l2.X * cos - l2.Y * sin, l2.X * sin + l2.Y * cos);
            }
        }

        private static bool BoxVsCircle(ref RigidBodyLS box, ref RigidBodyLS circle, ref ContactManifold manifold)
        {
            // Transform circle center to box's local space
            FPVector2 delta = circle.Position - box.Position;
            FP cos = FPMath.Cos(-box.Rotation);
            FP sin = FPMath.Sin(-box.Rotation);
            FPVector2 localCirclePos = new FPVector2(
                delta.X * cos - delta.Y * sin,
                delta.X * sin + delta.Y * cos
            );

            // Find closest point on box to circle center (in local space)
            FP hw = box.BoxShape.HalfWidth;
            FP hh = box.BoxShape.HalfHeight;
            FPVector2 closest = new FPVector2(
                FPMath.Clamp(localCirclePos.X, -hw, hw),
                FPMath.Clamp(localCirclePos.Y, -hh, hh)
            );

            // Check if closest point is inside circle
            FPVector2 localDelta = localCirclePos - closest;
            FP distSqr = localDelta.SqrMagnitude;
            FP radius = circle.CircleShape.Radius;

            if (distSqr >= radius * radius)
                return false;

            FP dist = FPMath.Sqrt(distSqr);

            // Transform back to world space
            FPVector2 worldClosest = box.Position + new FPVector2(
                closest.X * FPMath.Cos(box.Rotation) - closest.Y * FPMath.Sin(box.Rotation),
                closest.X * FPMath.Sin(box.Rotation) + closest.Y * FPMath.Cos(box.Rotation)
            );

            if (dist > FP.Epsilon)
            {
                manifold.Normal = (circle.Position - worldClosest) / dist;
            }
            else
            {
                // Circle center is inside box - use closest edge normal
                manifold.Normal = FPVector2.Up; // Simplified
            }

            manifold.Penetration = radius - dist;
            manifold.AddContact(worldClosest);

            return true;
        }

        private static bool CircleVsBox(ref RigidBodyLS circle, ref RigidBodyLS box, ref ContactManifold manifold)
        {
            bool result = BoxVsCircle(ref box, ref circle, ref manifold);
            if (result)
            {
                // Swap body IDs and flip normal
                int temp = manifold.BodyAId;
                manifold.BodyAId = manifold.BodyBId;
                manifold.BodyBId = temp;
                manifold.Normal = -manifold.Normal;
            }
            return result;
        }
    }
}
