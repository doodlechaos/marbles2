using System;
using System.Collections;
using System.Collections.Generic;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Networking;

public class AccountVisual
{
    public ulong AccountId;
    public string Username;
    public Sprite PfpSprite;

    // Track versions so we only load when needed.
    public int LoadedPfpVersion = -1; // what we currently have in PfpSprite
    public int RequestedPfpVersion = -1; // latest version we want (may be in-flight)
}

public interface IAccountCustomizationConsumer
{
    void ApplyAccountCustomization(AccountVisual visual);
}

public class AccountCustomizationCache : MonoBehaviour
{
    public static AccountCustomizationCache Inst { get; private set; }

    private readonly Dictionary<ulong, AccountVisual> _visualsCache = new();
    private readonly Dictionary<ulong, List<IAccountCustomizationConsumer>> _consumers = new();

    [Header("PFP")]
    [SerializeField]
    private Sprite _defaultPfpSprite;

    [SerializeField]
    private string _pfpBaseUrl = "https://cdn-dev.marbles.live/pfp";

    [SerializeField]
    private int _maxConcurrentPfpLoads = 8;

    [SerializeField]
    private int _pfpTimeoutSeconds = 10;

    [SerializeField]
    private int _pfpRetries = 2;

    private readonly Queue<(ulong accountId, int version)> _pfpQueue = new();
    private readonly HashSet<(ulong accountId, int version)> _pfpQueuedOrInFlight = new();
    private int _pfpActive;
    private Coroutine _pfpPump;

    public Sprite testSprite;
    void Awake()
    {
        Inst = this;
    }

    public void SetCallbacks(DbConnection conn)
    {
        conn.Db.ActiveAccountCustomsView.OnInsert += OnAccountCustomizationInsert;
        conn.Db.ActiveAccountCustomsView.OnUpdate += OnAccountCustomizationUpdate;
        conn.Db.ActiveAccountCustomsView.OnDelete += OnAccountCustomizationDelete;
    }

    private void OnAccountCustomizationInsert(EventContext ctx, AccountCustomization row) =>
        OnCustomizationChanged(row);

    private void OnAccountCustomizationUpdate(
        EventContext ctx,
        AccountCustomization oldRow,
        AccountCustomization newRow
    ) => OnCustomizationChanged(newRow);

    private void OnAccountCustomizationDelete(EventContext ctx, AccountCustomization row)
    {
        Debug.Log($"OnCustomizationDeleted: {row}");
        // Optional: if you want to free memory when no consumers exist:
        // TryEvictIfUnused(row.AccountId);
    }

    private void OnCustomizationChanged(AccountCustomization row)
    {
        var visual = EnsureVisualExists(row.AccountId);

        visual.Username = row.Username;

        // Apply immediately (username updates, old pfp stays until new arrives).
        NotifyConsumers(row.AccountId);

        // Now ensure pfp is up-to-date (async).
        EnsurePfpUpToDate(row.AccountId, row.PfpVersion);
    }

    private void EnsurePfpUpToDate(ulong accountId, byte newVersionByte)
    {
        int newVersion = newVersionByte;

        var visual = EnsureVisualExists(accountId);

        // If already loaded, nothing to do.
        if (visual.LoadedPfpVersion == newVersion)
            return;

        // Record desired version.
        visual.RequestedPfpVersion = newVersion;

        EnqueuePfpLoad(accountId, newVersion);
    }

    private void EnqueuePfpLoad(ulong accountId, int version)
    {
        var key = (accountId, version);
        if (_pfpQueuedOrInFlight.Contains(key))
            return;

        _pfpQueuedOrInFlight.Add(key);
        _pfpQueue.Enqueue(key);

        if (_pfpPump == null)
            _pfpPump = StartCoroutine(PfpPump());
    }

    private IEnumerator PfpPump()
    {
        while (_pfpQueue.Count > 0 || _pfpActive > 0)
        {
            while (_pfpActive < _maxConcurrentPfpLoads && _pfpQueue.Count > 0)
            {
                var job = _pfpQueue.Dequeue();
                _pfpActive++;
                StartCoroutine(LoadPfpJob(job.accountId, job.version));
            }
            yield return null;
        }

        _pfpPump = null;
    }

