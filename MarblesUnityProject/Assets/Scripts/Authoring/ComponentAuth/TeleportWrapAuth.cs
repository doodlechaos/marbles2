using FPMathLib;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Authors a trigger zone that teleports dynamic rigidbodies by an offset when they enter.
/// The BoxCollider2D on this GameObject should be set to IsTrigger=true.
/// The offset is applied instantly without changing velocity or rotation.
/// Useful for level wrapping (e.g., falling off bottom teleports to top).
/// </summary>
public class TeleportWrapAuth : GameComponentAuth<TeleportWrapComponent>
{
    [Tooltip("Position offset to apply when a rigidbody enters the trigger")]
    public Vector2 offset = Vector2.zero;

    protected override TeleportWrapComponent CreateComponent()
    {
        return new TeleportWrapComponent { Offset = offset.ToFPVector2() };
    }
}
