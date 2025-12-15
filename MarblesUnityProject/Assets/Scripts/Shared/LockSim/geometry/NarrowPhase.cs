namespace LockSim
{
    using System.Collections.Generic;
    using FPMathLib;

    /// <summary>
    /// Performs collision detection between colliders.
    /// Uses broad phase spatial hash to reduce candidate pairs, then narrow phase for precise detection.
    /// </summary>
    public static class NarrowPhase
    {
        public static void DetectCollisions(World world, WorldSimulationContext context)
        {
            List<ColliderLS> colliders = world.GetCollidersMutable();
            List<ContactManifold> contacts = context.GetContactsMutable();
            contacts.Clear();

            if (colliders.Count < 2)
                return;

            // Get scratch arrays from context to avoid allocations
            FPVector2[] scratchCornersA = context.ScratchCornersA;
            FPVector2[] scratchCornersB = context.ScratchCornersB;
            FPVector2[] scratchAxes = context.ScratchAxes;

            // Build broad phase spatial hash
            BroadPhase broadPhase = context.GetBroadPhase();
            broadPhase.Clear();

            // Cache world transforms and AABBs for all colliders
            int colliderCount = colliders.Count;

            // Use stackalloc-style approach: parallel arrays for transform cache
            // These are indexed by collider list index, not collider ID
            FPVector2[] positions = new FPVector2[colliderCount];
            FP[] rotations = new FP[colliderCount];
            bool[] isStatic = new bool[colliderCount];
            bool[] isValid = new bool[colliderCount];

            for (int i = 0; i < colliderCount; i++)
            {
                ColliderLS collider = colliders[i];

                if (!collider.IsEnabled)
                {
                    isValid[i] = false;
                    continue;
                }

                bool hasBody = world.TryGetBody(collider.ParentBodyId, out RigidBodyLS body);
                isStatic[i] = !hasBody || body.BodyType == BodyType.Static;

                if (hasBody)
                {
                    positions[i] = collider.GetWorldPosition(body);
                    rotations[i] = collider.GetWorldRotation(body);
                }
                else
                {
                    positions[i] = collider.LocalPosition;
                    rotations[i] = collider.LocalRotation;
                }

                isValid[i] = true;

                // Insert into broad phase
                AABB aabb = collider.ComputeAABB(positions[i], rotations[i]);
                broadPhase.Insert(i, aabb);
            }

            // Generate candidate pairs from broad phase
            List<BroadPhasePair> candidatePairs = broadPhase.GeneratePairs();

            // Narrow phase: test each candidate pair
            foreach (BroadPhasePair pair in candidatePairs)
            {
                int i = pair.ColliderIndexA;
                int j = pair.ColliderIndexB;

                // Skip invalid colliders
                if (!isValid[i] || !isValid[j])
                    continue;

                // Skip static-static pairs
                if (isStatic[i] && isStatic[j])
                    continue;

                ColliderLS colliderA = colliders[i];
                ColliderLS colliderB = colliders[j];

                FPVector2 posA = positions[i];
                FP rotA = rotations[i];
                FPVector2 posB = positions[j];
                FP rotB = rotations[j];

                // Narrow phase: shape-specific collision
                ContactManifold manifold = new ContactManifold(
                    colliderA.ParentBodyId,
                    colliderB.ParentBodyId,
                    colliderA.Id,
                    colliderB.Id
                );

                bool colliding = TestShapePair(
                    ref colliderA,
                    posA,
                    rotA,
                    ref colliderB,
                    posB,
                    rotB,
                    ref manifold,
                    scratchCornersA,
                    scratchCornersB,
                    scratchAxes
                );

                if (colliding)
                {
                    contacts.Add(manifold);
                }
            }
        }

        /// <summary>
        /// Tests collision between two shapes, dispatching to the appropriate handler.
        /// </summary>
        private static bool TestShapePair(
            ref ColliderLS colliderA,
            FPVector2 posA,
            FP rotA,
            ref ColliderLS colliderB,
            FPVector2 posB,
            FP rotB,
            ref ContactManifold manifold,
            FPVector2[] scratchCornersA,
            FPVector2[] scratchCornersB,
            FPVector2[] scratchAxes
        )
        {
            if (colliderA.ShapeType == ShapeType.Circle && colliderB.ShapeType == ShapeType.Circle)
            {
                return CircleVsCircle(ref colliderA, posA, ref colliderB, posB, ref manifold);
            }

            if (colliderA.ShapeType == ShapeType.Box && colliderB.ShapeType == ShapeType.Box)
            {
                return BoxVsBox(
                    ref colliderA,
                    posA,
                    rotA,
                    ref colliderB,
                    posB,
                    rotB,
                    ref manifold,
                    scratchCornersA,
                    scratchCornersB,
                    scratchAxes
                );
            }

            if (colliderA.ShapeType == ShapeType.Box && colliderB.ShapeType == ShapeType.Circle)
            {
                return BoxVsCircle(ref colliderA, posA, rotA, ref colliderB, posB, ref manifold);
            }

            if (colliderA.ShapeType == ShapeType.Circle && colliderB.ShapeType == ShapeType.Box)
            {
                return CircleVsBox(ref colliderA, posA, ref colliderB, posB, rotB, ref manifold);
            }

            return false;
        }

        private static bool CircleVsCircle(
            ref ColliderLS colliderA,
            FPVector2 posA,
            ref ColliderLS colliderB,
            FPVector2 posB,
            ref ContactManifold manifold
        )
        {
            FP radiusA = colliderA.CircleShape.Radius;
            FP radiusB = colliderB.CircleShape.Radius;

            FPVector2 delta = posB - posA;
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
            FPVector2 contactPoint = posA + manifold.Normal * radiusA;
            manifold.AddContact(contactPoint);

            return true;
        }

        private static bool BoxVsBox(
            ref ColliderLS colliderA,
            FPVector2 posA,
            FP rotA,
            ref ColliderLS colliderB,
            FPVector2 posB,
            FP rotB,
            ref ContactManifold manifold,
            FPVector2[] cornersA,
            FPVector2[] cornersB,
            FPVector2[] axes
        )
        {
            FP cosA = FPMath.Cos(rotA);
            FP sinA = FPMath.Sin(rotA);
            FP cosB = FPMath.Cos(rotB);
            FP sinB = FPMath.Sin(rotB);

            // Compute axes (box edge normals) - reuse scratch array
            axes[0] = new FPVector2(cosA, sinA);
            axes[1] = new FPVector2(-sinA, cosA);
            axes[2] = new FPVector2(cosB, sinB);
            axes[3] = new FPVector2(-sinB, cosB);

            // Compute corners once - reuse scratch arrays
            ComputeBoxCorners(ref colliderA, posA, cosA, sinA, cornersA);
            ComputeBoxCorners(ref colliderB, posB, cosB, sinB, cornersB);

            FP minPenetration = FP.MaxValue;
            FPVector2 minAxis = FPVector2.Zero;
            int minAxisIndex = -1;

            for (int i = 0; i < 4; i++)
            {
                FPVector2 axis = axes[i];
                FP penetration = ComputeOverlap(cornersA, cornersB, axis);

                if (penetration <= FP.Zero)
                    return false;

                if (penetration < minPenetration)
                {
                    minPenetration = penetration;
                    minAxis = axis;
                    minAxisIndex = i;
                }
            }

            // Ensure normal points from A to B
            FPVector2 delta = posB - posA;
            if (FPVector2.Dot(minAxis, delta) < FP.Zero)
            {
                minAxis = -minAxis;
            }

            manifold.Normal = minAxis;
            manifold.Penetration = minPenetration;

            // Generate contact point
            bool referenceIsA = minAxisIndex < 2;
            FPVector2 refPos = referenceIsA ? posA : posB;
            FP refCos = referenceIsA ? cosA : cosB;
            FP refSin = referenceIsA ? sinA : sinB;
            ref ColliderLS refCollider = ref (referenceIsA ? ref colliderA : ref colliderB);
            FPVector2[] incidentCorners = referenceIsA ? cornersB : cornersA;

            FPVector2 refToIncNormal = referenceIsA ? manifold.Normal : -manifold.Normal;

            GetReferenceEdge(
                ref refCollider,
                refPos,
                refCos,
                refSin,
                refToIncNormal,
                out FPVector2 refP1,
                out FPVector2 refP2
            );

            // Find incident point closest to reference face
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

            // Project incident point onto reference edge
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
                contactPoint = refP1;
            }

            manifold.AddContact(contactPoint);
            return true;
        }

        /// <summary>
        /// Computes box corners into a pre-allocated array.
        /// </summary>
        private static void ComputeBoxCorners(
            ref ColliderLS collider,
            FPVector2 pos,
            FP cos,
            FP sin,
            FPVector2[] corners
        )
        {
            FP hw = collider.BoxShape.HalfWidth;
            FP hh = collider.BoxShape.HalfHeight;

            // Local corners
            FP lx0 = -hw,
                ly0 = -hh;
            FP lx1 = hw,
                ly1 = -hh;
            FP lx2 = hw,
                ly2 = hh;
            FP lx3 = -hw,
                ly3 = hh;

            // Transform to world space
            corners[0] = pos + new FPVector2(lx0 * cos - ly0 * sin, lx0 * sin + ly0 * cos);
            corners[1] = pos + new FPVector2(lx1 * cos - ly1 * sin, lx1 * sin + ly1 * cos);
            corners[2] = pos + new FPVector2(lx2 * cos - ly2 * sin, lx2 * sin + ly2 * cos);
            corners[3] = pos + new FPVector2(lx3 * cos - ly3 * sin, lx3 * sin + ly3 * cos);
        }

        /// <summary>
        /// Computes overlap (penetration) between two sets of corners along an axis.
        /// Returns zero or negative if no overlap.
        /// </summary>
        private static FP ComputeOverlap(FPVector2[] cornersA, FPVector2[] cornersB, FPVector2 axis)
        {
            FP minA = FP.MaxValue,
                maxA = FP.MinValue;
            FP minB = FP.MaxValue,
                maxB = FP.MinValue;

            for (int i = 0; i < 4; i++)
            {
                FP projA = FPVector2.Dot(cornersA[i], axis);
                minA = FPMath.Min(minA, projA);
                maxA = FPMath.Max(maxA, projA);

                FP projB = FPVector2.Dot(cornersB[i], axis);
                minB = FPMath.Min(minB, projB);
                maxB = FPMath.Max(maxB, projB);
            }

            if (maxA < minB || maxB < minA)
                return FP.Zero;

            return FPMath.Min(maxA - minB, maxB - minA);
        }

        private static void GetReferenceEdge(
            ref ColliderLS collider,
            FPVector2 pos,
            FP cos,
            FP sin,
            FPVector2 normal,
            out FPVector2 p1,
            out FPVector2 p2
        )
        {
            FPVector2 axisX = new FPVector2(cos, sin);
            FPVector2 axisY = new FPVector2(-sin, cos);

            FP hw = collider.BoxShape.HalfWidth;
            FP hh = collider.BoxShape.HalfHeight;

            FP dotX = FPVector2.Dot(axisX, normal);
            FP dotY = FPVector2.Dot(axisY, normal);

            if (FPMath.Abs(dotX) > FPMath.Abs(dotY))
            {
                FP x = dotX > FP.Zero ? hw : -hw;
                FP lx1 = x,
                    ly1 = -hh;
                FP lx2 = x,
                    ly2 = hh;
                p1 = pos + new FPVector2(lx1 * cos - ly1 * sin, lx1 * sin + ly1 * cos);
                p2 = pos + new FPVector2(lx2 * cos - ly2 * sin, lx2 * sin + ly2 * cos);
            }
            else
            {
                FP y = dotY > FP.Zero ? hh : -hh;
                FP lx1 = -hw,
                    ly1 = y;
                FP lx2 = hw,
                    ly2 = y;
                p1 = pos + new FPVector2(lx1 * cos - ly1 * sin, lx1 * sin + ly1 * cos);
                p2 = pos + new FPVector2(lx2 * cos - ly2 * sin, lx2 * sin + ly2 * cos);
            }
        }

        private static bool BoxVsCircle(
            ref ColliderLS box,
            FPVector2 boxPos,
            FP boxRot,
            ref ColliderLS circle,
            FPVector2 circlePos,
            ref ContactManifold manifold
        )
        {
            // Transform circle center to box's local space
            FPVector2 delta = circlePos - boxPos;
            FP cos = FPMath.Cos(-boxRot);
            FP sin = FPMath.Sin(-boxRot);
            FPVector2 localCirclePos = new FPVector2(
                delta.X * cos - delta.Y * sin,
                delta.X * sin + delta.Y * cos
            );

            FP hw = box.BoxShape.HalfWidth;
            FP hh = box.BoxShape.HalfHeight;
            FPVector2 closest = new FPVector2(
                FPMath.Clamp(localCirclePos.X, -hw, hw),
                FPMath.Clamp(localCirclePos.Y, -hh, hh)
            );

            FPVector2 localDelta = localCirclePos - closest;
            FP distSqr = localDelta.SqrMagnitude;
            FP radius = circle.CircleShape.Radius;

            if (distSqr >= radius * radius)
                return false;

            FP dist = FPMath.Sqrt(distSqr);

            // Transform back to world space
            FP cosW = FPMath.Cos(boxRot);
            FP sinW = FPMath.Sin(boxRot);
            FPVector2 worldClosest =
                boxPos
                + new FPVector2(
                    closest.X * cosW - closest.Y * sinW,
                    closest.X * sinW + closest.Y * cosW
                );

            if (dist > FP.Epsilon)
            {
                manifold.Normal = (circlePos - worldClosest) / dist;
            }
            else
            {
                manifold.Normal = FPVector2.Up;
            }

            manifold.Penetration = radius - dist;
            manifold.AddContact(worldClosest);

            return true;
        }

        private static bool CircleVsBox(
            ref ColliderLS circle,
            FPVector2 circlePos,
            ref ColliderLS box,
            FPVector2 boxPos,
            FP boxRot,
            ref ContactManifold manifold
        )
        {
            bool result = BoxVsCircle(ref box, boxPos, boxRot, ref circle, circlePos, ref manifold);
            if (result)
            {
                // Swap IDs and flip normal
                (manifold.BodyAId, manifold.BodyBId) = (manifold.BodyBId, manifold.BodyAId);
                (manifold.ColliderAId, manifold.ColliderBId) = (
                    manifold.ColliderBId,
                    manifold.ColliderAId
                );
                manifold.Normal = -manifold.Normal;
            }
            return result;
        }
    }
}
