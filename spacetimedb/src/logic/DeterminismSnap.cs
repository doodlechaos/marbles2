using GameCoreLib;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void TakeDeterminismSnap(ReducerContext ctx, DeterminismSnapSchedule schedule)
    {
        InputEvent takeSnapInputEvent = new InputEvent.Dhash();
        ctx.Db.InputCollector.Insert(
            new InputCollector { delaySeqs = 0, inputEventData = takeSnapInputEvent.ToBinary() }
        );
        Log.Info($"Initiated determinism snap input command to gamecore");
    }
}
