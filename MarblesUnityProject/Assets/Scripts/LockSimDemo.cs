using UnityEngine;
using System.Collections.Generic;
using LockSim;

/// <summary>
/// Unity demo for the LockSim deterministic physics engine
/// Scans the scene for GameObjects with Collider2D/Rigidbody2D and syncs them with LockSim
/// </summary>
public class LockSimDemo : MonoBehaviour
{
    [Header("Simulation Settings")]
    [SerializeField] private bool runSimulation = true;
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private int stepsPerFrame = 1;
    [SerializeField] private float fixedTimeStep = 1f / 60f;
    
    [Header("World Settings")]
    [SerializeField] private Vector2 gravity = new Vector2(0f, -9.81f);
    [SerializeField] private int velocityIterations = 8;
    [SerializeField] private int positionIterations = 3;

    [Header("Visualization")]
    [SerializeField] private Color contactColor = Color.red;
    [SerializeField] private bool showContacts = true;
    [SerializeField] private float contactPointSize = 0.1f;

    private World world;
    private WorldSimulationContext context;
    private FP fixedDeltaTime;
    private float accumulator = 0f;
    
    // Mapping between Unity GameObjects and LockSim body IDs
    private Dictionary<GameObject, int> gameObjectToBodyId = new Dictionary<GameObject, int>();
    private Dictionary<int, GameObject> bodyIdToGameObject = new Dictionary<int, GameObject>();

    void Start()
    {
        fixedDeltaTime = FP.FromFloat(fixedTimeStep);
        InitializeWorld();
    }

    void InitializeWorld()
    {
        world = new World();
        world.Gravity = FPVector2.FromFloats(gravity.x, gravity.y);
        
        context = new WorldSimulationContext();
        context.VelocityIterations = velocityIterations;
        context.PositionIterations = positionIterations;

        // Clear previous mappings
        gameObjectToBodyId.Clear();
        bodyIdToGameObject.Clear();

        // Scan scene for all GameObjects with physics components
        Collider2D[] colliders = FindObjectsOfType<Collider2D>();
        
        int bodiesCreated = 0;
        foreach (var collider in colliders)
        {
            if (CreateBodyFromCollider(collider.gameObject))
            {
                bodiesCreated++;
            }
        }

        Debug.Log($"LockSim World initialized with {bodiesCreated} bodies from scene");
    }

    private bool CreateBodyFromCollider(GameObject go)
    {
        // Skip if already added
        if (gameObjectToBodyId.ContainsKey(go))
            return false;

        Collider2D collider = go.GetComponent<Collider2D>();
        if (collider == null)
            return false;

        // CRITICAL: Use collider.bounds.center for the actual world-space center of the collider
        // This automatically accounts for offset, scale, rotation, and any other transforms
        Vector2 colliderCenter = collider.bounds.center;
        float rotationZ = go.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        // Determine if static or dynamic
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
        bool isStatic = rb2d == null || rb2d.bodyType == RigidbodyType2D.Static;

        RigidBodyLS body;
        
        if (isStatic)
        {
            body = RigidBodyLS.CreateStatic(
                0, // ID will be assigned by world
                FPVector2.FromFloats(colliderCenter.x, colliderCenter.y),
                FP.FromFloat(rotationZ)
            );
        }
        else
        {
            float mass = rb2d.mass;
            body = RigidBodyLS.CreateDynamic(
                0, // ID will be assigned by world
                FPVector2.FromFloats(colliderCenter.x, colliderCenter.y),
                FP.FromFloat(rotationZ),
                FP.FromFloat(mass)
            );

            // Copy velocities if present
            body.LinearVelocity = FPVector2.FromFloats(rb2d.linearVelocity.x, rb2d.linearVelocity.y);
            body.AngularVelocity = FP.FromFloat(rb2d.angularVelocity * Mathf.Deg2Rad);
        }

        // Set shape based on collider type
        // Use local collider size scaled by transform (NOT bounds, which is axis-aligned and inflated for rotated objects)
        if (collider is BoxCollider2D boxCollider)
        {
            // Get local size and apply transform scale
            Vector2 localSize = boxCollider.size;
            Vector2 worldSize = new Vector2(
                localSize.x * Mathf.Abs(go.transform.lossyScale.x),
                localSize.y * Mathf.Abs(go.transform.lossyScale.y)
            );
            body.SetBoxShape(FP.FromFloat(worldSize.x), FP.FromFloat(worldSize.y));
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            // Get local radius and apply transform scale (use max of x/y scale for circles)
            float localRadius = circleCollider.radius;
            float worldRadius = localRadius * Mathf.Max(Mathf.Abs(go.transform.lossyScale.x), Mathf.Abs(go.transform.lossyScale.y));
            body.SetCircleShape(FP.FromFloat(worldRadius));
        }
        else
        {
            Debug.LogWarning($"Unsupported collider type on {go.name}: {collider.GetType().Name}");
            return false;
        }

        // Set material properties
        PhysicsMaterial2D material = collider.sharedMaterial;
        if (material != null)
        {
            body.Friction = FP.FromFloat(material.friction);
            body.Restitution = FP.FromFloat(material.bounciness);
        }
        else
        {
            body.Friction = FP.FromFloat(0.5f);
            body.Restitution = FP.FromFloat(0.2f);
        }

        // Add body to world and store mapping
        int bodyId = world.AddBody(body);
        gameObjectToBodyId[go] = bodyId;
        bodyIdToGameObject[bodyId] = go;

        return true;
    }

