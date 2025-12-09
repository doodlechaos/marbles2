using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Generic Unity-side converter that turns a GameObject hierarchy into RuntimeObj data.
/// Used by authoring tools (e.g. GameTileConverter) to serialize authored content
/// into GameCore runtime structures.
/// </summary>
public static class GameObjectToGCObj
{
    /// <summary>
    /// Entry point helper: convert a GameObject and its children into a RuntimeObj tree.
    /// </summary>
    public static GameCoreObj Convert(GameObject go, RenderPrefabRegistry prefabRegistry)
    {
        return SerializeGameObject(go, prefabRegistry);
    }

    /// <summary>
    /// Recursively serialize a GameObject and all its children into RuntimeObj.
    /// </summary>
    public static GameCoreObj SerializeGameObject(
        GameObject go,
        RenderPrefabRegistry prefabRegistry
    )
    {
        var context = new ComponentExportContext();
        var result = SerializeGameObject(go, prefabRegistry, context);
        context.ResolveDeferredReferences();
        return result;
    }

    private static GameCoreObj SerializeGameObject(
        GameObject go,
        RenderPrefabRegistry prefabRegistry,
        ComponentExportContext context
    )
    {
        if (prefabRegistry == null)
        {
            Debug.LogError("[GameObjectToRuntimeObjConverter] Prefab registry is null");
        }

        GameCoreObj runtimeObj = new GameCoreObj
        {
            Name = go.name,
            Children = new List<GameCoreObj>(),
            Transform = ConvertToFPTransform(go.transform),
            GameComponents = SerializeGameComponents(go, context),
            RenderPrefabID = prefabRegistry != null ? prefabRegistry.GetPrefabID(go) : 0,
        };

        // Add LevelRootComponent for objects with TileAuthBase
        TileAuthBase tileAuth = go.GetComponent<TileAuthBase>();
        if (tileAuth != null)
        {
            var levelRoot = new TileRootComponent
            {
                GameModeType = tileAuth.GetType().Name.Replace("Auth", ""),
            };
            context.RegisterComponent(null, levelRoot);
            runtimeObj.GameComponents.Add(levelRoot);
        }

        // Recursively serialize all children
        foreach (Transform child in go.transform)
        {
            GameCoreObj childObj = SerializeGameObject(child.gameObject, prefabRegistry, context);
            runtimeObj.Children.Add(childObj);
        }

        return runtimeObj;
    }

    /// <summary>
    /// Serialize all GameComponentAuth components on a GameObject to GameComponents.
    /// Also auto-exports Unity physics components if no explicit auth component exists.
    /// </summary>
    public static List<GCComponent> SerializeGameComponents(
        GameObject go,
        ComponentExportContext context
    )
    {
        List<GCComponent> components = new List<GCComponent>();

        // First, export all explicit GameComponentAuth components
        GameComponentAuth[] authComponents = go.GetComponents<GameComponentAuth>();
        foreach (var auth in authComponents)
        {
            try
            {
                GCComponent gameComponent = auth.ToGameComponent();
                if (gameComponent == null)
                    continue;

                context.RegisterComponent(auth, gameComponent);
                if (auth is IComponentReferenceAuthoring referenceAuthoring)
                {
                    context.RegisterDeferredResolver(referenceAuthoring, gameComponent);
                }

                components.Add(gameComponent);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(
                    $"[GameObjectToRuntimeObjConverter] Failed to export {auth.GetType().Name} on {go.name}: {e.Message}"
                );
            }
        }

        // Auto-export Unity physics components if no explicit auth exists
        AutoExportUnityPhysicsComponents(go, components, context);

        return components;
    }

    /// <summary>
    /// Automatically export Unity physics components that don't have explicit auth components.
    /// </summary>
    public static void AutoExportUnityPhysicsComponents(
        GameObject go,
        List<GCComponent> components,
        ComponentExportContext context
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
                // Read physics material properties (friction, bounciness) from Unity's PhysicsMaterial2D
                GetPhysicsMaterialProperties(
                    boxCollider.sharedMaterial,
                    out float friction,
                    out float restitution
                );

                var component = context.RegisterComponent(
                    boxCollider,
                    new BoxCollider2DComponent
                    {
                        Enabled = boxCollider.enabled,
                        Size = FPVector2.FromFloats(boxCollider.size.x, boxCollider.size.y),
                        Offset = FPVector2.FromFloats(boxCollider.offset.x, boxCollider.offset.y),
                        IsTrigger = boxCollider.isTrigger,
                        Friction = FP.FromFloat(friction),
                        Restitution = FP.FromFloat(restitution),
                    }
                );
                components.Add(component);
            }
        }

        // Auto-export CircleCollider2D
        if (!hasCircleCollider)
        {
            var circleCollider = go.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                // Read physics material properties (friction, bounciness) from Unity's PhysicsMaterial2D
                GetPhysicsMaterialProperties(
                    circleCollider.sharedMaterial,
                    out float friction,
                    out float restitution
                );

                var component = context.RegisterComponent(
                    circleCollider,
                    new CircleCollider2DComponent
                    {
                        Enabled = circleCollider.enabled,
                        Radius = FP.FromFloat(circleCollider.radius),
                        Offset = FPVector2.FromFloats(
                            circleCollider.offset.x,
                            circleCollider.offset.y
                        ),
                        IsTrigger = circleCollider.isTrigger,
                        Friction = FP.FromFloat(friction),
                        Restitution = FP.FromFloat(restitution),
                    }
                );
                components.Add(component);
            }
        }

        // Auto-export Rigidbody2D
        if (!hasRigidbody)
        {
            var rigidbody = go.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                var component = context.RegisterComponent(
                    rigidbody,
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
                components.Add(component);
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

    /// <summary>
    /// Extracts friction and restitution from a Unity PhysicsMaterial2D.
    /// Uses sensible defaults when no material is assigned.
    /// </summary>
    private static void GetPhysicsMaterialProperties(
        PhysicsMaterial2D material,
        out float friction,
        out float restitution
    )
    {
        if (material != null)
        {
            friction = material.friction;
            restitution = material.bounciness;
        }
        else
        {
            // Sensible defaults when no material is assigned
            friction = 0.5f;
            restitution = 0f;
        }
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
