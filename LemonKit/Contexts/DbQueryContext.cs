
namespace LemonKit.Contexts;

public sealed class DbQueryContext
{
    private static readonly ContextProvider<DbQueryContext> _Provider = new ContextProvider<DbQueryContext>();

    public static DbQueryContext Current => _Provider.Current;

    public IDbTransaction? Transaction { get; set; }

    public IDbConnection? Connection { get; set; }
}
