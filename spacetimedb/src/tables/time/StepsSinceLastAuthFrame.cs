using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct StepsSinceLastAuthFrame
    {
        [PrimaryKey]
        public byte Id; //u8
        public ushort Value; //u16

        public static void SetSingleton(ReducerContext ctx, ushort value)
        {
            var opt = ctx.Db.StepsSinceLastAuthFrame.Id.Find(0);
            if (opt.HasValue)
            {
                var updated = opt.Value;
                updated.Value = value;
                ctx.Db.StepsSinceLastAuthFrame.Id.Update(updated);
            }
            else
            {
                ctx.Db.StepsSinceLastAuthFrame.Insert(new StepsSinceLastAuthFrame { Id = 0, Value = value });
            }
        }

    }
}

