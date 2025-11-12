using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Internal;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct InputFrame
    {
        [PrimaryKey]
        public ushort Seq;

        // Serialized input events as bytes - should be replaced with actual InputEvent type when available
        public byte[] InputEventsList;
    }

    [Reducer]
    public static void UpsertInputFrame(ReducerContext ctx, InputFrame row)
    {
        ctx.Db.InputFrame.Seq.Delete(row.Seq);
        ctx.Db.InputFrame.Insert(row);
    }
}

