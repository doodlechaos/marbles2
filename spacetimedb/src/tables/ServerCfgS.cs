using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct ServerCfgS
    {
        [PrimaryKey]
        public byte Id;

        public ushort activeAccountEvectionScanIntervalSec;

        public static ServerCfgS Inst(ReducerContext ctx)
        {
            if (ctx.Db.ServerCfgS.Id.Find(0) is ServerCfgS opt)
            {
                return opt;
            }
            else
            {
                return ctx.Db.ServerCfgS.Insert(
                    new ServerCfgS { Id = 0, activeAccountEvectionScanIntervalSec = 60 }
                );
            }
        }
    }
}
