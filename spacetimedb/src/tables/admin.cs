using SpacetimeDB;

public static partial class Module
{
        [SpacetimeDB.Table(Public = false)]
        public partial struct Admin
        {
            [PrimaryKey]
            public Identity AdminIdentity;

        }
}

