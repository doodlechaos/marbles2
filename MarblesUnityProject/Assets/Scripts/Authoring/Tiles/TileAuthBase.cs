using UnityEngine;

/// <summary>
/// Base class for all tile authoring components.
/// Contains shared properties needed by all tile types.
/// </summary>
public abstract class TileAuthBase : MonoBehaviour
{
    [Tooltip("The marble prefab to spawn for players in this tile")]
    public MarbleAuth MarblePrefab;
}
