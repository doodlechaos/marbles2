using System.Collections.Generic;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct AccountCustomization
    {
        [PrimaryKey]
        public ulong AccountId;
        public string Username;

        //TODO: Username string color
        public byte PfpVersion;

        [SpacetimeDB.Index.BTree]
        public Timestamp LastActiveTime;

        public static AccountCustomization GetOrCreate(ReducerContext ctx, ulong accountId)
        {
            Touch(ctx, accountId);
            var accountCustomizationOpt = ctx.Db.AccountCustomization.AccountId.Find(accountId);
            if (accountCustomizationOpt.HasValue)
            {
                return accountCustomizationOpt.Value;
            }
            else
            {
                var newAccountCustomization = new AccountCustomization
                {
                    AccountId = accountId,
                    Username = "",
                    PfpVersion = 0,
                };
                return ctx.Db.AccountCustomization.Insert(newAccountCustomization);
            }
        }

        public static void Touch(ReducerContext ctx, ulong accountId)
        {
            if (ctx.Db.AccountCustomization.AccountId.Find(accountId) is AccountCustomization row)
            {
                row.LastActiveTime = ctx.Timestamp;
                ctx.Db.AccountCustomization.AccountId.Update(row);
            }
            else
            {
                var newAccountCustomization = new AccountCustomization
                {
                    AccountId = accountId,
                    Username = "",
                    PfpVersion = 0,
                    LastActiveTime = ctx.Timestamp,
                };
                ctx.Db.AccountCustomization.Insert(newAccountCustomization);
            }
        }

        [View(Name = "ActiveAccountCustomsView", Public = true)]
        public static List<AccountCustomization> ActiveAccountCustomsView(AnonymousViewContext ctx)
        {
            var rows = new List<AccountCustomization>();

            if (ctx.Db.Clock.Id.Find(0) is Clock clock)
            {
                Timestamp now = clock.PrevClockUpdate;
                var cut = now + new TimeDuration(-60_000_000);

                foreach (var row in ctx.Db.AccountCustomization.LastActiveTime.Filter((cut, now)))
                {
                    rows.Add(row);
                }
            }

            return rows;
        }
    }

    //TODO: Expose a view of the accountcustomizations which is a join of the activeaccountcache
    /*     [View(Name = "ActiveAccountCustomizations", Public = true)]
        public static List<AccountCustomization> ActiveAccountCustomizations(ViewContext ctx){
            var rows = new List<AccountCustomization>();
    
            foreach (var entry in ctx.Db.ActiveAccountCache.
            {
                if (ctx.Db.AccountCustomization.AccountId.Find(entry.AccountId) is AccountCustomization accountCustomization)
                {
                    rows.Add(accountCustomization);
                }
            }
    
            return rows;
        }; */
}
