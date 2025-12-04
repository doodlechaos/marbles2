using SpacetimeDB;

public static partial class Module
{
    [Type]
    public enum SessionKind : byte
    {
        Internal,
        Anonymous,
        SpacetimeAuth,
    }

    [Table(Public = false)]
    public partial struct Session
    {
        [PrimaryKey]
        public Identity Identity;

        public SessionKind Kind;

        public bool HasJwt;
        public string Issuer;
        public string Subject;

        public Timestamp ConnectedAt;
    }

    [Type]
    public partial struct MySessionKindRow
    {
        public SessionKind Kind;
    }

    [View(Name = "MySessionKind", Public = true)]
    public static MySessionKindRow? MySessionKind(ViewContext ctx)
    {
        if (ctx.Db.Session.Identity.Find(ctx.Sender) is Session s)
        {
            return new MySessionKindRow { Kind = s.Kind };
        }

        return null;
    }
}
