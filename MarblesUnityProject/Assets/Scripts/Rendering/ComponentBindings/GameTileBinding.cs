using System;
using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Component that binds a Unity GameObject (the render root) to a GameTileBase.
/// Provides easy access to tile state information from Unity's inspector and scripts.
/// Also handles door animation logic for game tiles.
/// </summary>
[Serializable]
public sealed class GameTileBinding : TileBinding
{
    /// <summary>
    /// Reference to the GameTileBase this render root represents.
    /// </summary>
    [SerializeReference]
    private GameTileBase gameTile;

    [Header("Door Animation")]
    [SerializeField]
    private DoorAnimationCfg doorConfig;

    [Header("Debug Display")]
    [SerializeField]
    private Vector2 screenOffset = new Vector2(0, 50);

    // Door animation state
    private ObjectPool<AnimationDoor> doorPool;
    private List<AnimationDoor> spinDoors = new List<AnimationDoor>();
    private Transform spinningDoorsRoot;
    private Transform openCloseDoor;
    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    /// <summary>
    /// The GameTile this binding is associated with.
    /// </summary>
    public GameTileBase GameTile
    {
        get => gameTile;
        set => gameTile = value;
    }

    /// <summary>
    /// Implementation of TileBinding.Tile
    /// </summary>
    public override TileBase Tile => gameTile;

    /// <summary>
    /// Implementation of TileBinding.IsValid
    /// </summary>
    public override bool IsValid => gameTile != null;

    /// <summary>
    /// The current state of the game tile.
    /// </summary>
    public GameTileState State => gameTile?.State ?? GameTileState.Spinning;

    /// <summary>
    /// The name of the GameTile type (e.g., "SimpleBattleRoyale").
    /// </summary>
    public string GameTypeName => gameTile?.GetType().Name ?? "Unknown";

    /// <summary>
    /// Try to get the GameTile as a specific derived type.
    /// </summary>
    /// <typeparam name="T">The specific GameTile type to cast to.</typeparam>
    /// <param name="result">The casted GameTile if successful.</param>
    /// <returns>True if the cast was successful.</returns>
    public bool TryGetGameTile<T>(out T result)
        where T : GameTileBase
    {
        result = gameTile as T;
        return result != null;
    }

    private void Awake()
    {
        InitializeDoorAnimation();
    }

    private void InitializeDoorAnimation()
    {
        if (doorConfig == null || doorConfig.AnimationDoorPrefab == null)
            return;

        doorPool = new ObjectPool<AnimationDoor>(
            () => Instantiate(doorConfig.AnimationDoorPrefab, transform),
            door => door.gameObject.SetActive(true),
            door => door.gameObject.SetActive(false),
            door => DestroyImmediate(door.gameObject),
            true,
            10,
            100
        );

        spinningDoorsRoot = new GameObject("SpinningDoorsRoot").transform;
        spinningDoorsRoot.SetParent(transform);
        spinningDoorsRoot.localPosition = new Vector3(0, 0, doorConfig.SpinRootPosZOffset);
    }

    private void Update()
    {
        if (gameTile != null && doorConfig != null)
        {
            UpdateDoors();
        }
    }