    void Update()
    {
        if (!runSimulation || world == null)
            return;

        // Fixed timestep accumulator
        accumulator += Time.deltaTime * timeScale;
        float fixedDt = fixedDeltaTime.ToFloat();

        int steps = 0;
        while (accumulator >= fixedDt && steps < stepsPerFrame)
        {
            PhysicsPipeline.Step(world, fixedDeltaTime, context);
            accumulator -= fixedDt;
            steps++;
        }

        // Prevent spiral of death
        if (accumulator > fixedDt * 10)
        {
            accumulator = 0;
        }

        // Sync GameObject transforms from LockSim bodies
        SyncTransforms();
    }

    void SyncTransforms()
    {
        foreach (var body in world.Bodies)
        {
            if (bodyIdToGameObject.TryGetValue(body.Id, out GameObject go))
            {
                if (go != null && body.BodyType == BodyType.Dynamic)
                {
                    Collider2D collider = go.GetComponent<Collider2D>();
                    if (collider == null)
                        continue;

                    // Body position in LockSim is the collider's center
                    // We need to compute what transform.position should be to achieve that collider center
                    // Formula: collider.bounds.center = transform.position + RotatedOffset
                    // So: transform.position = desiredColliderCenter - RotatedOffset
                    
                    Vector2 desiredColliderCenter = new Vector2(body.Position.X.ToFloat(), body.Position.Y.ToFloat());
                    float rotationRad = body.Rotation.ToFloat();
                    
                    // First update rotation (this affects how offset is computed)
                    float rotationDegrees = rotationRad * Mathf.Rad2Deg;
                    go.transform.rotation = Quaternion.Euler(0, 0, rotationDegrees);
                    
                    // Calculate the rotated offset from transform.position to collider.bounds.center
                    Vector2 colliderOffset = collider.offset;
                    Vector2 scaledOffset = new Vector2(
                        colliderOffset.x * go.transform.lossyScale.x,
                        colliderOffset.y * go.transform.lossyScale.y
                    );
                    
                    float cos = Mathf.Cos(rotationRad);
                    float sin = Mathf.Sin(rotationRad);
                    Vector2 rotatedOffset = new Vector2(
                        scaledOffset.x * cos - scaledOffset.y * sin,
                        scaledOffset.x * sin + scaledOffset.y * cos
                    );
                    
                    // Transform position = desired collider center - offset
                    Vector2 transformPosition = desiredColliderCenter - rotatedOffset;
                    
                    // Update position
                    Vector3 newPosition = new Vector3(
                        transformPosition.x,
                        transformPosition.y,
                        go.transform.position.z // Preserve Z
                    );
                    go.transform.position = newPosition;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showContacts || world == null || context == null || context.Contacts == null)
            return;

        // Draw contact points and normals
        Gizmos.color = contactColor;
        foreach (var contact in context.Contacts)
        {
            for (int i = 0; i < contact.ContactCount; i++)
            {
                FPVector2 point = i == 0 ? contact.ContactPoint1 : contact.ContactPoint2;
                Vector3 worldPoint = new Vector3(point.X.ToFloat(), point.Y.ToFloat(), 0);
                
                // Draw contact point
                Gizmos.DrawSphere(worldPoint, contactPointSize);
                
                // Draw contact normal
                Vector3 normal = new Vector3(contact.Normal.X.ToFloat(), contact.Normal.Y.ToFloat(), 0);
                Gizmos.DrawRay(worldPoint, normal * 0.5f);
            }
        }
    }

    void OnGUI()
    {
        if (world == null)
            return;

        GUILayout.BeginArea(new Rect(10, 10, 350, 200));
        
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        GUILayout.Label("LockSim Demo - Scene Sync", headerStyle);
        
        GUILayout.Space(10);
        GUILayout.Label($"Bodies: {world.Bodies.Count} (from scene)");
        GUILayout.Label($"Contacts: {(context != null ? context.Contacts.Count : 0)}");
        GUILayout.Label($"Fixed DT: {fixedTimeStep:F4}s");
        GUILayout.Label($"Time Scale: {timeScale:F2}x");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button(runSimulation ? "Pause Simulation" : "Resume Simulation"))
        {
            runSimulation = !runSimulation;
        }
        
        if (GUILayout.Button("Reset Simulation"))
        {
            accumulator = 0f;
            InitializeWorld();
        }
        
        GUILayout.EndArea();
    }
}

