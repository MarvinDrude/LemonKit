
namespace LemonKit.Database.Factories;

public sealed class MsConnectionFactory<TConnectionStringProvider>
    : IDbConnectionFactory<TConnectionStringProvider, string>
    where TConnectionStringProvider : IConnectionStringProvider<string>
{
    private readonly TConnectionStringProvider _ConnectionStringProvider;

    public MsConnectionFactory(
        TConnectionStringProvider connectionStringProvider)
    {
        _ConnectionStringProvider = connectionStringProvider;
    }

    public async Task<IDbConnection> Create(CancellationToken cancellationToken)
    {
        string connectionString = _ConnectionStringProvider.IsAsync
            ? await _ConnectionStringProvider.GetConnectionStringAsync()
            : _ConnectionStringProvider.GetConnectionString();

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken: cancellationToken);

        return connection;
    }
}
