using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct LevelFileData
    {
        [PrimaryKey]
        public string UnityPrefabGUID;
        public byte[] LevelFileBinary;
    }
}
