using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct InputCollector
    {
        public ushort delaySeqs;
        // Serialized input event as bytes - should be replaced with actual InputEvent type when available
        public byte[] inputEvent;
    }

    [Reducer]
    public static void UpsertInputCollector(ReducerContext ctx, InputCollector row)
    {
        ctx.Db.InputCollector.Delete(row);
        ctx.Db.InputCollector.Insert(row);
    }
}

