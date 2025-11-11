using SpacetimeDB;

public static partial class Module
{
        [Table(Public = false)]
        public partial struct Admin
        {
            [PrimaryKey]
            public Identity AdminIdentity;

        }
}

