# LockSim Integration Guide

This guide explains how to use the LockSim deterministic physics engine across both Unity and SpacetimeDB.

## Project Structure

```
marbles2/
├── MarblesUnityProject/
│   └── Assets/
│       └── Scripts/
│           └── LockSim/               # Physics engine source
│               ├── math/              # Fixed-point math
│               ├── data/              # Data structures
│               ├── pipeline/          # Physics pipeline
│               ├── LockSimDemo.cs     # Unity demo
│               └── README.md          # Documentation
│
└── spacetimedb/
    └── src/
        ├── Lifecycle.cs
        └── PhysicsExample.cs          # SpacetimeDB integration example
```

## Using LockSim in Unity

### Step 1: Demo Scene

1. Open Unity and load the `MarblesUnityProject`
2. Open or create a scene
3. Create an empty GameObject
4. Add the `LockSimDemo` component to it
5. Press Play to see the physics simulation

The demo creates:
- A static ground box
- A stack of 5 dynamic boxes that fall and topple

### Step 2: Customization

In the Inspector, you can adjust:
- **Run Simulation**: Toggle on/off
- **Time Scale**: Speed up/slow down simulation
- **Steps Per Frame**: Physics substeps
- **Stack Height**: Number of boxes
- **Box Size**: Size of each cube
- **Ground Width/Height**: Ground dimensions
- **Colors**: Visualization colors

### Step 3: Code Integration

```csharp
using LockSim;
using UnityEngine;

public class MyPhysicsController : MonoBehaviour
{
    private World world;
    
    void Start()
    {
        // Initialize world
        world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -9.81f);
        
        // Add bodies...
    }
    
    void FixedUpdate()
    {
        // Step simulation
        FP deltaTime = FP.FromFloat(Time.fixedDeltaTime);
        PhysicsEngine.Step(world, deltaTime);
    }
}
```

## Using LockSim in SpacetimeDB

### Step 1: Copy LockSim Files

You need to copy the LockSim source files to your SpacetimeDB project.

**Option A: Create symbolic links** (recommended for development):

```powershell
# From project root
cd spacetimedb/src
cmd /c mklink /D LockSim ..\..\MarblesUnityProject\Assets\Scripts\LockSim
```

**Option B: Copy files manually**:

```powershell
# From project root
xcopy /E /I MarblesUnityProject\Assets\Scripts\LockSim\math spacetimedb\src\LockSim\math
xcopy /E /I MarblesUnityProject\Assets\Scripts\LockSim\data spacetimedb\src\LockSim\data
xcopy /E /I MarblesUnityProject\Assets\Scripts\LockSim\pipeline spacetimedb\src\LockSim\pipeline
```

**Note**: Only copy `.cs` files, not `.meta` files or `.md` files.

### Step 2: Update .csproj

Ensure your `spacetimedb/StdbModule.csproj` includes the LockSim files:

```xml
<ItemGroup>
  <Compile Include="src\**\*.cs" />
  <Compile Include="src\LockSim\**\*.cs" />
</ItemGroup>
```

### Step 3: Use Physics Reducers

The `PhysicsExample.cs` provides example reducers:

```bash
# Initialize physics world
spacetime call <module-name> InitPhysicsWorld

# Add a dynamic box
spacetime call <module-name> AddDynamicBox 0.0 5.0 1.0 1.0

# Step physics simulation
spacetime call <module-name> StepPhysics

# Sync bodies to queryable table
spacetime call <module-name> SyncBodiesToTable
```

### Step 4: Query Physics State

```sql
-- Query all bodies
SELECT * FROM PhysicsBody;

-- Get dynamic bodies only
SELECT * FROM PhysicsBody WHERE BodyType = 1;
```

### Step 5: Client Subscription

In your Unity client, subscribe to physics updates:

```csharp
using SpacetimeDB;

void OnConnect()
{
    // Subscribe to body updates
    SpacetimeDBClient.instance.Subscribe(new List<string> 
    { 
        "SELECT * FROM PhysicsBody" 
    });
}

void OnPhysicsBodyUpdate(PhysicsBody body)
{
    // Convert from fixed-point to Unity
    Vector3 position = new Vector3(
        FP.FromRaw(body.PositionX).ToFloat(),
        FP.FromRaw(body.PositionY).ToFloat(),
        0f
    );
    
    // Update Unity GameObject...
}
```

## Determinism Guarantees

LockSim ensures determinism through:

