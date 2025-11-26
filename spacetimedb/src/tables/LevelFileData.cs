using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct LevelFileData
    {
        [PrimaryKey]
        public string UnityPrefabGUID;

        public int Rarity;

        public byte[] LevelFileBinary; //<< Serialized LevelFile
    }

    [Reducer]
    public static void UpsertLevelFileData(ReducerContext ctx, LevelFileData levelFileData)
    {
        // Delete existing entry if it exists, then insert the new one (upsert behavior)
        ctx.Db.LevelFileData.UnityPrefabGUID.Delete(levelFileData.UnityPrefabGUID);

        ctx.Db.LevelFileData.Insert(levelFileData);
    }
}
