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
            ushort value = Get(ctx);
            value = value.WrappingAdd(1);
            Set(ctx, value);
        }

        public static void Set(ReducerContext ctx, ushort value)
        {
            ctx.Db.Seq.Id.Delete(0);
            ctx.Db.Seq.Insert(new Seq { Id = 0, Value = value });
        }

        public static ushort Get(ReducerContext ctx)
        {
            var opt = ctx.Db.Seq.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value.Value;
            }
            else
            {
                Set(ctx, 0);
                return 0;
            }
        }
    }
}
