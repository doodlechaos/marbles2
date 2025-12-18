using GameCoreLib;
using UnityEngine;

/// <summary>
/// Root component for pooled render prefabs.
/// Place this on the root of every prefab that RenderObjPools manages.
/// It orchestrates the binding lifecycle for all IGCBinding components on the prefab.
/// </summary>
public class RenderPrefabRoot : MonoBehaviour
{
    [Tooltip(
        "The ID of this prefab in the RenderPrefabRegistry (0-based index). Set automatically by the registry editor."
    )]
    [SerializeField, GreyOut]
    private int _prefabId = -1;

    /// <summary>
    /// The render prefab ID for this prefab.
    /// </summary>
    public int PrefabId => _prefabId;

    private IGCBinding[] _bindings;

    public GameCoreObj GameCoreObj;

    private void Awake()
    {
        // Cache all bindings (including inactive children) to avoid GetComponent at runtime
        _bindings = GetComponentsInChildren<IGCBinding>(true);
    }

    /// <summary>
    /// Called by RenderObjPools when acquiring an instance from the pool.
    /// </summary>
    public void OnAcquire(GameCoreObj gameCoreObj)
    {
        gameObject.SetActive(true);

        GameCoreObj = gameCoreObj;

        foreach (var binding in _bindings)
        {
            binding.Bind(gameCoreObj);
        }
    }

    /// <summary>
    /// Called by RenderObjPools when releasing an instance back to the pool.
    /// </summary>
    public void OnRelease()
    {
        foreach (var binding in _bindings)
            binding.Unbind();

        GameCoreObj = null;

        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Set the prefab ID (editor only, used by RenderPrefabRegistryEditor).
    /// </summary>
    public void SetPrefabId(int id)
    {
        _prefabId = id;
    }
#endif
}
