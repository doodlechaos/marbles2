using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct Seq
    {
        [PrimaryKey]
        private byte Id; //u8
        public ushort Value; //u16

        public static void Step(ReducerContext ctx)
        {
            Seq seq = GetSingleton(ctx);
            seq.Value = seq.Value.WrappingAdd(1);
            Set(ctx, seq.Value);
        }

        public static void Set(ReducerContext ctx, ushort value)
        {
            ctx.Db.Seq.Id.Delete(0);
            ctx.Db.Seq.Insert(new Seq { Id = 0, Value = value });
        }

        public static Seq GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.Seq.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                Set(ctx, 0);
                return new Seq { Id = 0, Value = 0 };
            }
        }
    }
}

