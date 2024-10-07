
namespace LemonKit.SimpleDemo.Database.Concrete.Pets;

[ModuleService]
public sealed partial class PetService : IPetService {

    [ModuleProperty(typeof(PetReadModule))]
    public partial IPetReadModule Read { get; set; }

    [ModuleProperty(typeof(PetWriteModule))]
    public partial IPetWriteModule Write { get; set; }



    public PetService() {



    }

}
