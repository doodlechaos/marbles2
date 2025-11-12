using SpacetimeDB;

public static partial class Module
{
        [Table(Public = false)]
        public partial struct StepsSinceLastBatch
        {
            [PrimaryKey]
            public byte Id; //u8
            public ushort Value; //u16

            public static void SetSingleton(ReducerContext ctx, ushort value){
                var opt = ctx.Db.StepsSinceLastBatch.Id.Find(0);
                if (opt.HasValue)
                {
                    var updated = opt.Value;
                    updated.Value = value;
                    ctx.Db.StepsSinceLastBatch.Id.Update(updated);
                }
                else
                {
                    ctx.Db.StepsSinceLastBatch.Insert(new StepsSinceLastBatch { Id = 0, Value = value });
                }
            }

        }
}

