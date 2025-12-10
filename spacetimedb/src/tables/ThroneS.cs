using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct Throne
    {
        [PrimaryKey]
        public byte Id;
        public ulong? KingAccountId;

        public static Throne Inst(ReducerContext ctx)
        {
            var opt = ctx.Db.Throne.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                return ctx.Db.Throne.Insert(new Throne { Id = 0, KingAccountId = null });
            }
        }
    }
}
