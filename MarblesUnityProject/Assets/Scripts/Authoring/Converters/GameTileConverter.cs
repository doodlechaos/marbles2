using GameCoreLib;
using UnityEngine;

/// <summary>
/// Converts Unity GameObjects with GameTileAuth components into GameTileBase data.
/// Thin wrapper around the generic GameObjectToRuntimeObjConverter for tile-specific concerns.
/// Shared between GameTileExporter (Editor) and GameTilePlayground (Runtime).
/// </summary>
public static class GameTileConverter
{
    /// <summary>
    /// Convert a GameObject with a GameTileAuthBase into a fully populated GameTileBase.
    /// </summary>
    /// <param name="prefab">The GameObject with a GameTileAuthBase component</param>
    /// <param name="prefabRegistry">The RenderPrefabRegistry for looking up RenderPrefabIDs via RenderPrefabIdentifier components</param>
    /// <returns>A populated GameTileBase, or null if conversion fails</returns>
    public static GameTileBase ConvertToGameTile(
        GameObject prefab,
        RenderPrefabRegistry prefabRegistry
    )
    {
        if (prefab == null)
        {
            Debug.LogError("[GameTileConverter] Prefab is null");
            return null;
        }

        var gameTileAuth = prefab.GetComponent<GameTileAuthBase>();
        if (gameTileAuth == null)
        {
            Debug.LogError($"[GameTileConverter] No GameTileAuthBase found on {prefab.name}");
            return null;
        }

        GameTileBase gameTile = CreateGameTileFromAuth(gameTileAuth, prefabRegistry);
        if (gameTile == null)
        {
            Debug.LogError(
                $"[GameTileConverter] Unknown game tile auth type: {gameTileAuth.GetType().Name}"
            );
            return null;
        }

        gameTile.TileRoot = GameObjectToRuntimeObjConverter.SerializeGameObject(
            prefab,
            prefabRegistry
        );
        return gameTile;
    }

    /// <summary>
    /// Create the appropriate GameTileBase subtype based on the auth component type.
    /// Also builds the shared PlayerMarbleTemplate from GameTileAuthBase.PlayerMarblePrefab, if assigned.
    /// </summary>
    public static GameTileBase CreateGameTileFromAuth(
        GameTileAuthBase gameTileAuth,
        RenderPrefabRegistry prefabRegistry
    )
    {
        GameTileBase gameTile = gameTileAuth switch
        {
            SimpleBattleRoyaleAuth => new SimpleBattleRoyale(),
            // Add other game modes here as they're created:
            // SurvivalAuth => new Survival(),
            _ => null,
        };

        if (gameTile == null)
        {
            Debug.LogError(
                $"[GameTileConverter] Unknown game tile auth type: {gameTileAuth.GetType().Name}"
            );
            return null;
        }

        // Build the shared PlayerMarbleTemplate for all tile types that support spawning players.
        if (gameTileAuth.PlayerMarblePrefab == null)
        {
            Debug.LogError(
                $"[GameTileConverter] {gameTileAuth.GetType().Name} requires PlayerMarblePrefab to be assigned."
            );
        }
        else if (prefabRegistry == null)
        {
            Debug.LogError(
                "[GameTileConverter] PrefabRegistry is null – cannot serialize PlayerMarblePrefab."
            );
        }
        else
        {
            gameTile.PlayerMarbleTemplate = GameObjectToRuntimeObjConverter.SerializeGameObject(
                gameTileAuth.PlayerMarblePrefab.gameObject,
                prefabRegistry
            );
        }

        return gameTile;
    }
}
