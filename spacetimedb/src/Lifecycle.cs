using SpacetimeDB;

public static partial class Module
{
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"[Init] Server Starting!");
    }

    [Reducer]
    public static void Connect(ReducerContext ctx)
    {
        Log.Info($"[Init] Client Connecting");

    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        Log.Info($"[Init] Client Disconnecting");

    }

    [Reducer]
    public static void TestReducer(ReducerContext ctx)
    {
        Log.Info("Test reducer called"); 
    }
}