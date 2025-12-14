using GameCoreLib;
using UnityEngine;

/// <summary>
/// Authors a brick breaker style defense brick.
/// Requires a MarbleDetectorAuth component on the same GameObject to detect marble collisions.
/// When a marble hits the brick, it loses 1 point and the brick loses 1 health.
/// </summary>
public class DefenseBrickAuth : GameComponentAuth<DefenseBrickComponent>
{
    [Tooltip("Starting health of this brick. When it reaches 0, the brick is disabled.")]
    public int health = 1;

    protected override DefenseBrickComponent CreateComponent()
    {
        return new DefenseBrickComponent { Health = health };
    }
}
