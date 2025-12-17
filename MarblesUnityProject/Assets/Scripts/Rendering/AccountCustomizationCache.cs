using System.Collections.Generic;
using SpacetimeDB.Types;
using UnityEngine;

public class AccountVisual
{
    public ulong AccountId;
    public string Username;
    public Sprite PfpSprite;
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

    [SerializeField]
    private Sprite _defaultPfpSprite;

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

    private void OnAccountCustomizationInsert(
        EventContext ctx,
        AccountCustomization accountCustomization
    )
    {
        OnCustomizationChanged(accountCustomization);
    }

    private void OnAccountCustomizationUpdate(
        EventContext ctx,
        AccountCustomization oldAccountCustomization,
        AccountCustomization newAccountCustomization
    )
    {
        OnCustomizationChanged(newAccountCustomization);
    }

    private void OnAccountCustomizationDelete(
        EventContext ctx,
        AccountCustomization accountCustomization
    )
    {
        OnCustomizationDeleted(accountCustomization);
    }

    private void OnCustomizationChanged(AccountCustomization accountCustomization)
    {
        Debug.Log($"OnCustomizationChanged: {accountCustomization}");
        AccountVisual visual = EnsureVisualExists(accountCustomization.AccountId);
        visual.Username = accountCustomization.Username;
        NotifyConsumers(accountCustomization.AccountId);
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

    private void OnCustomizationDeleted(AccountCustomization accountCustomization)
    {
        Debug.Log($"OnCustomizationDeleted: {accountCustomization}");
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

    private AccountVisual EnsureVisualExists(ulong accountId)
    {
        if (_visualsCache.TryGetValue(accountId, out var visual))
            return visual;

        visual = new AccountVisual
        {
            AccountId = accountId,
            Username = "",
            PfpSprite = _defaultPfpSprite,
        };
        _visualsCache[accountId] = visual;
        return visual;
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

    public void Cleanup(DbConnection conn)
    {
        conn.Db.ActiveAccountCustomsView.OnInsert -= OnAccountCustomizationInsert;
        conn.Db.ActiveAccountCustomsView.OnUpdate -= OnAccountCustomizationUpdate;
        conn.Db.ActiveAccountCustomsView.OnDelete -= OnAccountCustomizationDelete;
    }
}
