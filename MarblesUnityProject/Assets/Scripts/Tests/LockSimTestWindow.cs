#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using LockSim;
using FPMathLib;

public class LockSimTestWindow : EditorWindow
{
    private readonly List<TestCase> _tests = new();
    private readonly List<TestResult> _results = new();
    private Vector2 _scroll;
    private bool _ranOnce;
    private DateTime _lastRunUtc;

    [MenuItem("Window/LockSim/Test Runner")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<LockSimTestWindow>();
        wnd.titleContent = new GUIContent("LockSim Test Runner");
        wnd.minSize = new Vector2(420, 300);
        wnd.InitTests();
        wnd.Show();
    }

    private void OnEnable()
    { 
        InitTests();
    }

    private void InitTests()
    {
        if (_tests.Count > 0) return;

        _tests.Add(new TestCase("Fixed-Point Math", TestFixedPointMath));
        _tests.Add(new TestCase("Vector2 Math", TestVector2Math));
        _tests.Add(new TestCase("RigidBody Creation", TestRigidBodyCreation));
        _tests.Add(new TestCase("Snapshot Restore Determinism", TestSnapshotRestoreDeterminism));
        _tests.Add(new TestCase("Simple Fall", TestSimpleFall));
        _tests.Add(new TestCase("Determinism (100 steps)", TestDeterminism));
        _tests.Add(new TestCase("Collision Detection", TestCollisionDetection));
    }

