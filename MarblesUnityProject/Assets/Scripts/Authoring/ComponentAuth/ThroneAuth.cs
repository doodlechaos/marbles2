using GameCoreLib;
using UnityEngine;

public class ThroneAuth : GameComponentAuth<ThroneComponent>, IComponentReferenceAuthoring
{
    [Tooltip("Reference to the CircleCollider2D on the KingBody that detects marble collisions")]
    public CircleCollider2D KingBodyCollider;

    public void ResolveReferences(GCComponent component, ComponentExportContext context)
    {
        if (component is not ThroneComponent throneComponent)
            return;

        if (
            KingBodyCollider != null
            && context.TryGetComponentId(KingBodyCollider, out var componentId)
        )
        {
            throneComponent.KingBodyColliderComponentId = componentId;
        }
        else
        {
            Debug.LogWarning("[ThroneAuth] Unable to resolve KingBodyCollider reference.");
        }
    }
}
