using System;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Component that binds a Unity GameObject (the render root) to a ThroneTile.
/// Provides easy access to throne state information from Unity's inspector and scripts.
/// </summary>
[Serializable]
public sealed class ThroneTileBinding : TileBinding
{
    /// <summary>
    /// Reference to the ThroneTile this render root represents.
    /// </summary>
    [SerializeField]
    private ThroneTile throneTile;

    /// <summary>
    /// The ThroneTile this binding is associated with.
    /// </summary>
    public ThroneTile ThroneTile
    {
        get => throneTile;
        set => throneTile = value;
    }

    /// <summary>
    /// Implementation of TileBinding.Tile
    /// </summary>
    public override TileBase Tile => throneTile;

    /// <summary>
    /// Implementation of TileBinding.IsValid
    /// </summary>
    public override bool IsValid => throneTile != null;
}
