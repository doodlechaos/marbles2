using UnityEngine;
using LockSim;

/// <summary>
/// Simple test script to verify LockSim physics engine is working correctly
/// Attach to a GameObject and check Console for test results
/// </summary>
public class LockSimTest : MonoBehaviour
{
    [SerializeField] private bool runTestsOnStart = true;

    void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }

    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("=== LockSim Test Suite ===");
        
        TestFixedPointMath();
        TestVector2Math();
        TestRigidBodyCreation();
        TestWorldSnapshotRestore();
        TestSimpleFall();
        TestDeterminism();
        TestCollisionDetection();
        
        Debug.Log("=== All Tests Complete ===");
    }

    void TestFixedPointMath()
    {
        Debug.Log("Test: Fixed-Point Math");
        
        // Basic arithmetic
        FP a = FP.FromInt(5);
        FP b = FP.FromInt(3);
        Assert(a + b == FP.FromInt(8), "Addition");
        Assert(a - b == FP.FromInt(2), "Subtraction");
        Assert(a * b == FP.FromInt(15), "Multiplication");
        Assert(a / b == FP.FromFloat(5f / 3f), "Division", 0.01f);
        
        // Math functions
        FP sqrt4 = FPMath.Sqrt(FP.FromInt(4));
        Assert(sqrt4 == FP.FromInt(2), "Sqrt");
        
        FP max = FPMath.Max(a, b);
        Assert(max == a, "Max");
        
        FP min = FPMath.Min(a, b);
        Assert(min == b, "Min");
        
        Debug.Log("  ✓ Fixed-Point Math OK");
    }

    void TestVector2Math()
    {
        Debug.Log("Test: Vector2 Math");
        
        FPVector2 v1 = FPVector2.FromFloats(3f, 4f);
        FPVector2 v2 = FPVector2.FromFloats(1f, 2f);
        
        // Basic operations
        FPVector2 sum = v1 + v2;
        Assert(sum.X == FP.FromInt(4) && sum.Y == FP.FromInt(6), "Vector addition");
        
        FPVector2 diff = v1 - v2;
        Assert(diff.X == FP.FromInt(2) && diff.Y == FP.FromInt(2), "Vector subtraction");
        
        // Magnitude (3,4 triangle = 5)
        FP mag = v1.Magnitude;
        Assert(mag == FP.FromInt(5), "Magnitude", 0.01f);
        
        // Dot product
        FP dot = FPVector2.Dot(v1, v2);
        Assert(dot == FP.FromInt(11), "Dot product"); // 3*1 + 4*2 = 11
        
        Debug.Log("  ✓ Vector2 Math OK");
    }

    void TestRigidBodyCreation()
    {
        Debug.Log("Test: RigidBody Creation");
        
        // Static body
        RigidBody staticBody = RigidBody.CreateStatic(0, FPVector2.Zero, FP.Zero);
        Assert(staticBody.BodyType == BodyType.Static, "Static body type");
        Assert(staticBody.InverseMass == FP.Zero, "Static body has no inverse mass");
        
        // Dynamic body
        RigidBody dynamicBody = RigidBody.CreateDynamic(1, FPVector2.One, FP.Zero, FP.FromInt(2));
        Assert(dynamicBody.BodyType == BodyType.Dynamic, "Dynamic body type");
        Assert(dynamicBody.Mass == FP.FromInt(2), "Dynamic body mass");
        Assert(dynamicBody.InverseMass == FP.Half, "Dynamic body inverse mass");
        
        // Box shape
        dynamicBody.SetBoxShape(FP.One, FP.One);
        Assert(dynamicBody.ShapeType == ShapeType.Box, "Box shape set");
        Assert(dynamicBody.Inertia > FP.Zero, "Box has inertia");
        
        Debug.Log("  ✓ RigidBody Creation OK");
    }

    void TestWorldSnapshotRestore()
    {
        Debug.Log("Test: World Snapshot/Restore");
        
        World world = new World();
        
        // Add a body
        RigidBody body = RigidBody.CreateDynamic(0, FPVector2.FromFloats(1f, 2f), FP.Zero, FP.One);
        body.SetBoxShape(FP.One, FP.One);
        body.LinearVelocity = FPVector2.FromFloats(5f, 0f);
        world.AddBody(body);
        
        // Take snapshot
        World.Snapshot snapshot = world.TakeSnapshot();
        
        // Modify world
        body = world.GetBody(0);
        body.Position = FPVector2.FromFloats(100f, 200f);
        world.SetBody(0, body);
        
        // Verify change
        body = world.GetBody(0);
        Assert(body.Position.X == FP.FromFloat(100f), "Body modified");
        
        // Restore
        world.RestoreSnapshot(snapshot);
        
        // Verify restoration
        body = world.GetBody(0);
        Assert(body.Position.X == FP.FromFloat(1f), "Position restored");
        Assert(body.Position.Y == FP.FromFloat(2f), "Position restored");
        Assert(body.LinearVelocity.X == FP.FromFloat(5f), "Velocity restored");
        
        Debug.Log("  ✓ Snapshot/Restore OK");
    }

    void TestSimpleFall()
    {
        Debug.Log("Test: Simple Fall Simulation");
        
        World world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -10f);
        
        // Create falling box
        RigidBody box = RigidBody.CreateDynamic(0, FPVector2.FromFloats(0f, 10f), FP.Zero, FP.One);
        box.SetBoxShape(FP.One, FP.One);
        world.AddBody(box);
        
        // Step simulation
        FP dt = FP.FromFloat(1f / 60f);
        for (int i = 0; i < 60; i++) // 1 second
        {
            PhysicsEngine.Step(world, dt);
        }
        
        // Box should have fallen
        box = world.GetBody(0);
        Assert(box.Position.Y < FP.FromFloat(10f), "Box fell down");
        Assert(box.LinearVelocity.Y < FP.Zero, "Box has downward velocity");
        
        Debug.Log($"  After 1s: Position Y = {box.Position.Y.ToFloat():F2}, Velocity Y = {box.LinearVelocity.Y.ToFloat():F2}");
        Debug.Log("  ✓ Simple Fall OK");
    }

    void TestDeterminism()
    {
        Debug.Log("Test: Determinism");
        
        // Create identical worlds
        World world1 = CreateTestWorld();
        World world2 = CreateTestWorld();
        
        // Step both worlds
        FP dt = FP.FromFloat(1f / 60f);
        for (int i = 0; i < 100; i++)
        {
            PhysicsEngine.Step(world1, dt);
            PhysicsEngine.Step(world2, dt);
        }
        
        // Compare results
        for (int i = 0; i < world1.Bodies.Count; i++)
        {
            RigidBody body1 = world1.Bodies[i];
            RigidBody body2 = world2.Bodies[i];
            
            Assert(body1.Position == body2.Position, $"Body {i} position deterministic");
            Assert(body1.Rotation == body2.Rotation, $"Body {i} rotation deterministic");
            Assert(body1.LinearVelocity == body2.LinearVelocity, $"Body {i} velocity deterministic");
        }
        
        Debug.Log("  ✓ Determinism OK (100 steps identical)");
    }

    void TestCollisionDetection()
    {
        Debug.Log("Test: Collision Detection");
        
        World world = new World();
        world.Gravity = FPVector2.Zero; // No gravity for this test
        
        // Create two clearly overlapping boxes
        // Box1: center at (0,0), size 2x2 -> bounds (-1,-1) to (1,1)
        RigidBody box1 = RigidBody.CreateStatic(0, FPVector2.Zero, FP.Zero);
        box1.SetBoxShape(FP.Two, FP.Two);
        world.AddBody(box1);
        
        // Box2: center at (0,0), size 1x1 -> bounds (-0.5,-0.5) to (0.5,0.5)
        // This is completely inside box1
        RigidBody box2 = RigidBody.CreateDynamic(1, FPVector2.Zero, FP.Zero, FP.One);
        box2.SetBoxShape(FP.One, FP.One);
        world.AddBody(box2);
        
        // Manually call collision detection (before physics step to check raw detection)
        CollisionDetection.DetectCollisions(world);
        
        // Should have at least one contact
        bool hasCollision = world.Contacts.Count > 0;
        
        if (!hasCollision)
        {
            Debug.LogWarning($"  No collision detected. Box1 AABB: {box1.ComputeAABB().Min} to {box1.ComputeAABB().Max}");
            Debug.LogWarning($"  Box2 AABB: {box2.ComputeAABB().Min} to {box2.ComputeAABB().Max}");
        }
        
        Assert(hasCollision, "Collision detected");
        
        Debug.Log($"  Contacts detected: {world.Contacts.Count}");
        Debug.Log("  ✓ Collision Detection OK");
    }

    World CreateTestWorld()
    {
        World world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -9.81f);
        
        // Ground
        RigidBody ground = RigidBody.CreateStatic(0, FPVector2.FromFloats(0f, -5f), FP.Zero);
        ground.SetBoxShape(FP.FromInt(10), FP.One);
        world.AddBody(ground);
        
        // Falling box
        RigidBody box = RigidBody.CreateDynamic(1, FPVector2.FromFloats(0f, 5f), FP.Zero, FP.One);
        box.SetBoxShape(FP.One, FP.One);
        world.AddBody(box);
        
        return world;
    }

    void Assert(bool condition, string message, float tolerance = 0.0001f)
    {
        if (!condition)
        {
            Debug.LogError($"  ✗ FAILED: {message}");
        }
    }
    
    bool Assert(FP a, FP b, string message, float tolerance = 0.0001f)
    {
        float diff = Mathf.Abs(a.ToFloat() - b.ToFloat());
        bool passed = diff < tolerance;
        if (!passed)
        {
            Debug.LogError($"  ✗ FAILED: {message} (Expected: {b.ToFloat():F4}, Got: {a.ToFloat():F4})");
        }
        return passed;
    }
}

