using GameCoreLib;
using UnityEngine;

/// <summary>
/// Marks a prefab as the player marble template.
/// This component is typically added at runtime when spawning players,
/// but can also be placed on a prefab to define the player marble template.
///
/// The AccountId and BidAmount are set at runtime when spawning.
/// </summary>
public class PlayerMarbleAuth : GameComponentAuth<PlayerMarbleComponent>
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

    protected override PlayerMarbleComponent CreateComponent()
    {
        // Get the name of the GameObject that has the rigidbody
        // This allows us to find the corresponding RuntimeObj after serialization
        string rigidbodyChildName = "";
        if (RB2D != null)
        {
            rigidbodyChildName = RB2D.gameObject.name;
        }

        return new PlayerMarbleComponent
        {
            AccountId = testAccountId,
            BidAmount = testBidAmount,
            IsAlive = true,
            RigidbodyChildName = rigidbodyChildName,
        };
    }
}
