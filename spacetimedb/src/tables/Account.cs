using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
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
            var accountOpt = ctx.Db.Account.Identity.Find(ctx.Sender);
            if (accountOpt.HasValue)
            {
                Log.Info($"[Account.GetOrCreate] Found existing account ID: {accountOpt.Value.Id}");
                return accountOpt.Value;
            }

            // Create new account
            var newId = AccountSeqHelper.IncAndGet(ctx);
            Log.Info(
                $"[Account.GetOrCreate] No existing account. Creating new one with new ID from sequence: {newId}"
            );

            var newAccount = new Account
            {
                Identity = ctx.Sender, //IMPORTANT: USE SENDER, NOT IDENTITY
                Id = newId,
                Marbles = 0,
                Points = 0,
                Gold = 0,
                FirstLoginBonusClaimed = false,
                IsMember = false,
                DailyRewardClaimStreak = 0,
                LastDailyRewardClaimDay = 0,
            };

            Log.Info(
                $"[Account.GetOrCreate] Creating AccountCustomization for account ID: {newId}"
            );
            AccountCustomization _ = AccountCustomization.GetOrCreate(ctx, newAccount.Id); //Each account must have a customization row

            Log.Info($"[Account.GetOrCreate] Inserting new account...");
            var insertedAccount = ctx.Db.Account.Insert(newAccount);
            Log.Info(
                $"[Account.GetOrCreate] Successfully inserted account ID: {insertedAccount.Id}"
            );

            return insertedAccount;
        }
    }

    [View(Name = "MyAccount", Public = true)]
    public static Account? MyAccount(ViewContext ctx) => ctx.Db.Account.Identity.Find(ctx.Sender);

    [Reducer]
    public static void UpsertAccount(ReducerContext ctx, Account row)
    {
        ctx.Db.Account.Identity.Delete(row.Identity);
        ctx.Db.Account.Insert(row);
    }
}
