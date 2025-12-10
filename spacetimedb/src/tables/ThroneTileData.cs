using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct ThroneTileData
    {
        [PrimaryKey]
        public string UnityPrefabGUID;

        public string TileName;

        public byte[] ThroneTileBinary;
    }

    [Reducer]
    public static void UpsertThroneTile(ReducerContext ctx, ThroneTileData throneTileData)
    {
        // Delete existing entry if it exists, then insert the new one (upsert behavior)
        ctx.Db.ThroneTileData.UnityPrefabGUID.Delete(throneTileData.UnityPrefabGUID);
        ctx.Db.ThroneTileData.Insert(throneTileData);
        Log.Info($"Upserted ThroneTileData: {throneTileData.UnityPrefabGUID}");

        ThroneTile? throneTile = MemoryPackSerializer.Deserialize<ThroneTile>(
            throneTileData.ThroneTileBinary
        );

        if (throneTile == null)
        {
            Log.Error($"Failed to deserialize ThroneTile from {throneTileData.UnityPrefabGUID}");
            return;
        }

        var inputEvent = new InputEvent.LoadThroneTile(throneTile);
        var inputEventData = inputEvent.ToBinary();
        ctx.Db.InputCollector.Insert(
            new InputCollector { delaySeqs = 0, inputEventData = inputEventData }
        );
    }
}
