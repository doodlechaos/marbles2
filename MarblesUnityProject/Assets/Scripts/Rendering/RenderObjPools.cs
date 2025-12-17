using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RenderObjPools : MonoBehaviour
{
    [SerializeField]
    private RenderPrefabRegistry _renderPrefabRegistry;
    private readonly Dictionary<int, ObjectPool<GCObjBinding>> _pools = new();

    public GCObjBinding Get(int prefabId)
    {
        return null;
    }

    public void Release(GCObjBinding obj) { }
}
