using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct AccountCustomization
    {
        [PrimaryKey]
        public ulong AccountId;
        public string Username;
        public byte PfpVersion;

        public static AccountCustomization GetOrCreate(ReducerContext ctx)
        {
            Account account = AccountHelper.GetOrCreate(ctx);
            var accountCustomizationOpt = ctx.Db.AccountCustomization.AccountId.Find(account.Id);
            if (accountCustomizationOpt.HasValue)
            {
                return accountCustomizationOpt.Value;
            }
            else
            {
                var newAccountCustomization = new AccountCustomization
                {
                    AccountId = account.Id,
                    Username = "",
                    PfpVersion = 0,
                };
                return ctx.Db.AccountCustomization.Insert(newAccountCustomization);
            }
        }
    }
}
