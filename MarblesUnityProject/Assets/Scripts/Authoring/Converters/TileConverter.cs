using GameCoreLib;
using UnityEngine;

/// <summary>
/// Shared utilities for converting tile authoring data to GameCore runtime structures.
/// Used by GameTileConverter and ThroneTileConverter.
/// </summary>
public static class TileConverter
{
    /// <summary>
    /// Build the PlayerMarbleTemplate from a TileAuthBase's MarblePrefab.
    /// Shared by all tile types that support spawning player marbles.
    /// </summary>
    /// <param name="tileAuth">The tile auth component containing the marble prefab reference</param>
    /// <param name="prefabRegistry">The render prefab registry for serialization</param>
    /// <returns>The serialized marble template, or null if not configured</returns>
    public static GameCoreObj BuildPlayerMarbleTemplate(
        TileAuthBase tileAuth,
        RenderPrefabRegistry prefabRegistry
    )
    {
        if (tileAuth == null)
        {
            Debug.LogError("[TileConverter] TileAuth is null");
            return null;
        }

        if (tileAuth.MarblePrefab == null)
        {
            Debug.LogWarning(
                $"[TileConverter] {tileAuth.GetType().Name} has no MarblePrefab assigned"
            );
            return null;
        }

        if (prefabRegistry == null)
        {
            Debug.LogError(
                "[TileConverter] PrefabRegistry is null – cannot serialize MarblePrefab"
            );
            return null;
        }

        return GameObjectToGCObj.SerializeGameObject(
            tileAuth.MarblePrefab.gameObject,
            prefabRegistry
        );
    }

    /// <summary>
    /// Convert a GameObject hierarchy to a GameCoreObj tree.
    /// Thin wrapper for GameObjectToGCObj.SerializeGameObject.
    /// </summary>
    public static GameCoreObj ConvertHierarchy(
        GameObject prefab,
        RenderPrefabRegistry prefabRegistry
    )
    {
        if (prefab == null)
        {
            Debug.LogError("[TileConverter] Prefab is null");
            return null;
        }

        return GameObjectToGCObj.SerializeGameObject(prefab, prefabRegistry);
    }
}
