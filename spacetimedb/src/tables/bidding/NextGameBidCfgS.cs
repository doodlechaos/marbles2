using GameCoreLib;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct NextGameBidCfgS
    {
        [PrimaryKey]
        public byte Id;

        public GameBidCfg GameBidCfg;

        public static NextGameBidCfgS Inst(ReducerContext ctx)
        {
            if (ctx.Db.NextGameBidCfgS.Id.Find(0) is NextGameBidCfgS opt)
            {
                return opt;
            }
            else
            {
                return ctx.Db.NextGameBidCfgS.Insert(
                    new NextGameBidCfgS { Id = 0, GameBidCfg = new GameBidCfg(2, 20, 10) }
                );
            }
        }
    }
}
