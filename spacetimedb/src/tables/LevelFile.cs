using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct LevelFile
    {
        [PrimaryKey]
        public byte Id;

        public string Json;
    }
}
