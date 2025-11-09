using UnityEngine;
using LockSim;

/// <summary>
/// Unity demo for the LockSim deterministic physics engine
/// Shows a stack of boxes falling onto a static ground box
/// </summary>
public class LockSimDemo : MonoBehaviour
{
    [Header("Simulation Settings")]
    [SerializeField] private bool runSimulation = true;
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private int stepsPerFrame = 1;
    
    [Header("Demo Setup")]
    [SerializeField] private int stackHeight = 5;
    [SerializeField] private float boxSize = 1f;
    [SerializeField] private float groundWidth = 20f;
    [SerializeField] private float groundHeight = 1f;

    [Header("Visualization")]
    [SerializeField] private Color staticBodyColor = Color.gray;
    [SerializeField] private Color dynamicBodyColor = Color.cyan;
    [SerializeField] private Color contactColor = Color.red;
    [SerializeField] private bool showContacts = true;

    private World world;
    private FP fixedDeltaTime = FP.FromFloat(1f / 60f);
    private float accumulator = 0f;

    void Start()
    {
        InitializeWorld();
    }

    void InitializeWorld()
    {
        world = new World();
        world.Gravity = FPVector2.FromFloats(0f, -9.81f);
        world.VelocityIterations = 8;
        world.PositionIterations = 3;

        // Create ground (static box)
        RigidBody ground = RigidBody.CreateStatic(
            0,
            FPVector2.FromFloats(0f, -2f),
            FP.Zero
        );
        ground.SetBoxShape(FP.FromFloat(groundWidth), FP.FromFloat(groundHeight));
        ground.Friction = FP.FromFloat(0.5f);
        ground.Restitution = FP.FromFloat(0.2f);
        world.AddBody(ground);

        // Create stack of dynamic boxes
        FP cubeSize = FP.FromFloat(boxSize);
        FP spacing = FP.FromFloat(0.01f); // Small gap to ensure they're not initially overlapping

        for (int i = 0; i < stackHeight; i++)
        {
            // Stack boxes vertically, slightly offset to create toppling effect
            FP xOffset = FP.FromFloat((i % 2 == 0 ? 0.1f : -0.1f) * i * 0.1f);
            FP yPos = FP.FromFloat(-1f) + cubeSize * FP.Half + (cubeSize + spacing) * FP.FromInt(i);

            RigidBody box = RigidBody.CreateDynamic(
                i + 1,
                FPVector2.FromFloats(xOffset.ToFloat(), yPos.ToFloat()),
                FP.FromFloat(Random.Range(-0.1f, 0.1f)), // Small random rotation
                FP.One // Mass of 1
            );
            
            box.SetBoxShape(cubeSize, cubeSize);
            box.Friction = FP.FromFloat(0.5f);
            box.Restitution = FP.FromFloat(0.2f);
            
            world.AddBody(box);
        }

        Debug.Log($"LockSim World initialized with {world.Bodies.Count} bodies");
    }

    void Update()
    {
        if (!runSimulation)
            return;

        // Fixed timestep accumulator
        accumulator += Time.deltaTime * timeScale;
        float fixedDt = fixedDeltaTime.ToFloat();

        int steps = 0;
        while (accumulator >= fixedDt && steps < stepsPerFrame)
        {
            PhysicsEngine.Step(world, fixedDeltaTime);
            accumulator -= fixedDt;
            steps++;
        }

        // Prevent spiral of death
        if (accumulator > fixedDt * 10)
        {
            accumulator = 0;
        }
    }

    void OnDrawGizmos()
    {
        if (world == null || world.Bodies == null)
            return;

        // Draw all bodies
        foreach (var body in world.Bodies)
        {
            Gizmos.color = body.BodyType == BodyType.Static ? staticBodyColor : dynamicBodyColor;
            DrawBody(body);
        }

        // Draw contacts
        if (showContacts && world.Contacts != null)
        {
            Gizmos.color = contactColor;
            foreach (var contact in world.Contacts)
            {
                for (int i = 0; i < contact.ContactCount; i++)
                {
                    FPVector2 point = i == 0 ? contact.ContactPoint1 : contact.ContactPoint2;
                    Vector3 worldPoint = new Vector3(point.X.ToFloat(), point.Y.ToFloat(), 0);
                    
                    // Draw contact point
                    Gizmos.DrawSphere(worldPoint, 0.1f);
                    
                    // Draw contact normal
                    Vector3 normal = new Vector3(contact.Normal.X.ToFloat(), contact.Normal.Y.ToFloat(), 0);
                    Gizmos.DrawRay(worldPoint, normal * 0.5f);
                }
            }
        }
    }

    private void DrawBody(RigidBody body)
    {
        Vector3 position = new Vector3(body.Position.X.ToFloat(), body.Position.Y.ToFloat(), 0);
        Quaternion rotation = Quaternion.Euler(0, 0, body.Rotation.ToFloat() * Mathf.Rad2Deg);

        if (body.ShapeType == ShapeType.Box)
        {
            Vector3 size = new Vector3(
                body.BoxShape.HalfWidth.ToFloat() * 2,
                body.BoxShape.HalfHeight.ToFloat() * 2,
                0.5f
            );
            
            // Draw box with rotation
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = Matrix4x4.identity;
        }
        else if (body.ShapeType == ShapeType.Circle)
        {
            DrawWireCircle(position, body.CircleShape.Radius.ToFloat());
        }
    }

    private void DrawWireCircle(Vector3 center, float radius, int segments = 32)
    {
        float angle = 0f;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"LockSim Demo - Deterministic 2D Physics", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold });
        GUILayout.Space(10);
        GUILayout.Label($"Bodies: {world?.Bodies.Count ?? 0}");
        GUILayout.Label($"Contacts: {world?.Contacts.Count ?? 0}");
        GUILayout.Label($"Fixed DT: {fixedDeltaTime.ToFloat():F4}s");
        GUILayout.Space(10);
        
        if (GUILayout.Button(runSimulation ? "Pause" : "Resume"))
        {
            runSimulation = !runSimulation;
        }
        
        if (GUILayout.Button("Reset"))
        {
            accumulator = 0f;
            InitializeWorld();
        }
        
        GUILayout.EndArea();
    }
}

