using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void ClaimDailyReward(ReducerContext ctx)
    {
        Account account = Account.GetOrCreate(ctx);

        long currentDay = ctx.Timestamp.MicrosecondsSinceUnixEpoch.GetDayIndex();
        long lastClaimDay = account.LastDailyRewardClaimDay;
        if (currentDay == lastClaimDay) // Enforce one claim per logical day.
        {
            Log.Error("Account has already claimed the daily reward for today");
            return;
        }

        //Compute the next streak based on if yesterday was the last claim day or not.
        if (lastClaimDay + 1 == currentDay)
            account.DailyRewardClaimStreak = account.DailyRewardClaimStreak.WrappingAdd(1);
        else
            account.DailyRewardClaimStreak = 1;

        //Reward based on the *new* streak value
        BaseCfgS cfg = BaseCfgS.Inst(ctx);
        ushort rewardMarbles;
        if (account.DailyRewardClaimStreak <= 1)
            rewardMarbles = cfg.dayRewardStreak1Marbles;
        else if (account.DailyRewardClaimStreak <= 2)
            rewardMarbles = cfg.dayRewardStreak2Marbles;
        else if (account.DailyRewardClaimStreak <= 3)
            rewardMarbles = cfg.dayRewardStreak3Marbles;
        else if (account.DailyRewardClaimStreak <= 4)
            rewardMarbles = cfg.dayRewardStreak4Marbles;
        else if (account.DailyRewardClaimStreak <= 5)
            rewardMarbles = cfg.dayRewardStreak5Marbles;
        else if (account.DailyRewardClaimStreak <= 6)
            rewardMarbles = cfg.dayRewardStreak6Marbles;
        else
            rewardMarbles = cfg.dayRewardStreak7Marbles;

        account.Marbles = account.Marbles.SaturatingAdd(rewardMarbles);
        ctx.Db.Account.Id.Update(account);
    }

    [Reducer]
    public static void SetUsername(
        ReducerContext ctx,
        string username,
        bool overwriteExisting = false
    )
    {
        Account account = Account.GetOrCreate(ctx);
        AccountCustomization? accountCustomization = ctx.Db.AccountCustomization.AccountId.Find(
            account.Id
        );
        if (accountCustomization.HasValue && overwriteExisting)
        {
            var accountCustomizationValue = accountCustomization.Value;
            accountCustomizationValue.Username = username;
            ctx.Db.AccountCustomization.AccountId.Update(accountCustomizationValue);
        }
        else
        {
            var newAccountCustomization = new AccountCustomization
            {
                AccountId = account.Id,
                Username = username,
            };
            ctx.Db.AccountCustomization.Insert(newAccountCustomization);
        }
    }

    [Reducer]
    public static void IncrementPfpVersion(ReducerContext ctx)
    {
        Account account = Account.GetOrCreate(ctx);
        AccountCustomization accountCustomization = AccountCustomization.GetOrCreate(
            ctx,
            account.Id
        );
        accountCustomization.PfpVersion = (byte)(accountCustomization.PfpVersion + 1);
        ctx.Db.AccountCustomization.AccountId.Update(accountCustomization);
    }
}
