using SpacetimeDB;

public static partial class Module
{
        // Table to store serialized world state
        [SpacetimeDB.Table(Public = true)]
        public partial struct LevelFile
        {

            [PrimaryKey]
            public string LevelName;

            public string LevelJSON;
        }
}

