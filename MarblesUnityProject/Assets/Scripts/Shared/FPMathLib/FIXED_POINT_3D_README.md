# Fixed-Point 3D Math Library

This library provides deterministic fixed-point math for 3D transformations, suitable for networked games and physics simulations where determinism is critical.

## Components

### 1. FPVector3
A 3D vector using fixed-point numbers for deterministic calculations.

**Features:**
- All standard vector operations (add, subtract, multiply, divide)
- Dot product and cross product
- Distance and magnitude calculations
- Normalization
- Lerp, Min, Max, Clamp operations
- Project and ProjectOnPlane
- Angle and Reflect

**Constants:**
- `Zero`, `One`
- `Right`, `Left`, `Up`, `Down`, `Forward`, `Back`

**Example:**
```csharp
FPVector3 a = FPVector3.FromFloats(1.0f, 0, 0);
FPVector3 b = FPVector3.FromFloats(0, 1.0f, 0);
FPVector3 sum = a + b;
FP distance = FPVector3.Distance(a, b);
FPVector3 cross = FPVector3.Cross(a, b);
```

### 2. FPQuaternion
A quaternion for 3D rotations using fixed-point numbers.

**Features:**
- Quaternion multiplication
- Rotate vectors
- Euler angle conversion
- Angle-axis representation
- Look rotation
- Slerp (spherical linear interpolation)
- Lerp (linear interpolation)

**Example:**
```csharp
// Create from Euler angles
FPQuaternion rot = FPQuaternion.Euler(FP.PiOver2, FP.Zero, FP.Zero);

// Create from angle-axis
FPQuaternion rot2 = FPQuaternion.AngleAxis(FP.PiOver2, FPVector3.Up);

// Rotate a vector
FPVector3 rotated = rot * FPVector3.Forward;

// Interpolate
FPQuaternion interpolated = FPQuaternion.Slerp(rot, rot2, FP.Half);
```

### 3. FPTransform3D
A complete 3D transform system mimicking Unity's Transform component.

**Features:**
- Position, Rotation (quaternion), and Scale
- Local and world space transformations
- Parent-child hierarchy support
- Direction vectors (Forward, Up, Right, etc.)
- Translate, Rotate, RotateAround, LookAt methods
- TransformPoint, InverseTransformPoint
- TransformDirection, InverseTransformDirection
- TransformVector, InverseTransformVector
- Matrix generation

**Properties:**
- `LocalPosition`, `LocalRotation`, `LocalScale`, `LocalEulerAngles`
- `Position`, `Rotation`, `LossyScale`, `EulerAngles`
- `Forward`, `Back`, `Up`, `Down`, `Right`, `Left`
- `Parent`

**Example:**
```csharp
// Create a transform
FPTransform3D transform = new FPTransform3D();
transform.Position = FPVector3.FromFloats(5.0f, 0, 0);
transform.EulerAngles = FPVector3.FromFloats(0, FP.PiOver2.ToFloat(), 0);

// Get direction vectors
FPVector3 forward = transform.Forward;
FPVector3 up = transform.Up;

// Translate
transform.Translate(FPVector3.Forward * FP.Two);

// Rotate
transform.Rotate(FPVector3.Up * FP.PiOver2);

// Look at target
transform.LookAt(targetPosition, FPVector3.Up);

// Transform points
FPVector3 worldPoint = transform.TransformPoint(localPoint);
FPVector3 localPoint = transform.InverseTransformPoint(worldPoint);
```

### 4. FPMath Extensions
Added `Acos` and `Asin` functions to the existing `FPMath` library to support quaternion operations.

## Hierarchy System

The `FPTransform3D` supports parent-child relationships similar to Unity's Transform:

```csharp
// Create parent
FPTransform3D parent = new FPTransform3D();
parent.Position = FPVector3.FromFloats(5.0f, 0, 0);

// Create child
FPTransform3D child = new FPTransform3D();
child.LocalPosition = FPVector3.FromFloats(0, 2.0f, 0);
child.Parent = parent;

// Child's world position is now (5, 2, 0)
FPVector3 childWorldPos = child.Position;

// When parent moves, child moves with it
parent.Translate(FPVector3.Right * FP.Two);
// Child world position is now (7, 2, 0)
```

## Key Features

### Determinism
All calculations use fixed-point arithmetic (Q16.16 format) ensuring identical results across different platforms, architectures, and compiler optimizations. This is critical for:
- Networked multiplayer games
- Replay systems
- Lockstep simulations
- Physics prediction

### Performance
- Aggressive inlining for hot-path methods
- Efficient fixed-point operations
- Cached world transforms to minimize recalculation
- Optimized quaternion-vector multiplication

### Unity Integration
- Serializable structs and classes work with Unity's serialization
- Easy conversion to/from Unity's types (`FromFloats` methods)
- Similar API to Unity's Transform component
- `.meta` files included for Unity Asset Database

## Usage Notes

### Angles
All angles in the library are in **radians**, not degrees. Use the predefined constants:
- `FP.Pi` ≈ 3.14159 (180°)
- `FP.PiOver2` ≈ 1.5708 (90°)
- Or convert: `FP.FromFloat(degrees * Mathf.Deg2Rad)`

### Precision
The Q16.16 fixed-point format provides:
- Range: approximately ±32,767
- Precision: approximately 0.0000152 (1/65536)
- Suitable for most game scenarios

### World Transform Caching
`FPTransform3D` caches world space values and marks them dirty when local values or parent transforms change. This provides good performance while maintaining accuracy.

## Integration with Existing Code

This library extends your existing LockSim fixed-point math system:
- Uses existing `FP`, `FPVector2`, and `FPMath` types
- Follows the same coding conventions
- Uses the same namespace (`LockSim`)
- Compatible with your physics pipeline

## Testing

See `FPTransform3D_Example.cs` for comprehensive usage examples including:
- Basic transform operations
- Hierarchy management
- Rotation techniques
- Space conversions
- Vector operations
- Quaternion operations

## Performance Considerations

1. **Avoid frequent parent changes** - Changing parents recalculates transforms
2. **Cache direction vectors** if using them multiple times per frame
3. **Use SqrMagnitude** instead of Magnitude when possible (avoids sqrt)
4. **Use SqrDistance** instead of Distance when just comparing distances
5. **Prefer InverseTransformPoint** over repeated calculations

## Future Enhancements

Potential additions:
- Matrix4x4 struct for more complex transformations
- Frustum culling utilities
- Bounding box transformations
- Animation interpolation helpers
- More optimized matrix operations

