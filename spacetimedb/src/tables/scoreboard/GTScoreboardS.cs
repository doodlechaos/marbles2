using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct GTScoreboardS
    {
        [PrimaryKey]
        public byte Id;

        public string GTUnityPrefabGUID;

        public ScoreboardEntry[] Entries; //In order of rank (1st, 2nd, 3rd, etc.)

        public static GTScoreboardS Inst(ReducerContext ctx)
        {
            var opt = ctx.Db.GTScoreboardS.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                return ctx.Db.GTScoreboardS.Insert(
                    new GTScoreboardS
                    {
                        Id = 0,
                        GTUnityPrefabGUID = "",
                        Entries = Array.Empty<ScoreboardEntry>(),
                    }
                );
            }
        }
    }
}
