using System.Collections.Generic;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct AuthFrame
    {
        [PrimaryKey]
        public ushort Seq;

        public List<InputFrame> Frames;
    }

    [Reducer]
    public static void UpsertAuthFrame(ReducerContext ctx, AuthFrame row)
    {
        ctx.Db.AuthFrame.Seq.Delete(row.Seq);
        ctx.Db.AuthFrame.Insert(row);
    }
}

