using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void SetUsername(
        ReducerContext ctx,
        string username,
        bool overwriteExisting = false
    )
    {
        Account account = AccountHelper.GetOrCreate(ctx);
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
        AccountCustomization accountCustomization = AccountCustomization.GetOrCreate(ctx);
        accountCustomization.PfpVersion = (byte)(accountCustomization.PfpVersion + 1);
        ctx.Db.AccountCustomization.AccountId.Update(accountCustomization);
    }
}
