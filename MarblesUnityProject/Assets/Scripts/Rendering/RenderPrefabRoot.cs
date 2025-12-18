using GameCoreLib;
using UnityEngine;

/// <summary>
/// Root component for pooled render prefabs.
/// Place this on the root of every prefab that RenderObjPools manages.
/// Handles pool lifecycle (acquire/release) and unbinds all IGCBinding components on release.
///
/// Note: Actual binding of GameCoreObjs to GameObjects is handled by TileBinding,
/// which ensures each GameObject gets bound to the correct GameCoreObj.
/// </summary>
public class RenderPrefabRoot : MonoBehaviour
{
    [Tooltip(
        "The ID of this prefab in the RenderPrefabRegistry (0-based index). Set automatically by the registry editor."
    )]
    [SerializeField, GreyOut]
    private short _prefabId = -1;

    /// <summary>
    /// The render prefab ID for this prefab.
    /// </summary>
    public short PrefabId => _prefabId;

    /// <summary>
    /// Called by RenderObjPools when acquiring an instance from the pool.
    /// </summary>
    public void OnAcquire()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Called by RenderObjPools when releasing an instance back to the pool.
    /// Unbinds all IGCBinding components to clear GameCoreObj references.
    /// </summary>
    public void OnRelease()
    {
        foreach (var binding in GetComponentsInChildren<IGCBinding>(true))
        {
            binding.Unbind();
        }

        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Set the prefab ID (editor only, used by RenderPrefabRegistryEditor).
    /// </summary>
    public void SetPrefabId(short id)
    {
        _prefabId = id;
    }
#endif
}
