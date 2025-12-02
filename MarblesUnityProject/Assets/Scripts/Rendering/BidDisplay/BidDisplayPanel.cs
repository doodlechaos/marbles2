using System.Collections.Generic;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Pool;

public class BidDisplayPanel : MonoBehaviour
{
    [SerializeField]
    private AuctionPlayerEntry _auctionPlayerEntryPrefab;

    [SerializeField]
    private List<AuctionPlayerEntry> _activeAuctionPlayerEntries = new List<AuctionPlayerEntry>();

    [SerializeField]
    private float _spacingBetweenAuctionEntries = 90.0f;

    //TODO: Every time there is a new bid, update the positions of the playerbidentry prefabs

    public void SetCallbacks(DbConnection conn)
    {
        conn.Db.AccountBid.OnInsert += (EventContext ctx, AccountBid accountBid) =>
        {
            OnAccountBidCallback(ctx);
        };
        conn.Db.AccountBid.OnUpdate += (
            EventContext ctx,
            AccountBid oldAccountBid,
            AccountBid newAccountBid
        ) =>
        {
            OnAccountBidCallback(ctx);
        };
        Debug.Log("Set callbacks for accountbid table.");
    }

    void OnAccountBidCallback(EventContext ctx)
    {
        //TODO: Update the entire list of auctionplayerentries by iterating through all the AccountBid rows.
        Debug.Log("Detected change in accountbid table. ");
    }

    public void OnBidButtonClicked()
    {
        //Assume a bid of one marble for now
        STDB.Conn.Reducers.PlaceBid(1);
        Debug.Log($"Called reducer to place bid.");
    }

    void Update() { }
}
