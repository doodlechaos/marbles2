using System;
using System.Collections.Generic;
using System.Linq;
using SpacetimeDB.Types;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BidDisplayPanel : MonoBehaviour
{
    [SerializeField]
    private Transform _GameTile1Origin; //TODO: Move the entire bid display panel to the origin of the game tile that we're actively bidding on.

    [SerializeField]
    private Transform _GameTile2Origin;

    [SerializeField]
    private AuctionPlayerEntry _auctionPlayerEntryPrefab;

    [SerializeField]
    private float _spacingBetweenAuctionEntries = 0.5f;

    [SerializeField]
    private Transform _auctionPlayerEntriesParent;

    private Dictionary<ulong, AuctionPlayerEntry> _entriesByAccountId =
        new Dictionary<ulong, AuctionPlayerEntry>();
    private List<AuctionPlayerEntry> _pooledEntries = new List<AuctionPlayerEntry>();

    [SerializeField]
    private TextMeshProUGUI _bidTimeText;

    [SerializeField]
    private TextMeshProUGUI _bidStateText;

    [SerializeField]
    private float _lerpSpeed = 5f;

    private byte _currentBidWorldId = 1;

    public void SetCallbacks(DbConnection conn)
    {
        conn.Db.AccountBid.OnInsert += OnAccountBidInsert;
        conn.Db.AccountBid.OnUpdate += OnAccountBidUpdate;
        conn.Db.AccountBid.OnDelete += OnAccountBidDelete;
        conn.Db.BiddingStateS.OnInsert += OnBiddingStateSInsert;
        conn.Db.BiddingStateS.OnUpdate += OnBiddingStateSUpdate;
        conn.Db.BidTimeS.OnUpdate += OnBidTimeSUpdate;
        Debug.Log("[BidDisplayPanel] Callbacks registered");
    }

    public void CleanupCallbacks(DbConnection conn)
    {
        conn.Db.AccountBid.OnInsert -= OnAccountBidInsert;
        conn.Db.AccountBid.OnUpdate -= OnAccountBidUpdate;
        conn.Db.AccountBid.OnDelete -= OnAccountBidDelete;
        conn.Db.BiddingStateS.OnInsert -= OnBiddingStateSInsert;
        conn.Db.BiddingStateS.OnUpdate -= OnBiddingStateSUpdate;
        conn.Db.BidTimeS.OnUpdate -= OnBidTimeSUpdate;
        Debug.Log("[BidDisplayPanel] Callbacks cleaned up");
    }

    void OnDestroy()
    {
        if (STDB.Conn != null)
            CleanupCallbacks(STDB.Conn);
    }

    // Callback method handlers
    private void OnAccountBidInsert(EventContext ctx, AccountBid accountBid)
    {
        RefreshBidDisplay(ctx.Db);
    }

    private void OnAccountBidUpdate(
        EventContext ctx,
        AccountBid oldAccountBid,
        AccountBid newAccountBid
    )
    {
        RefreshBidDisplay(ctx.Db);
    }

    private void OnAccountBidDelete(EventContext ctx, AccountBid accountBid)
    {
        RefreshBidDisplay(ctx.Db);
    }

    private void OnBiddingStateSInsert(EventContext ctx, BiddingStateS biddingState)
    {
        OnBiddingStateSChange(ctx, biddingState);
    }

    private void OnBiddingStateSUpdate(
        EventContext ctx,
        BiddingStateS oldBiddingState,
        BiddingStateS newBiddingState
    )
    {
        OnBiddingStateSChange(ctx, newBiddingState);
    }

    private void OnBiddingStateSChange(EventContext ctx, BiddingStateS biddingState)
    {
        _currentBidWorldId = biddingState.CurrBidWorldId;
        _bidStateText.SetText(
            $"OtherTileReadyForBidding: {biddingState.OtherTileReadyForBidding}, CurrBidWorldId: {biddingState.CurrBidWorldId}"
        );
    }

    private void OnBidTimeSUpdate(EventContext ctx, BidTimeS oldBidTime, BidTimeS newBidTime)
    {
        var ts = TimeSpan.FromTicks(newBidTime.MicrosecondsRemaining * 10);
        _bidTimeText.SetText(ts.ToString(@"mm\:ss"));
    }

    void Update()
    {
        // Determine target origin based on current bid world ID
        Transform targetOrigin = _currentBidWorldId == 2 ? _GameTile2Origin : _GameTile1Origin;

        if (targetOrigin != null)
        {
            // For a worldspace canvas, we can directly lerp the transform's position
            transform.position = Vector3.Lerp(
                transform.position,
                targetOrigin.position,
                _lerpSpeed * Time.deltaTime
            );
        }
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
