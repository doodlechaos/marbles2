using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct StepsSinceLastAuthFrame
    {
        [PrimaryKey]
        private byte Id; //u8
        public ushort Value; //u16

        public static void Inc(ReducerContext ctx)
        {
            ushort value = Get(ctx);
            value = value.WrappingAdd(1);
            Set(ctx, value);
        }

        public static void Clear(ReducerContext ctx)
        {
            ctx.Db.StepsSinceLastAuthFrame.Id.Delete(0);
            ctx.Db.StepsSinceLastAuthFrame.Insert(
                new StepsSinceLastAuthFrame { Id = 0, Value = 0 }
            );
        }

        public static void Set(ReducerContext ctx, ushort value)
        {
            ctx.Db.StepsSinceLastAuthFrame.Id.Delete(0);
            ctx.Db.StepsSinceLastAuthFrame.Insert(
                new StepsSinceLastAuthFrame { Id = 0, Value = value }
            );
        }

        public static ushort Get(ReducerContext ctx)
        {
            var opt = ctx.Db.StepsSinceLastAuthFrame.Id.Find(0);
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
