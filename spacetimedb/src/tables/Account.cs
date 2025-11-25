using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct Account
    {
        [PrimaryKey]
        public Identity Identity;

        [Unique]
        public ulong Id;

        public bool IsConnected;

        public uint Marbles;
        public uint Points;
        public uint Gold;
        public bool FirstLoginBonusClaimed;
        public bool IsMember;
        public long DailyRewardClaimStreak;
        public long LastDailyRewardClaimDay;

        public static Account GetOrCreate(ReducerContext ctx)
        {
            var accountOpt = ctx.Db.Account.Identity.Find(ctx.Identity);
            if (accountOpt.HasValue)
            {
                return accountOpt.Value;
            }

            // Create new account
            var newId = AccountSeqHelper.IncAndGet(ctx);
            var newAccount = new Account
            {
                Identity = ctx.Identity,
                Id = newId,
                Marbles = 0,
                Points = 0,
                Gold = 0,
                FirstLoginBonusClaimed = false,
                IsMember = false,
                DailyRewardClaimStreak = 0,
                LastDailyRewardClaimDay = 0,
            };

            AccountCustomization _ = AccountCustomization.GetOrCreate(ctx, newAccount.Id); //Each account must have a customization row

            return ctx.Db.Account.Insert(newAccount);
        }
    }

    [Reducer]
    public static void UpsertAccount(ReducerContext ctx, Account row)
    {
        ctx.Db.Account.Identity.Delete(row.Identity);
        ctx.Db.Account.Insert(row);
    }

    public static Account? GetById(ReducerContext ctx, ulong accountId)
    {
        return ctx.Db.Account.Id.Find(accountId);
    }
}
