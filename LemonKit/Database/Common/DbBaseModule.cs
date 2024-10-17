
namespace LemonKit.Database.Common;

public sealed class DbBaseModule
{
    private readonly IDbBaseConnectionFactory _ConnectionFactory;

    public DbBaseModule(
        IDbBaseConnectionFactory factory)
    {
        _ConnectionFactory = factory;
    }

    public async Task<Result<TResult, Exception>> Execute<TResult>(
        Func<IDbConnection, IDbTransaction?, Task<TResult>> executeFunc)
    {
        var context = DbQueryContext.Current;

        IDbConnection? connection = context.Connection;
        IDbTransaction? transaction = context.Transaction;

        bool connectionNeeded = connection is null;
        connection ??= await _ConnectionFactory.Create();

        try
        {
            return await executeFunc.Invoke(
                connection, 
                transaction);
        }
        catch(Exception error)
        {
            return error;
        }
        finally
        {
            if(connectionNeeded)
            {
                connection?.Dispose();
            }
        }
    }
}
