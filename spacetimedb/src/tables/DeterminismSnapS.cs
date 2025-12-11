using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct DeterminismSnapS
    {
        [PrimaryKey]
        public byte Id;

        public ushort Seq;
        public string HashString;

        public static DeterminismSnapS Inst(ReducerContext ctx)
        {
            if (ctx.Db.DeterminismSnapS.Id.Find(0) is DeterminismSnapS opt)
            {
                return opt;
            }
            else
            {
                return ctx.Db.DeterminismSnapS.Insert(
                    new DeterminismSnapS
                    {
                        Id = 0,
                        Seq = 0,
                        HashString = "",
                    }
                );
            }
        }
    }
}
