using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct GameplayFinishedFlagS
    {
        [PrimaryKey]
        public byte Id;

        public bool IsGameplayFinished; //TODO: Send an output signal out of the GameCore when the gameplay is finished (I don't want to check every frame because I'd have to deserialize the entire GameCore every frame)

        public static GameplayFinishedFlagS Inst(ReducerContext ctx)
        {
            var opt = ctx.Db.GameplayFinishedFlagS.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                return ctx.Db.GameplayFinishedFlagS.Insert(
                    new GameplayFinishedFlagS { Id = 0, IsGameplayFinished = false }
                );
            }
        }
    }
}