    private void OnGUI()
    {
        GUILayout.Space(6);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("LockSim Test Runner", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Run All Tests", GUILayout.Height(26)))
            {
                RunAll();
            }
        }

        GUILayout.Space(4);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Status", EditorStyles.miniBoldLabel, GUILayout.Width(60));
            GUILayout.Label("Test Name", EditorStyles.miniBoldLabel);
            GUILayout.Label("Details", EditorStyles.miniBoldLabel, GUILayout.Width(220));
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        if (_ranOnce && _results.Count == 0)
        {
            DrawRow(false, "No tests found", "Add tests to the suite.");
        }
        else if (_results.Count == 0)
        {
            GUILayout.Label("Click 'Run All Tests' to execute the suite.", EditorStyles.wordWrappedMiniLabel);
        }
        else
        {
            foreach (var r in _results)
            {
                var ok = r.Passed;
                var detail = string.IsNullOrEmpty(r.FirstError)
                    ? $"OK • {r.DurationMs:F1} ms"
                    : r.FirstError;
                DrawRow(ok, r.Name, detail);
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (_ranOnce)
            {
                int passed = 0;
                foreach (var r in _results) if (r.Passed) passed++;
                GUILayout.Label($"Last run: {_lastRunUtc.ToLocalTime():G}", EditorStyles.miniLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{passed}/{_results.Count} passed", EditorStyles.miniBoldLabel);
            }
        }
        GUILayout.Space(6);
    }

    private void DrawRow(bool passed, string name, string details)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            var iconLabel = passed ? "✅" : "❌";
            var color = passed ? new Color(0.2f, 0.7f, 0.2f) : new Color(0.85f, 0.25f, 0.25f);
            var iconStyle = new GUIStyle(EditorStyles.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            var nameStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };

            var prev = GUI.color;
            GUI.color = color;
            GUILayout.Label(iconLabel, iconStyle, GUILayout.Width(24));
            GUI.color = prev;

            GUILayout.Label(name, nameStyle);
            GUILayout.FlexibleSpace();

            var detailStyle = passed ? EditorStyles.miniLabel : new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = new Color(0.85f, 0.25f, 0.25f) }
            };
            GUILayout.Label(details, detailStyle, GUILayout.Width(220));
        }
    }

    private void RunAll()
    {
        _results.Clear();
        _ranOnce = true;
        _lastRunUtc = DateTime.UtcNow;

        foreach (var t in _tests)
        {
            var ctx = new TestContext(t.Name);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                t.Run(ctx);
            }
            catch (Exception ex)
            {
                ctx.Fail($"Exception: {ex.GetType().Name}: {ex.Message}");
            }
            sw.Stop();
            ctx.DurationMs = sw.Elapsed.TotalMilliseconds;
            _results.Add(ctx.ToResult());
        }

        // Optionally also log a compact summary to the Console:
        int pass = 0; foreach (var r in _results) if (r.Passed) pass++;
        Debug.Log($"[LockSim] Test run complete: {pass}/{_results.Count} passed.");
        Repaint();
    }

    // ---------- Test Implementations ----------

    private static void TestFixedPointMath(TestContext t)
    {
        t.Section("Basic arithmetic");
        FP a = FP.FromInt(5);
        FP b = FP.FromInt(3);
        t.Expect(a + b == FP.FromInt(8), "Addition 5+3=8");
        t.Expect(a - b == FP.FromInt(2), "Subtraction 5-3=2");
        t.Expect(a * b == FP.FromInt(15), "Multiplication 5*3=15");
        t.ExpectNear(a / b, FP.FromFloat(5f / 3f), 0.01f, "Division 5/3");

        t.Section("Math functions");
        FP sqrt4 = FPMath.Sqrt(FP.FromInt(4));
        t.Expect(sqrt4 == FP.FromInt(2), "Sqrt(4)=2");
        t.Expect(FPMath.Max(a, b) == a, "Max(5,3)=5");
        t.Expect(FPMath.Min(a, b) == b, "Min(5,3)=3");
    }

    private static void TestVector2Math(TestContext t)
    {
        FPVector2 v1 = FPVector2.FromFloats(3f, 4f);
        FPVector2 v2 = FPVector2.FromFloats(1f, 2f);

        var sum = v1 + v2;
        t.Expect(sum.X == FP.FromInt(4) && sum.Y == FP.FromInt(6), "Vector addition (3,4)+(1,2)=(4,6)");

        var diff = v1 - v2;
        t.Expect(diff.X == FP.FromInt(2) && diff.Y == FP.FromInt(2), "Vector subtraction (3,4)-(1,2)=(2,2)");

        t.ExpectNear(v1.Magnitude, FP.FromInt(5), 0.01f, "|(3,4)|=5");

        var dot = FPVector2.Dot(v1, v2);
        t.Expect(dot == FP.FromInt(11), "Dot((3,4),(1,2))=11");
    }

    private static void TestRigidBodyCreation(TestContext t)
    {
        var s = RigidBodyLS.CreateStatic(0, FPVector2.Zero, FP.Zero);
        t.Expect(s.BodyType == BodyType.Static, "Static body type");
        t.Expect(s.InverseMass == FP.Zero, "Static inverse mass is 0");

        var d = RigidBodyLS.CreateDynamic(1, FPVector2.One, FP.Zero, FP.FromInt(2));
        t.Expect(d.BodyType == BodyType.Dynamic, "Dynamic body type");
        t.Expect(d.Mass == FP.FromInt(2), "Dynamic mass = 2");
        t.Expect(d.InverseMass == FP.Half, "Dynamic inverse mass = 0.5");

        d.SetBoxShape(FP.One, FP.One);
        t.Expect(d.ShapeType == ShapeType.Box, "Box shape set");
        t.Expect(d.Inertia > FP.Zero, "Box inertia > 0");
    }

    private static void TestSnapshotRestoreDeterminism(TestContext t)
    {
        // Create fresh world
        var world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -9.81f);
        var context = new WorldSimulationContext();

        // Spawn static walls to contain everything
        var ground = RigidBodyLS.CreateStatic(0, FPVector2.FromFloats(0f, -10f), FP.Zero);
        ground.SetBoxShape(FP.FromInt(20), FP.One);
        world.AddBody(ground);

        var leftWall = RigidBodyLS.CreateStatic(0, FPVector2.FromFloats(-10f, 0f), FP.Zero);
        leftWall.SetBoxShape(FP.One, FP.FromInt(20));
        world.AddBody(leftWall);

        var rightWall = RigidBodyLS.CreateStatic(0, FPVector2.FromFloats(10f, 0f), FP.Zero);
        rightWall.SetBoxShape(FP.One, FP.FromInt(20));
        world.AddBody(rightWall);

        var ceiling = RigidBodyLS.CreateStatic(0, FPVector2.FromFloats(0f, 10f), FP.Zero);
        ceiling.SetBoxShape(FP.FromInt(20), FP.One);
        world.AddBody(ceiling);

        // Spawn 20 random rigid bodies inside
        int seed = 12345;
        System.Random random = new System.Random(seed);
        
        for (int i = 0; i < 20; i++)
        {
            FP x = FP.FromFloat((float)(random.NextDouble() * 16 - 8));
            FP y = FP.FromFloat((float)(random.NextDouble() * 16 - 8));
            FP rotation = FP.FromFloat((float)(random.NextDouble() * 6.28));
            FP mass = FP.FromFloat((float)(random.NextDouble() * 2 + 0.5));
            
            var body = RigidBodyLS.CreateDynamic(0, FPVector2.FromFloats(x.ToFloat(), y.ToFloat()), rotation, mass);
            
            if (random.Next(2) == 0)
            {
                FP size = FP.FromFloat((float)(random.NextDouble() * 0.5 + 0.3));
                body.SetBoxShape(size, size);
            }
            else
            {
                FP radius = FP.FromFloat((float)(random.NextDouble() * 0.5 + 0.3));
                body.SetCircleShape(radius);
            }
            
            world.AddBody(body);
        }

        // Simulate 100 steps
        FP dt = FP.FromFloat(1f / 60f);
        for (int i = 0; i < 100; i++)
        {
            PhysicsPipeline.Step(world, dt, context);
        }

        // Take snapshot
        var snapshot = world.TakeSnapshot();

        // Simulate 100 more steps
        for (int i = 0; i < 100; i++)
        {
            PhysicsPipeline.Step(world, dt, context);
        }

        // Get hash of the entire world state
        string hash1 = world.GetWorldHash();

        // Restore from the snapshot
        world.RestoreSnapshot(snapshot);

        // Simulate 100 more steps
        for (int i = 0; i < 100; i++)
        {
            PhysicsPipeline.Step(world, dt, context);
        }

        // Check if this hash of the world state matches the previous
        string hash2 = world.GetWorldHash();
        
        t.Expect(hash1 == hash2, $"World state after snapshot restore should match (hash1: {hash1.Substring(0, 16)}..., hash2: {hash2.Substring(0, 16)}...)");
    }

    private static void TestSimpleFall(TestContext t)
    {
        var world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -10f);

        var box = RigidBodyLS.CreateDynamic(0, FPVector2.FromFloats(0f, 10f), FP.Zero, FP.One);
        box.SetBoxShape(FP.One, FP.One);
        world.AddBody(box);

        FP dt = FP.FromFloat(1f / 60f);
        for (int i = 0; i < 60; i++)
            PhysicsPipeline.Step(world, dt);

        box = world.GetBody(0);
        t.Expect(box.Position.Y < FP.FromFloat(10f), "Box fell below start height");
        t.Expect(box.LinearVelocity.Y < FP.Zero, "Box has downward velocity");
    }

    private static void TestDeterminism(TestContext t)
    {
        var world1 = CreateTestWorld();
        var world2 = CreateTestWorld();

        FP dt = FP.FromFloat(1f / 60f);
        for (int i = 0; i < 100; i++)
        {
            PhysicsPipeline.Step(world1, dt);
            PhysicsPipeline.Step(world2, dt);
        }

        for (int i = 0; i < world1.Bodies.Count; i++)
        {
            var b1 = world1.Bodies[i];
            var b2 = world2.Bodies[i];

            t.Expect(b1.Position == b2.Position, $"Body {i} position deterministic");
            t.Expect(b1.Rotation == b2.Rotation, $"Body {i} rotation deterministic");
            t.Expect(b1.LinearVelocity == b2.LinearVelocity, $"Body {i} velocity deterministic");
        }
    }

    private static void TestCollisionDetection(TestContext t)
    {
        var world = new World();
        world.Gravity = FPVector2.Zero;
        var context = new WorldSimulationContext();

        var box1 = RigidBodyLS.CreateStatic(0, FPVector2.Zero, FP.Zero);
        box1.SetBoxShape(FP.Two, FP.Two);
        world.AddBody(box1);

        var box2 = RigidBodyLS.CreateDynamic(1, FPVector2.Zero, FP.Zero, FP.One);
        box2.SetBoxShape(FP.One, FP.One);
        world.AddBody(box2);

        NarrowPhase.DetectCollisions(world, context);

        bool hasCollision = context.Contacts.Count > 0;
        if (!hasCollision)
        {
            var aabb1 = box1.ComputeAABB();
            var aabb2 = box2.ComputeAABB();
            t.Fail($"No collision detected. Box1 AABB: {aabb1.Min}..{aabb1.Max}, Box2 AABB: {aabb2.Min}..{aabb2.Max}");
        }
        else
        {
            t.Expect(true, $"Contacts detected: {context.Contacts.Count}");
        }
    }

    private static World CreateTestWorld()
    {
        var world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -9.81f);

        var ground = RigidBodyLS.CreateStatic(0, FPVector2.FromFloats(0f, -5f), FP.Zero);
        ground.SetBoxShape(FP.FromInt(10), FP.One);
        world.AddBody(ground);

        var box = RigidBodyLS.CreateDynamic(1, FPVector2.FromFloats(0f, 5f), FP.Zero, FP.One);
        box.SetBoxShape(FP.One, FP.One);
        world.AddBody(box);

        return world;
    }

    // ---------- Harness types ----------

    private sealed class TestCase
    {
        public string Name { get; }
        public Action<TestContext> Run { get; }
        public TestCase(string name, Action<TestContext> run) { Name = name; Run = run; }
    }

    private sealed class TestResult
    {
        public string Name;
        public bool Passed;
        public string FirstError;
        public double DurationMs;
    }

    private sealed class TestContext
    {
        private readonly List<string> _errors = new();
        private string _section = "";
        public string Name { get; }
        public double DurationMs;

        public TestContext(string name) { Name = name; }

        public void Section(string title) => _section = title;

        public void Expect(bool condition, string message)
        {
            if (!condition) Fail(message);
        }

        public void ExpectNear(FP a, FP b, float tolerance, string message)
        {
            float diff = Mathf.Abs(a.ToFloat() - b.ToFloat());
            if (diff > tolerance) Fail($"{message} (expected {b.ToFloat():F4}, got {a.ToFloat():F4}, tol {tolerance})");
        }

        public void Fail(string message)
        {
            var prefix = string.IsNullOrEmpty(_section) ? "" : $"[{_section}] ";
            _errors.Add(prefix + message);
        }

        public TestResult ToResult()
        {
            return new TestResult
            {
                Name = Name,
                Passed = _errors.Count == 0,
                FirstError = _errors.Count > 0 ? _errors[0] : "",
                DurationMs = DurationMs
            };
        }
    }
}
#endif
