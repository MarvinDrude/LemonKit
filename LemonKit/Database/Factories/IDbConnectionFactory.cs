
namespace LemonKit.Database.Factories;

public interface IDbConnectionFactory<TConnectionStringProvider, TConnectionString>
    : IDbBaseConnectionFactory
    where TConnectionStringProvider : IConnectionStringProvider<TConnectionString>
{

}

public interface IDbBaseConnectionFactory
{
    public Task<IDbConnection> Create();
}