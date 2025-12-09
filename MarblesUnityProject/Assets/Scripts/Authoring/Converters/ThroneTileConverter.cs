using GameCoreLib;
using UnityEngine;

/// <summary>
/// Converts Unity GameObjects with ThroneTileAuthBase components into ThroneTile data.
/// Uses TileConverter utilities for shared conversion logic.
/// </summary>
public static class ThroneTileConverter
{
    /// <summary>
    /// Convert a GameObject with a ThroneTileAuthBase into a fully populated ThroneTile.
    /// </summary>
    /// <param name="prefab">The GameObject with a ThroneTileAuthBase component</param>
    /// <param name="prefabRegistry">The RenderPrefabRegistry for looking up RenderPrefabIDs</param>
    /// <returns>A populated ThroneTile, or null if conversion fails</returns>
    public static ThroneTile ConvertToThroneTile(
        GameObject prefab,
        RenderPrefabRegistry prefabRegistry
    )
    {
        if (prefab == null)
        {
            Debug.LogError("[ThroneTileConverter] Prefab is null");
            return null;
        }

        var throneTileAuth = prefab.GetComponent<ThroneTileAuthBase>();
        if (throneTileAuth == null)
        {
            Debug.LogError($"[ThroneTileConverter] No ThroneTileAuthBase found on {prefab.name}");
            return null;
        }

        var throneTile = new ThroneTile();

        // Convert the hierarchy
        throneTile.TileRoot = TileConverter.ConvertHierarchy(prefab, prefabRegistry);

        // Build the marble template if a marble prefab is assigned
        throneTile.PlayerMarbleTemplate = TileConverter.BuildPlayerMarbleTemplate(
            throneTileAuth,
            prefabRegistry
        );

        return throneTile;
    }
}
