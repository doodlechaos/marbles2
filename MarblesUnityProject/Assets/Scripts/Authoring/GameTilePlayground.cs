using com.cyborgAssets.inspectorButtonPro;
using GameCoreLib;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [Tooltip("The TileBinding to use for displaying the tile")]
    [SerializeField]
    private GameTileBinding tileBinding;

    [Header("Debug Info")]
    [SerializeField]
    private bool _isRunning;

    [SerializeField, Min(0f)]
    private float _stepSpeedMultiplier = 1f;

    // Your sim tick rate (GameTile.Step() == 1 tick)
    [SerializeField, Min(1)]
    private int _simTicksPerSecond = 60;

    // Safety so huge multipliers don’t hang the editor
    [SerializeField, Min(1)]
    private int _maxStepsPerFixedUpdate = 50;

    private double _tickAccumulator;

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
        if (tileBinding != null)
        {
            tileBinding.GameTile = GameTile;
            tileBinding.Render();
        }

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return; // no keyboard available (e.g. on some devices)

        if (keyboard.spaceKey.wasPressedThisFrame)
            _isRunning = !_isRunning;

        if (keyboard.rKey.wasPressedThisFrame)
            RestartSimulation();

        if (keyboard.rightArrowKey.wasPressedThisFrame)
            StepOnce();
    }

    private void FixedUpdate()
    {
        if (!_isRunning || GameTile == null)
            return;

        // Use unscaled so Unity timescale doesn’t affect your test speed (optional).
        double dt = Time.fixedUnscaledDeltaTime;

        // Convert real time into “how many sim ticks should run”
        _tickAccumulator += dt * _simTicksPerSecond * _stepSpeedMultiplier;

        int steps = 0;
        while (_tickAccumulator >= 1.0 && steps < _maxStepsPerFixedUpdate)
        {
            _tickAccumulator -= 1.0;
            StepOnce();
            steps++;
        }

        // If we hit the cap, drop accumulated debt to avoid spiraling forever.
        if (steps == _maxStepsPerFixedUpdate)
            _tickAccumulator = 0;
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
            GameTile.InitTile(1);

            Debug.Log($"[GameTilePlayground] Loaded: {GameTile.TileRoot?.Name ?? "Unknown"}");
        }
        _tickAccumulator = 0;
    }

    private void StartGameplay(InputEvent.GameplayStartInput gameplayStartInput)
    {
        if (GameTile == null)
        {
            Debug.LogWarning("[GameTilePlayground] No GameTile loaded");
            return;
        }

        GameTile.StartGameplay(gameplayStartInput);

        Debug.Log($"[GameTilePlayground] Gameplay started with {gameplayStartInput} test entrants");
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
        GameTile.SetOutputEventsBufferReference(LastOutputEvents);
        GameTile.Step();

        foreach (var outputEvent in LastOutputEvents.Events)
        {
            Debug.Log($"[GameTilePlayground] Output event: {outputEvent.GetType().Name}");
        }
    }

    /// <summary>
    /// Reload and restart the simulation from the beginning.
    /// </summary>
    [ProButton]
    public void RestartSimulation()
    {
        _tickAccumulator = 0;
        LoadFromPrefab();
        StartGameplay(_gameplayStartInput);
        Debug.Log("[GameTilePlayground] Simulation restarted");
    }
}
