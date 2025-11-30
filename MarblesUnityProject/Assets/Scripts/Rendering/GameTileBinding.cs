using System;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Component that binds a Unity GameObject (the render root) to a GameTileBase.
/// Provides easy access to tile state information from Unity's inspector and scripts.
/// </summary>
[Serializable]
public sealed class GameTileBinding : MonoBehaviour
{
    /// <summary>
    /// Reference to the GameTileBase this render root represents.
    /// </summary>
    [SerializeField]
    private GameTileBase gameTile;

    /// <summary>
    /// The GameTile this binding is associated with.
    /// </summary>
    public GameTileBase GameTile
    {
        get => gameTile;
        set => gameTile = value;
    }

    /// <summary>
    /// The current state of the game tile.
    /// </summary>
    public GameTileState State => gameTile?.State ?? GameTileState.Spinning;

    /// <summary>
    /// The tile's world ID (1 or 2).
    /// </summary>
    public byte TileWorldId => gameTile?.TileWorldId ?? 0;

    /// <summary>
    /// The name of the GameTile type (e.g., "SimpleBattleRoyale").
    /// </summary>
    public string GameTypeName => gameTile?.GetType().Name ?? "Unknown";

    /// <summary>
    /// Check if this binding has a valid GameTile reference.
    /// </summary>
    public bool IsValid => gameTile != null;

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
}
