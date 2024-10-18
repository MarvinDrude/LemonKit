
using Npgsql;

namespace LemonKit.MainDemo.Database.Factories;

public sealed class PostgreConnectionFactory
    : IDbConnectionFactory<PostgreConnectionStringProvider, string>
{
    private readonly PostgreConnectionStringProvider _ConnectionStringProvider;

    public PostgreConnectionFactory(
        PostgreConnectionStringProvider connectionProvider)
    {
        _ConnectionStringProvider = connectionProvider;
    }

    public async Task<IDbConnection> Create(CancellationToken cancellationToken)
    {
        string connectionString = _ConnectionStringProvider.GetConnectionString();

        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken: cancellationToken);

        return connection;
    }
}

public sealed class PostgreConnectionStringProvider : IConnectionStringProvider<string>
{
    public bool IsAsync => false;

    private readonly SettingsContainer<MainSettings> _SettingsContainer;
    private MainSettings Settings => _SettingsContainer.Current;

    public PostgreConnectionStringProvider(
        SettingsContainer<MainSettings> settingsContainer)
    {
        _SettingsContainer = settingsContainer;
    }

    public string GetConnectionString() => Settings.ConnectionString;

    public Task<string> GetConnectionStringAsync()
    {
        throw new NotImplementedException();
    }
}