### 1. Fixed-Point Math
- All calculations use Q16.16 fixed-point arithmetic
- No floating-point operations in the simulation loop
- Consistent across platforms (x86, x64, ARM)

### 2. Deterministic Ordering
- Bodies stored in `List<T>` with sequential IDs
- Iteration order is always the same
- Contact resolution processes bodies by ID order

### 3. No Random Numbers
- Physics is entirely deterministic
- Same initial state + same inputs = same results

### 4. Snapshot/Restore
- Complete world state can be captured
- Perfect for rollback networking
- State includes all bodies and their properties

## Example: Shared Physics Simulation

### Server (SpacetimeDB)

```csharp
[Reducer]
public static void GameTick(ReducerContext ctx)
{
    // Load world
    World world = LoadWorldFromDatabase();
    
    // Step physics (20 Hz)
    PhysicsEngine.Step(world, FP.FromFloat(1f / 20f));
    
    // Save world
    SaveWorldToDatabase(world, tick++);
}
```

### Client (Unity)

```csharp
void OnPhysicsUpdate(List<PhysicsBody> bodies)
{
    foreach (var body in bodies)
    {
        // Find corresponding GameObject
        GameObject obj = bodyObjects[body.BodyId];
        
        // Update transform from fixed-point values
        obj.transform.position = new Vector3(
            FP.FromRaw(body.PositionX).ToFloat(),
            FP.FromRaw(body.PositionY).ToFloat(),
            0f
        );
        
        obj.transform.rotation = Quaternion.Euler(
            0f, 0f, 
            FP.FromRaw(body.Rotation).ToFloat() * Mathf.Rad2Deg
        );
    }
}
```

## Performance Tips

### Unity
- Use `FixedUpdate()` with a fixed timestep (e.g., 0.02s = 50 Hz)
- Limit the number of physics steps per frame
- Use simple shapes (boxes, circles) for better performance

### SpacetimeDB
- Run physics at server tickrate (typically 20 Hz)
- Use binary serialization for efficient storage
- Only sync changed bodies to clients (delta compression)
- Consider spatial partitioning for large worlds (>100 bodies)

## Testing Determinism

To verify determinism across platforms:

```csharp
// Take snapshot at start
var snapshot1 = world.TakeSnapshot();

// Run simulation for N steps
for (int i = 0; i < 1000; i++)
{
    PhysicsEngine.Step(world, FP.FromFloat(1f / 60f));
}

// Record end state
var endState1 = world.TakeSnapshot();

// Restore and re-run
world.RestoreSnapshot(snapshot1);
for (int i = 0; i < 1000; i++)
{
    PhysicsEngine.Step(world, FP.FromFloat(1f / 60f));
}

// Compare end states - should be identical
var endState2 = world.TakeSnapshot();
Assert.AreEqual(SerializeSnapshot(endState1), SerializeSnapshot(endState2));
```

## Common Issues

### Issue: Bodies fall through floor
- **Solution**: Ensure static bodies are created before dynamic bodies
- **Solution**: Increase velocity iterations in World settings
- **Solution**: Use smaller timesteps (1/60 instead of 1/30)

### Issue: Simulation is too slow
- **Solution**: Reduce number of bodies
- **Solution**: Decrease velocity/position iterations
- **Solution**: Use larger timesteps (but maintain determinism)

### Issue: Bodies jitter or vibrate
- **Solution**: Increase position correction (Baumgarte stabilization)
- **Solution**: Add more damping to bodies
- **Solution**: Ensure bodies start slightly separated (not overlapping)

### Issue: Different results on server vs client
- **Solution**: Verify both are using the same LockSim version
- **Solution**: Check that fixed-point values are transmitted correctly (RawValue)
- **Solution**: Ensure same timestep on both sides

## Next Steps

1. **Run the Unity demo**: See LockSim in action
2. **Try SpacetimeDB integration**: Follow the PhysicsExample.cs
3. **Build your game**: Use LockSim for deterministic multiplayer physics
4. **Optimize**: Add spatial partitioning or other optimizations as needed

## Support

For issues or questions about LockSim:
1. Check the [README.md](MarblesUnityProject/Assets/Scripts/LockSim/README.md)
2. Review the demo code: `LockSimDemo.cs` and `PhysicsExample.cs`
3. Test with simple scenarios first (2-3 bodies) before scaling up

---

**Happy building!** LockSim gives you deterministic physics for confident multiplayer game development.

