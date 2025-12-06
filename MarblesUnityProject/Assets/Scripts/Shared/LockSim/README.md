# LockSim - Deterministic Fixed-Point 2D Physics Engine

A simple, deterministic fixed-point 2D physics engine written in C# for use in both Unity and SpacetimeDB server modules.

## Overview

LockSim provides:
- **Deterministic simulation**: Uses Q16.16 fixed-point math for cross-platform determinism
- **2D rigid body dynamics**: Position, velocity, rotation, angular velocity
- **Separate colliders**: Colliders are independent from bodies, supporting multiple colliders per body
- **Collision detection**: Box-box, circle-circle, and box-circle collisions
- **Physics constraints**: Contact resolution with friction and restitution
- **Snapshot/restore**: Easy state management for rollback and replay

## Architecture

The architecture follows Rapier/Box2D patterns with separate rigid bodies and colliders.

### Data (`data/`)
- `World.cs` - Main world container managing bodies and colliders
- `RigidBodyLS.cs` - Rigid body with transform, velocity, and mass properties
- `ColliderLS.cs` - Collider with shape, local transform, and material properties
- `Shape.cs` - Shape types (Box, Circle) and AABB for broad phase
- `Contact.cs` - Contact manifolds for collision resolution
- `WorldSimulationContext.cs` - Non-serialized runtime state (contacts, iteration settings)

### Pipeline (`pipeline/`)
- `PhysicsPipeline.cs` - Main entry point for stepping simulation

### Dynamics (`dynamics/`)
- `Integration.cs` - Force/velocity integration
- `ConstraintSolver.cs` - Iterative impulse-based constraint solver

### Geometry (`geometry/`)
- `NarrowPhase.cs` - Broad and narrow phase collision detection

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

// Create static ground body
RigidBodyLS ground = RigidBodyLS.CreateStatic(0, FPVector2.FromFloats(0f, -5f), FP.Zero);
int groundId = world.AddBody(ground);

// Attach a box collider to the ground
ColliderLS groundCollider = ColliderLS.CreateBox(0, FP.FromFloat(10f), FP.FromFloat(1f), groundId);
world.AddCollider(groundCollider);

// Create dynamic box body
RigidBodyLS box = RigidBodyLS.CreateDynamic(0, FPVector2.FromFloats(0f, 5f), FP.Zero, FP.One);
int boxId = world.AddBody(box);

// Attach a box collider and update inertia
ColliderLS boxCollider = ColliderLS.CreateBox(0, FP.One, FP.One, boxId);
world.AddCollider(boxCollider);

// Update body inertia from collider shape
box = world.GetBody(boxId);
box.SetInertiaFromCollider(boxCollider);
world.SetBody(boxId, box);

// Step simulation
FP deltaTime = FP.FromFloat(1f / 60f);
PhysicsPipeline.Step(world, deltaTime, context);
```

### Multiple Colliders Per Body

```csharp
// Create a body
RigidBodyLS body = RigidBodyLS.CreateDynamic(0, FPVector2.Zero, FP.Zero, FP.FromFloat(2f));
int bodyId = world.AddBody(body);

// Attach multiple colliders with local offsets
ColliderLS collider1 = ColliderLS.CreateBox(0, FP.One, FP.One, bodyId);
collider1.LocalPosition = FPVector2.FromFloats(-1f, 0f);
world.AddCollider(collider1);

ColliderLS collider2 = ColliderLS.CreateBox(0, FP.One, FP.One, bodyId);
collider2.LocalPosition = FPVector2.FromFloats(1f, 0f);
world.AddCollider(collider2);
```

### Collider Material Properties

```csharp
// Material properties are on the collider, not the body
ColliderLS collider = ColliderLS.CreateCircle(0, FP.One, bodyId);
collider.Friction = FP.FromFloat(0.3f);
collider.Restitution = FP.FromFloat(0.8f); // Bouncy!
world.AddCollider(collider);
```

### Snapshot and Restore

```csharp
// Take snapshot (serializes both bodies and colliders)
byte[] snapshot = world.ToSnapshot();

// Run simulation...
PhysicsPipeline.Step(world, deltaTime);

// Restore to previous state
world = MemoryPackSerializer.Deserialize<World>(snapshot);
```

## Key Types

### RigidBodyLS
Represents the dynamics of a physical object:
- Position and rotation (transform)
- Linear and angular velocity
- Mass and inertia
- Forces and torques (cleared each frame)

### ColliderLS
Defines the shape and material for collision detection:
- Shape type (Box or Circle)
- Local position and rotation relative to parent body
- Friction and restitution (material properties)
- Parent body ID (or -1 for orphan/sensor colliders)

### World
Container for all physics state:
- Bodies collection (dynamics)
- Colliders collection (shapes)
- Gravity setting
- Serializable with MemoryPack

## Features

### Deterministic Ordering
- All collections use `List<T>` with deterministic iteration order
- Bodies and colliders are assigned sequential IDs
- Contact resolution processes in deterministic order

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

## Performance

Typical performance for simple scenes:
- 10 colliders: < 0.1ms per step
- 50 colliders: < 1ms per step
- 100 colliders: < 5ms per step

Complexity: O(n²) collision detection (suitable for small-medium simulations)

## Limitations

- 2D only (no 3D support)
- Simple shapes (boxes and circles)
- O(n²) broad phase (no spatial partitioning)
- No joints or constraints beyond contacts
- No continuous collision detection (CCD)
- Fixed-point precision limits maximum velocities and sizes

## License

This is part of the Marbles project. Feel free to use and modify as needed.
