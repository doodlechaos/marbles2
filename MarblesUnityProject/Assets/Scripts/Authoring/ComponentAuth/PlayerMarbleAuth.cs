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
    [Tooltip("For testing in editor - the account ID this marble belongs to")]
    public ulong testAccountId = 0;

    [Tooltip("For testing in editor - the bid amount")]
    public uint testBidAmount = 0;

    protected override PlayerMarbleComponent CreateComponent()
    {
        return new PlayerMarbleComponent
        {
            AccountId = testAccountId,
            BidAmount = testBidAmount,
            IsAlive = true,
        };
    }
}
