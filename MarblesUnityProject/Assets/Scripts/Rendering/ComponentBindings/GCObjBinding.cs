/* using GameCoreLib;
using UnityEngine;

/// <summary>
/// Component that binds a Unity GameObject to a GameCoreObj by its stable RuntimeId.
/// This allows the renderer to survive serialization/deserialization and hot-reloads
/// without losing track of which GameObject corresponds to which RuntimeObj.
/// </summary>
public sealed class GCObjBinding : MonoBehaviour
{
    /// <summary>
    /// The stable ID of the RuntimeObj this GameObject represents.
    /// This ID persists through save/load cycles.
    /// </summary>
    public GameCoreObj GameCoreObj;
}
 */