    private void UpdateDoors()
    {
        float direction = (gameTile.TileWorldId == 1) ? -1 : 1;

        // If we're in closing door state, interpolate between the start and end positions
        if (gameTile.State == GameTileState.ClosingDoor)
        {
            float t = gameTile.StateSteps / (GameTileBase.CLOSING_DOOR_DURATION_SEC * 60.0f);
            EnsureOpenCloseDoorSpawned();
            openCloseDoor.transform.position = Vector3.LerpUnclamped(
                transform.position + new Vector3(doorConfig.OpenDoorOffset * direction, 0, 0),
                transform.position,
                t
            );
        }

        // If we're in spinning door state, animate the spinning doors
        if (gameTile.State == GameTileState.Spinning)
        {
            float t = gameTile.StateSteps / (GameTileBase.SPINNING_DURATION_SEC * 60.0f);
            t = doorConfig.SpinAnimationCurve.Evaluate(t);

            EnsureSpinningDoorsSpawned(doorConfig.AnimationDoorCount);

            float rootStartRotationX = (spinDoors.Count - 1) * doorConfig.SpinDegreesPerDoor;
            float rootEndRotationX = 0;

            float x =
                rootStartRotationX + Mathf.DeltaAngle(rootStartRotationX, rootEndRotationX) * t;
            spinningDoorsRoot.localRotation = Quaternion.Euler(x, 0f, 0f);
        }
        else if (spinningDoorsRoot != null)
        {
            spinningDoorsRoot.gameObject.SetActive(false);
        }

        // If we're in opening door state, interpolate between the start and end positions
        if (gameTile.State == GameTileState.OpeningDoor)
        {
            float t = gameTile.StateSteps / (GameTileBase.OPENING_DOOR_DURATION_SEC * 60.0f);
            EnsureOpenCloseDoorSpawned();
            openCloseDoor.transform.position = Vector3.LerpUnclamped(
                transform.position,
                transform.position + new Vector3(doorConfig.OpenDoorOffset * direction, 0, 0),
                t
            );
        }
    }

    private void EnsureOpenCloseDoorSpawned()
    {
        if (openCloseDoor != null || doorPool == null)
            return;

        openCloseDoor = doorPool.Get().transform;
        openCloseDoor.gameObject.SetActive(true);
    }

    private void EnsureSpinningDoorsSpawned(int count)
    {
        if (spinningDoorsRoot == null || doorPool == null)
            return;

        if (spinningDoorsRoot.gameObject.activeSelf)
            return;

        spinningDoorsRoot.gameObject.SetActive(true);
        spinningDoorsRoot.eulerAngles = Vector3.zero;

        // Release existing doors back to pool
        for (int i = spinDoors.Count - 1; i >= 0; i--)
        {
            doorPool.Release(spinDoors[i]);
            spinDoors.RemoveAt(i);
        }

        // Spawn new doors
        for (int i = 0; i < count; i++)
        {
            AnimationDoor door = doorPool.Get();
            spinDoors.Add(door);

            door.transform.position = transform.position;
            door.transform.localRotation = Quaternion.identity;
            door.transform.SetParent(spinningDoorsRoot);

            spinningDoorsRoot.Rotate(new Vector3(doorConfig.SpinDegreesPerDoor, 0, 0), Space.World);
        }
    }

    private void OnDestroy()
    {
        // Clean up pooled objects
        if (doorPool != null)
        {
            foreach (var door in spinDoors)
            {
                if (door != null)
                    doorPool.Release(door);
            }
            spinDoors.Clear();
            doorPool.Dispose();
        }
    }

    void OnGUI()
    {
        if (gameTile == null)
            return;

        // Lazy init styles
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
        }

        // Convert world position to screen position
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

        // Skip if behind camera
        if (screenPos.z < 0)
            return;

        // Flip Y for GUI coordinates
        float guiY = Screen.height - screenPos.y;

        // Build display text
        string stateText = $"Tile {TileWorldId}: {gameTile.State}";
        string stepsText = $"Steps: {gameTile.StateSteps}";

        // Draw box with label
        Rect boxRect = new Rect(
            screenPos.x + screenOffset.x - 160,
            guiY + screenOffset.y - 30,
            320,
            90
        );

        // Color based on state
        Color stateColor = gameTile.State switch
        {
            GameTileState.Spinning => Color.yellow,
            GameTileState.OpeningDoor => Color.cyan,
            GameTileState.Bidding => Color.green,
            GameTileState.Gameplay => Color.white,
            GameTileState.ScoreScreen => Color.magenta,
            _ => Color.white,
        };

        GUI.backgroundColor = stateColor;
        GUI.Box(boxRect, "", boxStyle);

        // Draw state text
        Rect stateRect = new Rect(boxRect.x, boxRect.y + 10, boxRect.width, 30);
        GUI.Label(stateRect, stateText, labelStyle);

        // Draw steps text below
        Rect stepsRect = new Rect(boxRect.x, boxRect.y + 45, boxRect.width, 30);
        GUI.Label(stepsRect, stepsText, labelStyle);

        GUI.backgroundColor = Color.white;
    }
}
