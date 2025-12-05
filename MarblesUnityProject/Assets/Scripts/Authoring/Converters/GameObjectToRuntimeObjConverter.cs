using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Generic Unity-side converter that turns a GameObject hierarchy into RuntimeObj data.
/// Used by authoring tools (e.g. GameTileConverter) to serialize authored content
/// into GameCore runtime structures.
/// </summary>
public static class GameObjectToRuntimeObjConverter
{
    /// <summary>
    /// Entry point helper: convert a GameObject and its children into a RuntimeObj tree.
    /// </summary>
    public static RuntimeObj Convert(GameObject go, RenderPrefabRegistry prefabRegistry)
    {
        return SerializeGameObject(go, prefabRegistry);
    }

    /// <summary>
    /// Recursively serialize a GameObject and all its children into RuntimeObj.
    /// </summary>
    public static RuntimeObj SerializeGameObject(GameObject go, RenderPrefabRegistry prefabRegistry)
    {
        if (prefabRegistry == null)
        {
            Debug.LogError("[GameObjectToRuntimeObjConverter] Prefab registry is null");
        }

        RuntimeObj runtimeObj = new RuntimeObj
        {
            Name = go.name,
            Children = new List<RuntimeObj>(),
            Transform = ConvertToFPTransform(go.transform),
            GameComponents = SerializeGameComponents(go),
            RenderPrefabID = prefabRegistry != null ? prefabRegistry.GetPrefabID(go) : 0,
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
    public static List<RuntimeObjComponent> SerializeGameComponents(GameObject go)
    {
        List<RuntimeObjComponent> components = new List<RuntimeObjComponent>();

        // First, export all explicit GameComponentAuth components
        GameComponentAuth[] authComponents = go.GetComponents<GameComponentAuth>();
        foreach (var auth in authComponents)
        {
            try
            {
                RuntimeObjComponent gameComponent = auth.ToGameComponent();
                if (gameComponent != null)
                {
                    components.Add(gameComponent);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(
                    $"[GameObjectToRuntimeObjConverter] Failed to export {auth.GetType().Name} on {go.name}: {e.Message}"
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
        List<RuntimeObjComponent> components
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
