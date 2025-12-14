using com.cyborgAssets.inspectorButtonPro;
using GameCoreLib;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GameTilePlayground allows testing GameTiles independently from networking.
/// Converts a GameTileAuthBase prefab into runtime data and simulates it locally.
/// </summary>
public class ThroneTilePlayground : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("The ThroneTileAuth prefab to convert and simulate")]
    [SerializeField]
    private ThroneTileAuthBase inputThroneTileAuthPrefab;

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
    public ThroneTile ThroneTile { get; private set; }

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
        if (inputThroneTileAuthPrefab != null)
        {
            LoadFromPrefab();
        }
    }

    private void Update()
    {
        if (tileRenderer != null)
        {
            tileRenderer.Render(ThroneTile);
            tileRenderer.PhysicsSim = ThroneTile?.Sim;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return; // no keyboard available (e.g. on some devices)

        if (keyboard.spaceKey.wasPressedThisFrame)
            _isRunning = !_isRunning;

        if (keyboard.rKey.wasPressedThisFrame)
            LoadFromPrefab();

        if (keyboard.rightArrowKey.wasPressedThisFrame)
            StepOnce();
    }

    private void FixedUpdate()
    {
        if (!_isRunning || ThroneTile == null)
            return;

        StepOnce();
    }

    /// <summary>
    /// Convert the InputGameTileAuthPrefab into a GameTile and initialize it.
    /// </summary>
    [ProButton]
    public void LoadFromPrefab()
    {
        if (inputThroneTileAuthPrefab == null)
        {
            Debug.LogError("[GameTilePlayground] InputGameTileAuthPrefab is not assigned");
            return;
        }

        ThroneTile = ThroneTileConverter.ConvertToThroneTile(
            inputThroneTileAuthPrefab.gameObject,
            prefabRegistry
        );

        if (ThroneTile != null)
        {
            ThroneTile.Initialize(1);

            Debug.Log($"[ThroneTilePlayground] Loaded: {ThroneTile.TileRoot?.Name ?? "Unknown"}");
        }
    }

    /// <summary>
    /// Step the simulation forward by one tick.
    /// </summary>
    [ProButton]
    public void StepOnce()
    {
        if (ThroneTile == null)
        {
            Debug.LogWarning("[GameTilePlayground] No GameTile loaded");
            return;
        }

        LastOutputEvents.Clear();
        ThroneTile.SetOutputEventsBufferReference(LastOutputEvents);
        ThroneTile.Step();
    }
}
