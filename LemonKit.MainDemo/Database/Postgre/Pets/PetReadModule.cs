
namespace LemonKit.MainDemo.Database.Postgre.Pets;

[Module]
[Observe(
    activitySourceName: "PetReadModule.Activity",
    meterName: "PetReadModule.Meter"
)]
public sealed partial class PetReadModule : IPetReadModule
{
    private static readonly Counter<long> _QueryCounter;

    static PetReadModule()
    {
        _QueryCounter = _Meter.CreateCounter<long>("PetService.Read.QueryCount", "Queries");
    }

    private readonly ILogger<PetService> _Logger;
    private readonly IDbBaseConnectionFactory _DbFactory;
    private readonly DbBaseModule _Module;

    public PetReadModule(
        ILogger<PetService> logger,
        IDbBaseConnectionFactory dbFactory)
    {
        _Logger = logger;
        _DbFactory = dbFactory;

        _Module = new DbBaseModule(dbFactory);
    }

}
