using System;
using System.Collections.Generic;
using System.Linq;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void PlaceBid(ReducerContext ctx, uint bid)
    {
        Account account = Account.GetOrCreate(ctx);
        if (account.Marbles < bid)
        {
            Log.Error(
                $"[BidManager.PlaceBid] Account {account.Id} does not have enough marbles to bid {bid}. Account has {account.Marbles} marbles."
            );
            return;
        }

        account.Marbles = account.Marbles.SaturatingSub(bid);
        ctx.Db.Account.Identity.Update(account);

        if (ctx.Db.AccountBid.AccountId.Find(account.Id) is AccountBid existingBid)
        {
            existingBid.TotalBid += bid;
            existingBid.LatestBid = bid;

            ctx.Db.AccountBid.AccountId.Update(existingBid);
        }
        else
        {
            AccountBid accountBid = new AccountBid
            {
                AccountId = account.Id,
                LatestBid = bid,
                TotalBid = bid,
            };

            ctx.Db.AccountBid.Insert(accountBid);
        }

        Log.Info($"[BidManager.PlaceBid] Account {account.Id} bid {bid} marbles.");
    }

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
        // Only countdown when:
        // 1. The other tile is in Bidding state (so users have something to bid on next)
        // 2. We have enough bidders for a game
        if (!BiddingStateS.Inst(ctx).OtherTileReadyForBidding)
            return;

        if ((int)ctx.Db.AccountBid.Count < BidConfigS.Inst(ctx).MinAuctionSpots)
            return;

        BidTimeS bidTime = BidTimeS.Inst(ctx);
        if (bidTime.MicrosecondsRemaining <= 0)
            return;

        long delta = (long)(deltaTimeSec * 1000000);
        bidTime.MicrosecondsRemaining = bidTime.MicrosecondsRemaining.SaturatingSub(delta);

        BidTimeS.Set(ctx, bidTime);
    }

    private static void FinishBiddingForTile(ReducerContext ctx)
    {
        Log.Info("[BidManager.FinishBiddingForTile] Finishing bidding for tile");

        BidConfigS config = BidConfigS.Inst(ctx);

        // Get all bids (already one row per account with TotalBid pre-calculated)
        List<AccountBid> allBids = ctx.Db.AccountBid.Iter().ToList();

        // Calculate total marbles bid by all accounts
        uint totalMarblesBidByAll = 0;
        foreach (AccountBid bid in allBids)
            totalMarblesBidByAll += bid.TotalBid;

        // Sort accounts by total bid descending
        List<AccountBid> sortedBidders = allBids.OrderByDescending(x => x.TotalBid).ToList();

        // Create an empty list of entrants
        List<InputEvent.Entrant> entrants = new List<InputEvent.Entrant>();

        // Take the top (# of guaranteed spots) highest total bid accounts
        int guaranteedSpots = Math.Min(config.MaxAcutionSpots, sortedBidders.Count);
        for (int i = 0; i < guaranteedSpots; i++)
        {
            entrants.Add(
                new InputEvent.Entrant
                {
                    AccountId = sortedBidders[i].AccountId,
                    TotalBid = sortedBidders[i].TotalBid,
                }
            );
        }

        // Get remaining bidders for raffle
        List<AccountBid> remainingBidders = sortedBidders.Skip(guaranteedSpots).ToList();

        // Randomly select more players (# of raffle draws) from remaining accounts
        int raffleDraws = Math.Min(config.MaxRaffleDraws, remainingBidders.Count);
        for (int i = 0; i < raffleDraws; i++)
        {
            int randomIndex = ctx.Rng.Next(0, remainingBidders.Count);
            AccountBid winner = remainingBidders[randomIndex];
            entrants.Add(
                new InputEvent.Entrant { AccountId = winner.AccountId, TotalBid = winner.TotalBid }
            );
            remainingBidders.RemoveAt(randomIndex);
        }

        BiddingStateS biddingState = BiddingStateS.Inst(ctx);

        // Create an input event with the entrants list and insert it into the input collector
        InputEvent.GameplayStartInput startGameTileEvent = new InputEvent.GameplayStartInput(
            entrants.ToArray(),
            totalMarblesBidByAll,
            biddingState.CurrBidWorldId
        );
        byte[] eventData = startGameTileEvent.ToBinary();
        ctx.Db.InputCollector.Insert(
            new InputCollector { delaySeqs = 0, inputEventData = eventData }
        );

        // Clear all account bids for the next round
        foreach (AccountBid bid in ctx.Db.AccountBid.Iter().ToList())
            ctx.Db.AccountBid.Delete(bid);

        // Reset bid timer for next round
        BidTimeS.Set(ctx, new BidTimeS { MicrosecondsRemaining = SecondsToMicroseconds(10) });

        // Switch bidding to the other tile and reset the ready flag
        // (the flag will be set again when the next tile enters Bidding state)
        biddingState.OtherTileReadyForBidding = false;
        biddingState.CurrBidWorldId = (byte)(biddingState.CurrBidWorldId == 1 ? 2 : 1);
        ctx.Db.BiddingStateS.Id.Update(biddingState);

        Log.Info(
            $"Bidding finished. Gameplay starting on tile {(biddingState.CurrBidWorldId == 1 ? 2 : 1)}, now accepting bids on tile {biddingState.CurrBidWorldId}"
        );
    }
}
