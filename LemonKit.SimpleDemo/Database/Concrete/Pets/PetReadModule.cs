namespace LemonKit.SimpleDemo.Database.Concrete.Pets;

[Module]
public sealed class PetReadModule : IPetReadModule
{

    private readonly ILogger<PetService> _Logger;
    private readonly MainDbContext _DbContext;

    public PetReadModule(
        ILogger<PetService> logger,
        MainDbContext dbContext)
    {

        _Logger = logger;
        _DbContext = dbContext;

    }

}
