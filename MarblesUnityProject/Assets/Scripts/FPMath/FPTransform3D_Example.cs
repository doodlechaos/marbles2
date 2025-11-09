using UnityEngine;

namespace LockSim
{
    /// <summary>
    /// Example usage of FPVector3, FPQuaternion, and FPTransform3D
    /// This demonstrates the deterministic fixed-point 3D transform system
    /// </summary>
    public class FPTransform3D_Example : MonoBehaviour
    {
        void Start()
        {
            ExampleBasicUsage();
            ExampleHierarchy();
            ExampleRotations();
            ExampleSpaceConversions();
        }

        void ExampleBasicUsage()
        {
            // Create a transform at origin
            FPTransform3D transform = new FPTransform3D();
            
            // Set position
            transform.Position = FPVector3.FromFloats(1.5f, 2.0f, 3.0f);
            
            // Set rotation using Euler angles (in radians)
            transform.EulerAngles = FPVector3.FromFloats(0, FP.PiOver2.ToFloat(), 0); // 90 degrees around Y
            
            // Set scale
            transform.LocalScale = FPVector3.FromFloats(2.0f, 2.0f, 2.0f);
            
            // Access direction vectors
            FPVector3 forward = transform.Forward;
            FPVector3 up = transform.Up;
            FPVector3 right = transform.Right;
            
            Debug.Log($"Transform: {transform}");
            Debug.Log($"Forward: {forward}, Up: {up}, Right: {right}");
        }

        void ExampleHierarchy()
        {
            // Create parent transform
            FPTransform3D parent = new FPTransform3D();
            parent.Position = FPVector3.FromFloats(5.0f, 0, 0);
            
            // Create child transform
            FPTransform3D child = new FPTransform3D();
            child.LocalPosition = FPVector3.FromFloats(0, 2.0f, 0); // 2 units up from parent
            child.Parent = parent;
            
            // Child's world position will be (5, 2, 0)
            Debug.Log($"Child world position: {child.Position}");
            
            // Rotate parent - child rotates with it
            parent.Rotate(FPVector3.FromFloats(0, FP.PiOver2.ToFloat(), 0));
            Debug.Log($"Child world position after parent rotation: {child.Position}");
        }

        void ExampleRotations()
        {
            FPTransform3D transform = new FPTransform3D();
            
            // Rotate using Euler angles
            transform.Rotate(FPVector3.FromFloats(FP.PiOver2.ToFloat(), 0, 0)); // 90° around X-axis
            
            // Rotate using quaternion
            FPQuaternion rotation = FPQuaternion.AngleAxis(FP.PiOver2, FPVector3.Up);
            transform.Rotate(rotation);
            
            // Rotate around a point
            FPVector3 pivot = FPVector3.FromFloats(5.0f, 0, 0);
            transform.RotateAround(pivot, FPVector3.Up, FP.PiOver2);
            
            // Look at a target
            FPVector3 target = FPVector3.FromFloats(10.0f, 0, 10.0f);
            transform.LookAt(target, FPVector3.Up);
            
            Debug.Log($"Transform after rotations: {transform}");
        }

        void ExampleSpaceConversions()
        {
            FPTransform3D transform = new FPTransform3D();
            transform.Position = FPVector3.FromFloats(5.0f, 0, 0);
            transform.Rotate(FPVector3.FromFloats(0, FP.PiOver2.ToFloat(), 0));
            
            // Transform a point from local space to world space
            FPVector3 localPoint = FPVector3.FromFloats(1.0f, 0, 0);
            FPVector3 worldPoint = transform.TransformPoint(localPoint);
            Debug.Log($"Local point {localPoint} in world space: {worldPoint}");
            
            // Transform a point from world space to local space
            FPVector3 backToLocal = transform.InverseTransformPoint(worldPoint);
            Debug.Log($"World point {worldPoint} in local space: {backToLocal}");
            
            // Transform a direction (not affected by position or scale)
            FPVector3 localDirection = FPVector3.Forward;
            FPVector3 worldDirection = transform.TransformDirection(localDirection);
            Debug.Log($"Local direction {localDirection} in world space: {worldDirection}");
            
            // Get transformation matrix
            FP[] matrix = transform.GetMatrix();
            Debug.Log($"Transform matrix (first 4 values): {matrix[0]}, {matrix[1]}, {matrix[2]}, {matrix[3]}");
        }

        void ExampleVectorOperations()
        {
            // Create vectors
            FPVector3 a = FPVector3.FromFloats(1.0f, 0, 0);
            FPVector3 b = FPVector3.FromFloats(0, 1.0f, 0);
            
            // Vector operations
            FPVector3 sum = a + b;
            FPVector3 scaled = a * FP.Two;
            FP dot = FPVector3.Dot(a, b);
            FPVector3 cross = FPVector3.Cross(a, b);
            
            Debug.Log($"Sum: {sum}, Scaled: {scaled}, Dot: {dot}, Cross: {cross}");
            
            // Distance and magnitude
            FP distance = FPVector3.Distance(a, b);
            FP magnitude = a.Magnitude;
            FPVector3 normalized = a.Normalized;
            
            Debug.Log($"Distance: {distance}, Magnitude: {magnitude}, Normalized: {normalized}");
        }

        void ExampleQuaternionOperations()
        {
            // Create quaternions
            FPQuaternion rot1 = FPQuaternion.Euler(FP.PiOver2, FP.Zero, FP.Zero);
            FPQuaternion rot2 = FPQuaternion.AngleAxis(FP.PiOver2, FPVector3.Up);
            
            // Combine rotations
            FPQuaternion combined = rot1 * rot2;
            
            // Rotate a vector
            FPVector3 vector = FPVector3.Forward;
            FPVector3 rotated = combined * vector;
            
            Debug.Log($"Original: {vector}, Rotated: {rotated}");
            
            // Interpolate between rotations
            FPQuaternion interpolated = FPQuaternion.Slerp(rot1, rot2, FP.Half);
            
            // Look rotation
            FPQuaternion lookRot = FPQuaternion.LookRotation(FPVector3.Forward, FPVector3.Up);
            
            Debug.Log($"Interpolated: {interpolated}, Look rotation: {lookRot}");
        }
    }
}

