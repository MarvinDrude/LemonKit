
namespace LemonKit.SimpleDemo.Database.Concrete.Pets;

[Module]
[Observe(
    activitySourceName: "PetWriteModule.Activity"
)]
public sealed partial class PetWriteModule : IPetWriteModule
{

    private readonly ILogger<PetService> _Logger;
    private readonly MainDbContext _DbContext;

    public PetWriteModule(
        ILogger<PetService> logger,
        MainDbContext dbContext)
    {

        _Logger = logger;
        _DbContext = dbContext;

    }

    public async Task<Result<Pet, Exception>> Create(Pet pet, CancellationToken cancellationToken = default)
    {

        try
        {

            _DbContext.Pets.Add(pet);
            await _DbContext.SaveChangesAsync(cancellationToken);

        }
        catch(Exception error)
        {

            return error;

        }

        return pet;

    }

}
