
namespace LemonKit.Contexts;

public sealed class DbQueryContext
{
    private static readonly ContextProvider<DbQueryContext> _Provider = new();

    public static DbQueryContext Current => _Provider.Current;

    public IDbTransaction? Transaction { get; set; }

    public IDbConnection? Connection { get; set; }

    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}
