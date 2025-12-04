using System;
using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Converts Unity GameObjects with GameTileAuth components into GameTileBase data.
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

        GameTileBase gameTile = CreateGameTileFromAuth(gameTileAuth);
        if (gameTile == null)
        {
            Debug.LogError(
                $"[GameTileConverter] Unknown game tile auth type: {gameTileAuth.GetType().Name}"
            );
            return null;
        }

        gameTile.TileRoot = SerializeGameObject(prefab, prefabRegistry);
        return gameTile;
    }

    /// <summary>
    /// Create the appropriate GameTileBase subtype based on the auth component type.
    /// </summary>
    public static GameTileBase CreateGameTileFromAuth(GameTileAuthBase gameTileAuth)
    {
        return gameTileAuth switch
        {
            SimpleBattleRoyaleAuth => new SimpleBattleRoyale(),
            // Add other game modes here as they're created:
            // SurvivalAuth => new Survival(),
            // RaceAuth => new Race(),
            _ => null,
        };
    }

    /// <summary>
    /// Recursively serialize a GameObject and all its children into RuntimeObj.
    /// </summary>
    public static RuntimeObj SerializeGameObject(GameObject go, RenderPrefabRegistry prefabRegistry)
    {
        if (prefabRegistry == null)
        {
            Debug.LogError($"[GameTileConverter] Prefab registry is null");
        }
        RuntimeObj runtimeObj = new RuntimeObj
        {
            Name = go.name,
            Children = new List<RuntimeObj>(),
            Transform = ConvertToFPTransform(go.transform),
            GameComponents = SerializeGameComponents(go),
            RenderPrefabID = prefabRegistry.GetPrefabID(go),
        };

        // Add LevelRootComponent for objects with GameTileAuth
        GameTileAuthBase gameTileAuth = go.GetComponent<GameTileAuthBase>();
        if (gameTileAuth != null)
        {
            runtimeObj.GameComponents.Add(
                new LevelRootComponent
                {
                    GameModeType = gameTileAuth.GetType().Name.Replace("Auth", ""),
                }
            );
        }

        // Recursively serialize all children
        foreach (Transform child in go.transform)
        {
            RuntimeObj childObj = SerializeGameObject(child.gameObject, prefabRegistry);
            runtimeObj.Children.Add(childObj);
        }

        return runtimeObj;
    }

    /// <summary>
    /// Serialize all GameComponentAuth components on a GameObject to GameComponents.
    /// Also auto-exports Unity physics components if no explicit auth component exists.
    /// </summary>
    public static List<GameComponent> SerializeGameComponents(GameObject go)
    {
        List<GameComponent> components = new List<GameComponent>();

        // First, export all explicit GameComponentAuth components
        GameComponentAuth[] authComponents = go.GetComponents<GameComponentAuth>();
        foreach (var auth in authComponents)
        {
            try
            {
                GameComponent gameComponent = auth.ToGameComponent();
                if (gameComponent != null)
                {
                    components.Add(gameComponent);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    $"[GameTileConverter] Failed to export {auth.GetType().Name} on {go.name}: {e.Message}"
                );
            }
        }

        // Auto-export Unity physics components if no explicit auth exists
        AutoExportUnityPhysicsComponents(go, components);

        return components;
    }

    /// <summary>
    /// Automatically export Unity physics components that don't have explicit auth components.
    /// </summary>
    public static void AutoExportUnityPhysicsComponents(
        GameObject go,
        List<GameComponent> components
    )
    {
        // Check if we already have these component types from auth
        bool hasBoxCollider = false;
        bool hasCircleCollider = false;
        bool hasRigidbody = false;

        foreach (var comp in components)
        {
            if (comp is BoxCollider2DComponent)
                hasBoxCollider = true;
            if (comp is CircleCollider2DComponent)
                hasCircleCollider = true;
            if (comp is Rigidbody2DComponent)
                hasRigidbody = true;
        }

        // Auto-export BoxCollider2D
        if (!hasBoxCollider)
        {
            var boxCollider = go.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                components.Add(
                    new BoxCollider2DComponent
                    {
                        Enabled = boxCollider.enabled,
                        Size = FPVector2.FromFloats(boxCollider.size.x, boxCollider.size.y),
                        Offset = FPVector2.FromFloats(boxCollider.offset.x, boxCollider.offset.y),
                        IsTrigger = boxCollider.isTrigger,
                    }
                );
            }
        }

        // Auto-export CircleCollider2D
        if (!hasCircleCollider)
        {
            var circleCollider = go.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                components.Add(
                    new CircleCollider2DComponent
                    {
                        Enabled = circleCollider.enabled,
                        Radius = FP.FromFloat(circleCollider.radius),
                        Offset = FPVector2.FromFloats(
                            circleCollider.offset.x,
                            circleCollider.offset.y
                        ),
                        IsTrigger = circleCollider.isTrigger,
                    }
                );
            }
        }

        // Auto-export Rigidbody2D
        if (!hasRigidbody)
        {
            var rigidbody = go.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                components.Add(
                    new Rigidbody2DComponent
                    {
                        Enabled = rigidbody.simulated,
                        BodyType = ConvertBodyType(rigidbody.bodyType),
                        Mass = FP.FromFloat(rigidbody.mass),
                        LinearDrag = FP.FromFloat(rigidbody.linearDamping),
                        AngularDrag = FP.FromFloat(rigidbody.angularDamping),
                        GravityScale = FP.FromFloat(rigidbody.gravityScale),
                        FreezeRotation = rigidbody.freezeRotation,
                    }
                );
            }
        }
    }

    public static Rigidbody2DType ConvertBodyType(RigidbodyType2D unityType)
    {
        return unityType switch
        {
            RigidbodyType2D.Dynamic => Rigidbody2DType.Dynamic,
            RigidbodyType2D.Kinematic => Rigidbody2DType.Kinematic,
            RigidbodyType2D.Static => Rigidbody2DType.Static,
            _ => Rigidbody2DType.Dynamic,
        };
    }

    public static FPTransform3D ConvertToFPTransform(Transform t)
    {
        FPVector3 localPos = FPVector3.FromFloats(
            t.localPosition.x,
            t.localPosition.y,
            t.localPosition.z
        );

        FPQuaternion localRot = FPQuaternion.FromFloats(
            t.localRotation.x,
            t.localRotation.y,
            t.localRotation.z,
            t.localRotation.w
        );

        FPVector3 localScale = FPVector3.FromFloats(t.localScale.x, t.localScale.y, t.localScale.z);

        return new FPTransform3D(localPos, localRot, localScale);
    }
}
