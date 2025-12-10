using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var existing))
            return existing;

        return go.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Component c) where T : Component
    {
        if (c.TryGetComponent<T>(out var existing))
            return existing;

        return c.gameObject.AddComponent<T>();
    }
}
