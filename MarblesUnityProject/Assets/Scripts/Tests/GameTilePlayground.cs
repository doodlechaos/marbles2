using com.cyborgAssets.inspectorButtonPro;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// GameTilePlayground allows testing GameTiles independently from networking.
/// Converts a GameTileAuthBase prefab into runtime data and simulates it locally.
/// </summary>
public class GameTilePlayground : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("The GameTileAuth prefab to convert and simulate")]
    [SerializeField]
    private GameTileAuthBase inputGameTileAuthPrefab;

    [SerializeField]
    private RenderPrefabRegistry prefabRegistry;

    [Header("Rendering")]
    [Tooltip("The TileRenderer to use for displaying the tile")]
    [SerializeField]
    private TileRenderer tileRenderer;

    [Header("Simulation Control")]
    [Tooltip("Whether to automatically step the simulation each FixedUpdate")]
    public bool AutoStep = true;

    [Tooltip("Time scale for the simulation (1 = normal speed)")]
    [Range(0.1f, 3f)]
    public float TimeScale = 1f;

    [Header("Debug Info")]
    [SerializeField]
    private bool _isRunning;

    [SerializeField]
    private int _stepCount;

    /// <summary>
    /// The GameTile currently being tested.
    /// </summary>
    public GameTileBase GameTile { get; private set; }

    /// <summary>
    /// Output events from the last step, useful for debugging.
    /// </summary>
    public OutputEventBuffer LastOutputEvents { get; private set; } = new();

    private float _stepAccumulator;
    private const float FIXED_TIMESTEP = 1f / 60f;

    private void Awake()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;
    }

    private void Start()
    {
        if (inputGameTileAuthPrefab != null)
        {
            LoadFromPrefab();
        }
    }

    private void Update()
    {
        if (tileRenderer != null)
        {
            tileRenderer.Render(GameTile);
            tileRenderer.PhysicsSim = GameTile?.Sim;
        }
    }

    private void FixedUpdate()
    {
        if (!_isRunning || !AutoStep || GameTile == null)
            return;

        _stepAccumulator += Time.fixedDeltaTime * TimeScale;

        while (_stepAccumulator >= FIXED_TIMESTEP)
        {
            _stepAccumulator -= FIXED_TIMESTEP;
            StepOnce();
        }
    }

    /// <summary>
    /// Convert the InputGameTileAuthPrefab into a GameTile and initialize it.
    /// </summary>
    [ProButton]
    public void LoadFromPrefab()
    {
        if (inputGameTileAuthPrefab == null)
        {
            Debug.LogError("[GameTilePlayground] InputGameTileAuthPrefab is not assigned");
            return;
        }

        GameTile = GameTileConverter.ConvertToGameTile(
            inputGameTileAuthPrefab.gameObject,
            prefabRegistry
        );

        if (GameTile != null)
        {
            GameTile.Initialize(1);
            _stepCount = 0;
            _isRunning = false;

            Debug.Log($"[GameTilePlayground] Loaded: {GameTile.TileRoot?.Name ?? "Unknown"}");
        }
    }

    /// <summary>
    /// Start the simulation (or resume if paused).
    /// </summary>
    [ProButton]
    public void StartSimulation()
    {
        if (GameTile == null)
        {
            Debug.LogWarning("[GameTilePlayground] No GameTile loaded");
            return;
        }

        _isRunning = true;
        Debug.Log("[GameTilePlayground] Simulation started");
    }

    /// <summary>
    /// Start gameplay with test entrants.
    /// </summary>
    [ProButton]
    public void StartGameplayWithTestEntrants()
    {
        if (GameTile == null)
        {
            Debug.LogWarning("[GameTilePlayground] No GameTile loaded");
            return;
        }

        var testEntrants = new InputEvent.Entrant[]
        {
            new() { AccountId = 1, TotalBid = 10 },
            new() { AccountId = 2, TotalBid = 15 },
            new() { AccountId = 3, TotalBid = 20 },
        };

        uint totalMarbles = 0;
        foreach (var e in testEntrants)
            totalMarbles += e.TotalBid;

        GameTile.StartGameplay(testEntrants, totalMarbles, LastOutputEvents);
        _isRunning = true;

        Debug.Log(
            $"[GameTilePlayground] Gameplay started with {testEntrants.Length} test entrants"
        );
    }

    /// <summary>
    /// Pause the simulation.
    /// </summary>
    [ProButton]
    public void PauseSimulation()
    {
        _isRunning = false;
        Debug.Log($"[GameTilePlayground] Simulation paused at step {_stepCount}");
    }

    /// <summary>
    /// Step the simulation forward by one tick.
    /// </summary>
    [ProButton]
    public void StepOnce()
    {
        if (GameTile == null)
        {
            Debug.LogWarning("[GameTilePlayground] No GameTile loaded");
            return;
        }

        LastOutputEvents.Clear();
        GameTile.Step(LastOutputEvents);
        _stepCount++;
    }

    /// <summary>
    /// Reload and restart the simulation from the beginning.
    /// </summary>
    [ProButton]
    public void RestartSimulation()
    {
        LoadFromPrefab();
        _stepAccumulator = 0f;
        Debug.Log("[GameTilePlayground] Simulation restarted");
    }

    /// <summary>
    /// Clear the current GameTile.
    /// </summary>
    [ProButton]
    public void ClearTile()
    {
        GameTile = null;
        _stepCount = 0;
        _isRunning = false;

        if (tileRenderer != null)
        {
            tileRenderer.Render(null);
            tileRenderer.PhysicsSim = null;
            tileRenderer.ClearRendering();
        }

        Debug.Log("[GameTilePlayground] Tile cleared");
    }

    /// <summary>
    /// Log current state info for debugging.
    /// </summary>
    [ProButton]
    public void LogStateInfo()
    {
        if (GameTile == null)
        {
            Debug.Log("[GameTilePlayground] No GameTile loaded");
            return;
        }

        Debug.Log($"=== GameTile State ===");
        Debug.Log($"  Step Count: {_stepCount}");
        Debug.Log($"  Is Running: {_isRunning}");
        Debug.Log($"  State: {GameTile.State}");

        if (GameTile.Sim != null)
        {
            Debug.Log($"  Physics Bodies: {GameTile.Sim.Bodies.Count}");
        }

        if (GameTile.TileRoot != null)
        {
            int objCount = CountRuntimeObjects(GameTile.TileRoot);
            Debug.Log($"  RuntimeObj Count: {objCount}");
        }
    }

    private int CountRuntimeObjects(RuntimeObj obj)
    {
        int count = 1;
        if (obj.Children != null)
        {
            foreach (var child in obj.Children)
            {
                count += CountRuntimeObjects(child);
            }
        }
        return count;
    }
}
