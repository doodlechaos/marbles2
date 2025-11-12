using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct StepsSinceLastAuthFrame
    {
        [PrimaryKey]
        private byte Id; //u8
        public ushort Value; //u16

        public static void Set(ReducerContext ctx, ushort value)
        {
            ctx.Db.StepsSinceLastAuthFrame.Id.Delete(0);
            ctx.Db.StepsSinceLastAuthFrame.Insert(new StepsSinceLastAuthFrame { Id = 0, Value = value });
        }

        public static StepsSinceLastAuthFrame GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.StepsSinceLastAuthFrame.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                Set(ctx, 0);
                return new StepsSinceLastAuthFrame { Id = 0, Value = 0 };
            }
        }

    }
}

