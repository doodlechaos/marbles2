namespace LockSim
{
    using System.Collections.Generic;
    using FPMathLib;

    /// <summary>
    /// Performs collision detection between colliders.
    /// </summary>
    public static class NarrowPhase
    {
        public static void DetectCollisions(World world, WorldSimulationContext context)
        {
            List<ColliderLS> colliders = world.GetCollidersMutable();
            List<ContactManifold> contacts = context.GetContactsMutable();
            contacts.Clear();

            // O(n²) broad + narrow phase over colliders
            for (int i = 0; i < colliders.Count; i++)
            {
                for (int j = i + 1; j < colliders.Count; j++)
                {
                    ColliderLS colliderA = colliders[i];
                    ColliderLS colliderB = colliders[j];

                    // Get parent bodies (may be invalid for orphan colliders)
                    bool hasBodyA = world.TryGetBody(colliderA.ParentBodyId, out RigidBodyLS bodyA);
                    bool hasBodyB = world.TryGetBody(colliderB.ParentBodyId, out RigidBodyLS bodyB);

                    // Skip if both are static (no parent or static body)
                    bool aIsStatic = !hasBodyA || bodyA.BodyType == BodyType.Static;
                    bool bIsStatic = !hasBodyB || bodyB.BodyType == BodyType.Static;
                    if (aIsStatic && bIsStatic)
                        continue;

                    // Compute world-space transforms
                    FPVector2 posA = hasBodyA
                        ? colliderA.GetWorldPosition(bodyA)
                        : colliderA.LocalPosition;
                    FP rotA = hasBodyA
                        ? colliderA.GetWorldRotation(bodyA)
                        : colliderA.LocalRotation;
                    FPVector2 posB = hasBodyB
                        ? colliderB.GetWorldPosition(bodyB)
                        : colliderB.LocalPosition;
                    FP rotB = hasBodyB
                        ? colliderB.GetWorldRotation(bodyB)
                        : colliderB.LocalRotation;

                    // Broad phase: AABB test
                    AABB aabbA = colliderA.ComputeAABB(posA, rotA);
                    AABB aabbB = colliderB.ComputeAABB(posB, rotB);

                    if (!aabbA.Overlaps(aabbB))
                        continue;

                    // Narrow phase: shape-specific collision
                    ContactManifold manifold = new ContactManifold(
                        colliderA.ParentBodyId,
                        colliderB.ParentBodyId,
                        colliderA.Id,
                        colliderB.Id
                    );

                    bool colliding = false;
                    if (
                        colliderA.ShapeType == ShapeType.Box
                        && colliderB.ShapeType == ShapeType.Box
                    )
                    {
                        colliding = BoxVsBox(
                            ref colliderA,
                            posA,
                            rotA,
                            ref colliderB,
                            posB,
                            rotB,
                            ref manifold
                        );
                    }
                    else if (
                        colliderA.ShapeType == ShapeType.Circle
                        && colliderB.ShapeType == ShapeType.Circle
                    )
                    {
                        colliding = CircleVsCircle(
                            ref colliderA,
                            posA,
                            ref colliderB,
                            posB,
                            ref manifold
                        );
                    }
                    else if (
                        colliderA.ShapeType == ShapeType.Box
                        && colliderB.ShapeType == ShapeType.Circle
                    )
                    {
                        colliding = BoxVsCircle(
                            ref colliderA,
                            posA,
                            rotA,
                            ref colliderB,
                            posB,
                            ref manifold
                        );
                    }
                    else if (
                        colliderA.ShapeType == ShapeType.Circle
                        && colliderB.ShapeType == ShapeType.Box
                    )
                    {
                        colliding = CircleVsBox(
                            ref colliderA,
                            posA,
                            ref colliderB,
                            posB,
                            rotB,
                            ref manifold
                        );
                    }

                    if (colliding)
                    {
                        contacts.Add(manifold);
                    }
                }
            }
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
            ref ContactManifold manifold
        )
        {
            FP cosA = FPMath.Cos(rotA);
            FP sinA = FPMath.Sin(rotA);
            FP cosB = FPMath.Cos(rotB);
            FP sinB = FPMath.Sin(rotB);

            // Axes to test (box edge normals)
            FPVector2[] axes = new FPVector2[4];
            axes[0] = new FPVector2(cosA, sinA);
            axes[1] = new FPVector2(-sinA, cosA);
            axes[2] = new FPVector2(cosB, sinB);
            axes[3] = new FPVector2(-sinB, cosB);

            FP minPenetration = FP.MaxValue;
            FPVector2 minAxis = FPVector2.Zero;
            int minAxisIndex = -1;

            for (int i = 0; i < 4; i++)
            {
                FPVector2 axis = axes[i];
                FP penetration = ProjectAndCheckOverlap(
                    ref colliderA,
                    posA,
                    rotA,
                    ref colliderB,
                    posB,
                    rotB,
                    axis
                );

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
            bool referenceIsA = (minAxisIndex >= 0 && minAxisIndex < 2);
            FPVector2 refPos = referenceIsA ? posA : posB;
            FP refRot = referenceIsA ? rotA : rotB;
            ref ColliderLS refCollider = ref (referenceIsA ? ref colliderA : ref colliderB);
            FPVector2 incPos = referenceIsA ? posB : posA;
            FP incRot = referenceIsA ? rotB : rotA;
            ref ColliderLS incCollider = ref (referenceIsA ? ref colliderB : ref colliderA);

            FPVector2 refToIncNormal = referenceIsA ? manifold.Normal : -manifold.Normal;

            GetReferenceEdge(
                ref refCollider,
                refPos,
                refRot,
                refToIncNormal,
                out FPVector2 refP1,
                out FPVector2 refP2
            );

            FPVector2[] incidentCorners = GetBoxCorners(ref incCollider, incPos, incRot);
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

        private static FP ProjectAndCheckOverlap(
            ref ColliderLS boxA,
            FPVector2 posA,
            FP rotA,
            ref ColliderLS boxB,
            FPVector2 posB,
            FP rotB,
            FPVector2 axis
        )
        {
            FPVector2[] cornersA = GetBoxCorners(ref boxA, posA, rotA);
            FPVector2[] cornersB = GetBoxCorners(ref boxB, posB, rotB);

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

        private static FPVector2[] GetBoxCorners(ref ColliderLS collider, FPVector2 pos, FP rot)
        {
            FP cos = FPMath.Cos(rot);
            FP sin = FPMath.Sin(rot);

            FP hw = collider.BoxShape.HalfWidth;
            FP hh = collider.BoxShape.HalfHeight;

            FPVector2[] corners = new FPVector2[4];
            FPVector2 c0 = new FPVector2(-hw, -hh);
            FPVector2 c1 = new FPVector2(hw, -hh);
            FPVector2 c2 = new FPVector2(hw, hh);
            FPVector2 c3 = new FPVector2(-hw, hh);

            corners[0] = pos + new FPVector2(c0.X * cos - c0.Y * sin, c0.X * sin + c0.Y * cos);
            corners[1] = pos + new FPVector2(c1.X * cos - c1.Y * sin, c1.X * sin + c1.Y * cos);
            corners[2] = pos + new FPVector2(c2.X * cos - c2.Y * sin, c2.X * sin + c2.Y * cos);
            corners[3] = pos + new FPVector2(c3.X * cos - c3.Y * sin, c3.X * sin + c3.Y * cos);

            return corners;
        }

        private static void GetReferenceEdge(
            ref ColliderLS collider,
            FPVector2 pos,
            FP rot,
            FPVector2 normal,
            out FPVector2 p1,
            out FPVector2 p2
        )
        {
            FP cos = FPMath.Cos(rot);
            FP sin = FPMath.Sin(rot);
            FPVector2 axisX = new FPVector2(cos, sin);
            FPVector2 axisY = new FPVector2(-sin, cos);

            FP hw = collider.BoxShape.HalfWidth;
            FP hh = collider.BoxShape.HalfHeight;

            FP dotX = FPVector2.Dot(axisX, normal);
            FP dotY = FPVector2.Dot(axisY, normal);

            if (FPMath.Abs(dotX) > FPMath.Abs(dotY))
            {
                FP x = dotX > FP.Zero ? hw : -hw;
                FPVector2 l1 = new FPVector2(x, -hh);
                FPVector2 l2 = new FPVector2(x, hh);
                p1 = pos + new FPVector2(l1.X * cos - l1.Y * sin, l1.X * sin + l1.Y * cos);
                p2 = pos + new FPVector2(l2.X * cos - l2.Y * sin, l2.X * sin + l2.Y * cos);
            }
            else
            {
                FP y = dotY > FP.Zero ? hh : -hh;
                FPVector2 l1 = new FPVector2(-hw, y);
                FPVector2 l2 = new FPVector2(hw, y);
                p1 = pos + new FPVector2(l1.X * cos - l1.Y * sin, l1.X * sin + l1.Y * cos);
                p2 = pos + new FPVector2(l2.X * cos - l2.Y * sin, l2.X * sin + l2.Y * cos);
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
