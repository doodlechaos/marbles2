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
    private InputEvent.GameplayStartInput _gameplayStartInput;

    [SerializeField]
    private RenderPrefabRegistry prefabRegistry;

    [Header("Rendering")]
    [Tooltip("The TileRenderer to use for displaying the tile")]
    [SerializeField]
    private TileRenderer tileRenderer;

    [Header("Debug Info")]
    [SerializeField]
    private bool _isRunning;

    /// <summary>
    /// The GameTile currently being tested.
    /// </summary>
    public GameTileBase GameTile { get; private set; }

    /// <summary>
    /// Output events from the last step, useful for debugging.
    /// </summary>
    public OutputEventBuffer LastOutputEvents { get; private set; } = new();

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
            StartGameplay(_gameplayStartInput);
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
        if (!_isRunning || GameTile == null)
            return;

        StepOnce();
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

    private void StartGameplay(InputEvent.GameplayStartInput gameplayStartInput)
    {
        if (GameTile == null)
        {
            Debug.LogWarning("[GameTilePlayground] No GameTile loaded");
            return;
        }

        GameTile.StartGameplay(gameplayStartInput, LastOutputEvents);

        Debug.Log($"[GameTilePlayground] Gameplay started with {gameplayStartInput} test entrants");
    }

    /// <summary>
    /// Pause the simulation.
    /// </summary>
    [ProButton]
    public void PauseSimulation()
    {
        _isRunning = false;
        Debug.Log($"[GameTilePlayground] Simulation paused");
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
    }

    /// <summary>
    /// Reload and restart the simulation from the beginning.
    /// </summary>
    [ProButton]
    public void RestartSimulation()
    {
        LoadFromPrefab();
        StartGameplay(_gameplayStartInput);
        Debug.Log("[GameTilePlayground] Simulation restarted");
    }
}
