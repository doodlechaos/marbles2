using System;
using System.Collections.Generic;
using System.Linq;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    /// <summary>
    /// Main tick function - called every simulation step
    /// Order is important here
    /// </summary>
    private static void UpdateBidManager(ReducerContext ctx, float deltaTimeSec)
    {
        //If the previous gametile has finished running, and we have at least the minimum number of auction spots, countdown the bid timer
        TryCountdownBidTimer(ctx, deltaTimeSec);

        //If the bid timer reaches 0, send the player bid data to the gametile, reset the bidder, and start the bidder on the next gametile.
        if (BidTimeS.Inst(ctx).MicrosecondsRemaining <= 0)
            FinishBiddingForTile(ctx);
    }

    private static void TryCountdownBidTimer(ReducerContext ctx, float deltaTimeSec)
    {
        //TODO: If the previous gametile hasn't finished running, don't countdown the bid timer
        if (false)
            return;

        if ((int)ctx.Db.AccountBid.Count < BidConfigS.Inst(ctx).MinAuctionSpots)
            return;

        BidTimeS bidTime = BidTimeS.Inst(ctx);
        if (bidTime.MicrosecondsRemaining <= 0)
            return;

        ulong delta = (ulong)(deltaTimeSec * 1000000);
        bidTime.MicrosecondsRemaining = bidTime.MicrosecondsRemaining.SaturatingSub(delta);

        BidTimeS.Set(ctx, bidTime);
    }

    private static void FinishBiddingForTile(ReducerContext ctx)
    {
        //TODO: Send the results of the bid to the gametime we were just bidding on

        //Reset the bidding for the config from the next gametile
    }
}
