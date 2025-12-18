using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Object pool manager for render prefabs.
/// Handles spawning, pooling, and lifecycle of visual representations.
/// </summary>
public class RenderObjPools : MonoBehaviour
{
    [SerializeField]
    private RenderPrefabRegistry _registry;

    private readonly Dictionary<int, ObjectPool<RenderPrefabRoot>> _pools = new();

    /// <summary>
    /// Spawns a visual instance from the pool and binds it to the data.
    /// </summary>
    public RenderPrefabRoot Spawn(int prefabId, Transform parent, GameCoreObj data)
    {
        if (!_pools.TryGetValue(prefabId, out var pool))
        {
            pool = CreatePool(prefabId);
            if (pool == null)
                return null;

            _pools[prefabId] = pool;
        }

        RenderPrefabRoot instance = pool.Get();
        instance.transform.SetParent(parent, false);
        instance.OnAcquire(data);

        return instance;
    }

    /// <summary>
    /// Despawns an instance and returns it to the pool.
    /// </summary>
    public void Despawn(RenderPrefabRoot instance)
    {
        if (instance == null)
            return;

        instance.OnRelease();

        if (_pools.TryGetValue(instance.PrefabId, out var pool))
        {
            pool.Release(instance);
            instance.transform.SetParent(transform);
        }
        else
        {
            Destroy(instance.gameObject);
        }
    }

    private ObjectPool<RenderPrefabRoot> CreatePool(int prefabId)
    {
        RenderPrefabRoot prefab = _registry.GetPrefabByID(prefabId);
        if (prefab == null)
        {
            Debug.LogWarning($"[RenderObjPools] No prefab found for ID {prefabId}");
            return null;
        }

        return new ObjectPool<RenderPrefabRoot>(
            createFunc: () =>
            {
                RenderPrefabRoot instance = Instantiate(prefab);
                instance.gameObject.SetActive(false);
                return instance;
            },
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: 10,
            maxSize: 1000
        );
    }
}
