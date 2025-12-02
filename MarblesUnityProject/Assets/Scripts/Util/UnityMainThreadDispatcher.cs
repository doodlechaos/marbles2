using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dispatches actions to Unity's main thread from background threads.
/// Required for the Editor OAuth loopback server which runs on a background thread.
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly object _lock = new object();
    private readonly Queue<Action> _executionQueue = new Queue<Action>();

    /// <summary>
    /// Gets the singleton instance, creating it if necessary.
    /// </summary>
    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var go = new GameObject("[MainThreadDispatcher]");
                        _instance = go.AddComponent<UnityMainThreadDispatcher>();
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Ensures the dispatcher exists. Call this from the main thread during initialization.
    /// </summary>
    public static void EnsureExists()
    {
        var _ = Instance;
    }

    /// <summary>
    /// Enqueues an action to be executed on the main thread.
    /// Thread-safe and can be called from any thread.
    /// </summary>
    public void Enqueue(Action action)
    {
        if (action == null)
            return;

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                try
                {
                    _executionQueue.Dequeue()?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[MainThreadDispatcher] Error executing action: {ex}");
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
