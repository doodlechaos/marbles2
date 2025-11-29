using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct GameCoreSnap
    {
        [PrimaryKey]
        public byte Id;

        public ushort Seq;

        public byte[] BinaryData;

        public static void SetSingleton(ReducerContext ctx, GameCoreSnap row)
        {
            ctx.Db.GameCoreSnap.Id.Delete(0);
            ctx.Db.GameCoreSnap.Insert(row);
        }

        public static GameCoreSnap GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.GameCoreSnap.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                return new GameCoreSnap
                {
                    Id = 0,
                    Seq = 0,
                    BinaryData = new byte[0],
                };
            }
        }
    }
}
