
namespace LemonKit.MainDemo.Database.Postgre.Pets;

[Module]
[Observe(
    activitySourceName: "PetWriteModule.Activity",
    meterName: "PetWriteModule.Meter"
)]
public sealed partial class PetWriteModule : IPetWriteModule
{
    private static readonly Counter<long> _QueryCounter;

    static PetWriteModule()
    {
        _QueryCounter = _Meter.CreateCounter<long>("PetService.Write.QueryCount", "Queries");
    }

    private readonly ILogger<PetService> _Logger;
    private readonly IDbBaseConnectionFactory _DbFactory;
    private readonly DbBaseModule _Module;

    public PetWriteModule(
        ILogger<PetService> logger,
        IDbBaseConnectionFactory dbFactory)
    {
        _Logger = logger;
        _DbFactory = dbFactory;

        _Module = new DbBaseModule(dbFactory);
    }

    public async Task<Result<Guid, Error>> Insert(
        Pet toCreate)
    {
        using var activity = StartActivity();
        _QueryCounter.Add(1);

        async Task<Guid?> Execute(
            IDbConnection conn,
            IDbTransaction? trans,
            CancellationToken token)
        {
            var command = new CommandDefinition($@"

                INSERT INTO {Pet.Sql.TableName} 
                (Id, Name, Color, Height)
                OUTPUT INSERTED.Id
                VALUES
                (@{nameof(Pet.Id)}, @{nameof(Pet.Name)}, @{nameof(Pet.Color)}, @{nameof(Pet.Height)});

            ", toCreate, transaction: trans, cancellationToken: token);

            return await conn.QuerySingleOrDefaultAsync<Guid?>(command);
        }

        var (isSuccess, cInsertId, cError) = await _Module.Execute(Execute);

        if(cInsertId is Guid insertId)
        {
            return insertId;
        }
        
        if(cError is Exception error)
        {
            return new Error(error.ToString());
        }

        return new Error("Unknown");

    }

    public async Task<Result<int, Error>> Update(
        Pet toUpdate)
    {
        using var activity = StartActivity();
        _QueryCounter.Add(1);

        async Task<int> Execute(
            IDbConnection conn,
            IDbTransaction? trans,
            CancellationToken token)
        {
            var command = new CommandDefinition($@"

                UPDATE {Pet.Sql.TableName} SET
                    Name = @{nameof(Pet.Name)},
                    Color = @{nameof(Pet.Color)},
                    Height = @{nameof(Pet.Height)}
                WHERE
                    Id = @{nameof(Pet.Id)}

            ", toUpdate, transaction: trans, cancellationToken: token);

            return await conn.ExecuteAsync(command);
        }

        var (isSuccess, updateCount, cError) = await _Module.Execute(Execute);

        if(isSuccess)
        {
            return updateCount;
        }

        if(cError is Exception error)
        {
            return new Error(error.ToString());
        }

        return new Error("Unknown");

    }
}
