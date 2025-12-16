using GameCoreLib;
using UnityEngine;

/// <summary>
/// Marks a prefab as the player marble template.
/// This component is typically added at runtime when spawning players,
/// but can also be placed on a prefab to define the player marble template.
///
/// The AccountId and BidAmount are set at runtime when spawning.
/// </summary>
public class MarbleAuth : GameComponentAuth<MarbleComponent>, IComponentReferenceAuthoring
{
    [Tooltip(
        "Reference to the Rigidbody2D component on this marble (can be on a child object). "
            + "Used for physics-aware positioning and teleportation."
    )]
    public Rigidbody2D RB2D;

    [Tooltip("For testing in editor - the account ID this marble belongs to")]
    public ulong testAccountId = 0;

    [Tooltip("For testing in editor - the bid amount")]
    public uint testBidAmount = 0;

    protected override MarbleComponent CreateComponent()
    {
        return new MarbleComponent { AccountId = testAccountId, BidAmount = testBidAmount };
    }

    public void ResolveReferences(GCComponent component, ComponentExportContext context)
    {
        if (component is not MarbleComponent playerComponent)
            return;

        if (RB2D != null && context.TryGetComponentId(RB2D, out var componentId))
        {
            playerComponent.RigidbodyComponentId = componentId;
        }
        else
        {
            Debug.LogWarning(
                "[PlayerMarbleAuth] Unable to resolve Rigidbody2D reference. Falling back to hierarchy search at runtime."
            );
        }
    }
}