    private IEnumerator LoadPfpJob(ulong accountId, int version)
    {
        try
        {
            // If a newer version got requested before we even started, skip.
            if (!_visualsCache.TryGetValue(accountId, out var visual))
                yield break;

            if (visual.RequestedPfpVersion != version)
                yield break;

            string url = BuildPfpUrl(accountId, version);

            Texture2D tex = null;
            bool success = false;

            for (int attempt = 0; attempt <= _pfpRetries; attempt++)
            {
                using var req = UnityWebRequestTexture.GetTexture(url, nonReadable: true);
                req.timeout = _pfpTimeoutSeconds;

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    tex = DownloadHandlerTexture.GetContent(req);
                    success = tex != null;
                    break;
                }

                if (attempt < _pfpRetries)
                    yield return new WaitForSeconds(0.25f * (attempt + 1));
            }

            // Late/outdated result guard.
            if (!_visualsCache.TryGetValue(accountId, out visual))
                yield break;

            if (visual.RequestedPfpVersion != version)
            {
                if (tex != null)
                    Destroy(tex);
                yield break;
            }

            if (!success)
            {
                // Keep current sprite (often default) and just notify (optional).
                yield break;
            }

            // Replace old sprite/texture (avoid leaking textures).
            ReplaceSprite(ref visual.PfpSprite, tex);

            visual.LoadedPfpVersion = version;

            NotifyConsumers(accountId);
        }
        finally
        {
            _pfpActive--;
            _pfpQueuedOrInFlight.Remove((accountId, version));
        }
    }

    private string BuildPfpUrl(ulong accountId, int version)
    {
        // Cache-bust via query param so CDN/client caches don’t serve the old image.
        // Example: https://cdn-dev.marbles.live/pfp/1.png?v=3
        return $"{_pfpBaseUrl}/{accountId}.png?v={version}";
    }

    private void ReplaceSprite(ref Sprite existing, Texture2D tex)
    {
        if (existing != null && existing != _defaultPfpSprite)
        {
            var oldTex = existing.texture;
            Destroy(existing);
            if (oldTex != null)
                Destroy(oldTex);
        }

        var rect = new Rect(0, 0, tex.width, tex.height);
        existing = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 100f);
        testSprite = existing;
    }

    private void NotifyConsumers(ulong accountId)
    {
        if (!_visualsCache.TryGetValue(accountId, out var visual))
            return;
        if (!_consumers.TryGetValue(accountId, out var list))
            return;

        foreach (var consumer in list)
            consumer.ApplyAccountCustomization(visual);
    }

    public void RegisterConsumer(ulong accountId, IAccountCustomizationConsumer consumer)
    {
        if (!_consumers.TryGetValue(accountId, out var list))
        {
            list = new List<IAccountCustomizationConsumer>();
            _consumers[accountId] = list;
        }

        if (!list.Contains(consumer))
            list.Add(consumer);

        var visual = EnsureVisualExists(accountId);
        consumer.ApplyAccountCustomization(visual);
    }

    public void UnregisterConsumer(ulong accountId, IAccountCustomizationConsumer consumer)
    {
        if (_consumers.TryGetValue(accountId, out var list))
        {
            list.Remove(consumer);
            if (list.Count <= 0)
                _consumers.Remove(accountId);
        }
    }

    private AccountVisual EnsureVisualExists(ulong accountId)
    {
        if (_visualsCache.TryGetValue(accountId, out var visual))
            return visual;

        visual = new AccountVisual
        {
            AccountId = accountId,
            Username = "",
            PfpSprite = _defaultPfpSprite,
            LoadedPfpVersion = -1,
            RequestedPfpVersion = -1,
        };
        _visualsCache[accountId] = visual;
        return visual;
    }

    public void Cleanup(DbConnection conn)
    {
        conn.Db.ActiveAccountCustomsView.OnInsert -= OnAccountCustomizationInsert;
        conn.Db.ActiveAccountCustomsView.OnUpdate -= OnAccountCustomizationUpdate;
        conn.Db.ActiveAccountCustomsView.OnDelete -= OnAccountCustomizationDelete;
    }
}
