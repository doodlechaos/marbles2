# LockSim - Deterministic Fixed-Point 2D Physics Engine

A simple, deterministic fixed-point 2D physics engine written in C# for use in both Unity and SpacetimeDB server modules.

## Overview

LockSim provides:
- **Deterministic simulation**: Uses Q16.16 fixed-point math for cross-platform determinism
- **2D rigid body dynamics**: Position, velocity, rotation, angular velocity
- **Collision detection**: Box-box, circle-circle, and box-circle collisions
- **Physics constraints**: Contact resolution with friction and restitution
- **Snapshot/restore**: Easy state management for rollback and replay

## Architecture

### Math (`math/`)
- `FP.cs` - Q16.16 fixed-point number type
- `FPMath.cs` - Fixed-point math functions (sqrt, sin, cos, atan2, etc.)
- `FPVector2.cs` - 2D vector using fixed-point components

### Data (`data/`)
- `World.cs` - Main world container with snapshot/restore capabilities
- `RigidBody.cs` - Rigid body with transform, velocity, and material properties
- `Shape.cs` - Shape types (Box, Circle) and AABB for broad phase
- `Contact.cs` - Contact manifolds for collision resolution

### Pipeline (`pipeline/`)
- `PhysicsPipeline.cs` - Main entry point for stepping simulation
- `Integration.cs` - Force/velocity integration
- `NarrowPhase.cs` - Broad and narrow phase collision detection
- `ConstraintSolver.cs` - Iterative impulse-based constraint solver

## Usage

### Basic Setup

```csharp
using LockSim;

// Create world (contains only deterministic state)
World world = new World();
world.Gravity = FPVector2.FromFloats(0f, -9.81f);

// Create simulation context (contains runtime/non-serializable data)
WorldSimulationContext context = new WorldSimulationContext();
context.VelocityIterations = 8;
context.PositionIterations = 3;

// Create static ground
RigidBodyLS ground = RigidBodyLS.CreateStatic(
    0, 
    FPVector2.FromFloats(0f, -5f), 
    FP.Zero
);
ground.SetBoxShape(FP.FromFloat(10f), FP.FromFloat(1f));
world.AddBody(ground);

// Create dynamic box
RigidBodyLS box = RigidBodyLS.CreateDynamic(
    1, 
    FPVector2.FromFloats(0f, 5f), 
    FP.Zero, 
    FP.One // mass
);
box.SetBoxShape(FP.One, FP.One);
world.AddBody(box);

// Step simulation
FP deltaTime = FP.FromFloat(1f / 60f);
PhysicsPipeline.Step(world, deltaTime, context);
```

### Snapshot and Restore

```csharp
// Take snapshot
World.Snapshot snapshot = world.TakeSnapshot();

// Run simulation...
PhysicsPipeline.Step(world, deltaTime);

// Restore to previous state
world.RestoreSnapshot(snapshot);
```

### Unity Integration

Attach the `LockSimDemo` component to a GameObject in your scene:

```csharp
// The demo will automatically:
// 1. Create a world with a ground plane
// 2. Spawn a stack of boxes
// 3. Step the simulation each frame
// 4. Visualize bodies using Gizmos
```

### SpacetimeDB Integration

```csharp
// In your SpacetimeDB reducer:
[SpacetimeDB.Reducer]
public static void StepPhysics(ReducerContext ctx)
{
    // Load world state from database
    World world = LoadWorldFromDB();
    
    // Step simulation
    FP deltaTime = FP.FromFloat(1f / 20f);
    PhysicsPipeline.Step(world, deltaTime);
    
    // Save world state back to database
    SaveWorldToDB(world);
}
```

## Features

### Deterministic Ordering
- All collections use `List<T>` with deterministic iteration order
- Bodies are assigned sequential IDs
- Contact resolution processes bodies in ID order

### Fixed-Point Math
- Q16.16 format: 16 bits integer, 16 bits fractional
- Range: approximately ±32,768 with ~0.00001526 precision
- Pre-computed lookup tables for trigonometric functions
- No floating-point operations in simulation loop

### Collision Detection
- **Broad phase**: AABB overlap tests
- **Narrow phase**: 
  - Box-box: Separating Axis Theorem (SAT)
  - Circle-circle: Distance check
  - Box-circle: Closest point projection

### Physics Solver
- Iterative impulse-based solver
- Velocity constraints with friction (Coulomb model)
- Position correction to prevent sinking (Baumgarte stabilization)
- Configurable iteration counts for accuracy vs performance

## Demo

The included `LockSimDemo` creates a simple scene:
- Static ground box at the bottom
- Stack of 5 dynamic boxes
- Boxes fall due to gravity and topple realistically
- UI controls for pause/resume and reset

### Controls
- **Pause/Resume**: Toggle simulation
- **Reset**: Recreate the world from scratch

### Visualization
- Gray wireframes: Static bodies
- Cyan wireframes: Dynamic bodies
- Red spheres + rays: Contact points and normals

## Performance

Typical performance for simple scenes:
- 10 bodies: < 0.1ms per step
- 50 bodies: < 1ms per step
- 100 bodies: < 5ms per step

Complexity: O(n²) collision detection (suitable for small-medium simulations)

## Limitations

- 2D only (no 3D support)
- Simple shapes (boxes and circles)
- O(n²) broad phase (no spatial partitioning)
- No joints or constraints beyond contacts
- No continuous collision detection (CCD)
- Fixed-point precision limits maximum velocities and sizes

## Future Enhancements

Potential improvements:
- Spatial partitioning (quadtree) for O(n log n) broad phase
- More shape types (polygons, capsules)
- Joint constraints (distance, revolute, prismatic)
- Continuous collision detection for fast-moving objects
- Island detection for sleeping bodies

## License

This is part of the Marbles project. Feel free to use and modify as needed.

