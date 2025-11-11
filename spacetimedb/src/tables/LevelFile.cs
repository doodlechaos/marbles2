using SpacetimeDB;

public static partial class Module
{
        [SpacetimeDB.Table(Public = true)]
        public partial struct LevelFile
        {
            [PrimaryKey]
            public string LevelName;

            public string LevelJSON;
        }
}

