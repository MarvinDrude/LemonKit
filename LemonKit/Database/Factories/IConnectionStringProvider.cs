
namespace LemonKit.Database.Factories;

public interface IConnectionStringProvider<TConnectionString>
{
    public bool IsAsync { get; }

    public Task<TConnectionString> GetConnectionStringAsync();

    public TConnectionString GetConnectionString();
}
