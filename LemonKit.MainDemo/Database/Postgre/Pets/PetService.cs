namespace LemonKit.MainDemo.Database.Postgre.Pets;

[Observe(
    activitySourceName: "PetService.Activity",
    meterName: "PetService.Meter"
)]
[ModuleService]
public partial class PetService : IPetService
{
    [ModuleProperty(typeof(PetReadModule))]
    public partial IPetReadModule Read { get; }

    [ModuleProperty(typeof(PetWriteModule))]
    public partial IPetWriteModule Write { get; }

    private readonly ILogger<PetService> _Logger;
    private readonly IDbBaseConnectionFactory _DbFactory;

    public PetService(
        ILogger<PetService> logger,
        IDbBaseConnectionFactory dbFactory)
    {
        _Logger = logger;
        _DbFactory = dbFactory;
    }
}
