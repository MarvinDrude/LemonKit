
namespace LemonKit.SimpleDemo.Database.Concrete.Pets;

[ModuleService]
public sealed partial class PetService : IPetService {

    [ModuleProperty(typeof(PetReadModule))]
    public partial IPetReadModule Read { get; }

    [ModuleProperty(typeof(PetWriteModule))]
    public partial IPetWriteModule Write { get; }

    private readonly ILogger<PetService> _Logger;
    private readonly MainDbContext _DbContext;

    public PetService(
        ILogger<PetService> logger,
        MainDbContext dbContext) {

        _Logger = logger;
        _DbContext = dbContext;

    }

}
