using GameCoreLib;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void AttackThrone(ReducerContext ctx)
    {
        //Create an input event into the GameCore to attack with one marble

        //TODO: Spend a marble from the player's account

        Account account = Account.GetOrCreate(ctx);

        InputEvent.Attack inputEvent = new InputEvent.Attack(account.Id, 1);

        ctx.Db.InputCollector.Insert(
            new InputCollector { delaySeqs = 0, inputEventData = inputEvent.ToBinary() }
        );
        Log.Info($"Inserted input event to attack throne with account {account.Id}");
    }
}
