using System.Collections.Generic;
using System.Linq;
using SpacetimeDB.Types;
using UnityEngine;

public class BidDisplayPanel : MonoBehaviour
{
    [SerializeField]
    private AuctionPlayerEntry _auctionPlayerEntryPrefab;

    [SerializeField]
    private float _spacingBetweenAuctionEntries = 0.5f;

    [SerializeField]
    private Transform _auctionPlayerEntriesParent;

    private Dictionary<ulong, AuctionPlayerEntry> _entriesByAccountId =
        new Dictionary<ulong, AuctionPlayerEntry>();
    private List<AuctionPlayerEntry> _pooledEntries = new List<AuctionPlayerEntry>();

    public void SetCallbacks(DbConnection conn)
    {
        conn.Db.AccountBid.OnInsert += (EventContext ctx, AccountBid accountBid) =>
        {
            RefreshBidDisplay(ctx.Db);
        };
        conn.Db.AccountBid.OnUpdate += (
            EventContext ctx,
            AccountBid oldAccountBid,
            AccountBid newAccountBid
        ) =>
        {
            RefreshBidDisplay(ctx.Db);
        };
        conn.Db.AccountBid.OnDelete += (EventContext ctx, AccountBid accountBid) =>
        {
            RefreshBidDisplay(ctx.Db);
        };
        Debug.Log("Set callbacks for AccountBid table.");
    }

    private void RefreshBidDisplay(RemoteTables db)
    {
        // Get all bids sorted by TotalBid descending (highest bidder = rank 1)
        List<AccountBid> sortedBids = db
            .AccountBid.Iter()
            .OrderByDescending(bid => bid.TotalBid)
            .ToList();

        // Track which accounts are still active
        HashSet<ulong> activeAccountIds = new HashSet<ulong>();

        for (int i = 0; i < sortedBids.Count; i++)
        {
            AccountBid bid = sortedBids[i];
            activeAccountIds.Add(bid.AccountId);

            // Get or create entry for this account
            if (!_entriesByAccountId.TryGetValue(bid.AccountId, out AuctionPlayerEntry entry))
            {
                entry = GetOrCreateEntry();
                string username = GetUsername(db, bid.AccountId);
                entry.Init(bid.AccountId, (ushort)(i + 1), username, bid.TotalBid);
                _entriesByAccountId[bid.AccountId] = entry;
            }
            else
            {
                entry.UpdateDisplay((ushort)(i + 1), bid.TotalBid);
            }

            entry.gameObject.SetActive(true);
            Vector3 targetLocalPos = new Vector3(0, -i * _spacingBetweenAuctionEntries, 0);
            entry.SetTargetLocalPos(targetLocalPos);
        }

        // Hide entries for accounts no longer bidding
        List<ulong> toRemove = new List<ulong>();
        foreach (var kvp in _entriesByAccountId)
        {
            if (!activeAccountIds.Contains(kvp.Key))
            {
                kvp.Value.gameObject.SetActive(false);
                _pooledEntries.Add(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (ulong id in toRemove)
        {
            _entriesByAccountId.Remove(id);
        }
    }

    private AuctionPlayerEntry GetOrCreateEntry()
    {
        if (_pooledEntries.Count > 0)
        {
            AuctionPlayerEntry pooled = _pooledEntries[_pooledEntries.Count - 1];
            _pooledEntries.RemoveAt(_pooledEntries.Count - 1);
            return pooled;
        }
        return Instantiate(_auctionPlayerEntryPrefab, _auctionPlayerEntriesParent);
    }

    private string GetUsername(RemoteTables db, ulong accountId)
    {
        AccountCustomization customization = db.AccountCustomization.AccountId.Find(accountId);
        if (customization != null && !string.IsNullOrEmpty(customization.Username))
        {
            return customization.Username;
        }
        return $"Player {accountId}";
    }

    public void OnBidButtonClicked()
    {
        STDB.Conn.Reducers.PlaceBid(1);
        Debug.Log("Called reducer to place bid.");
    }
}
