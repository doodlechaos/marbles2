using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct InputCollector
    {
        public ushort delaySeqs;
        public byte[] inputEventData;
    }

    [Reducer]
    public static void UpsertInputCollector(ReducerContext ctx, InputCollector row)
    {
        ctx.Db.InputCollector.Delete(row);
        ctx.Db.InputCollector.Insert(row);
    }
}

