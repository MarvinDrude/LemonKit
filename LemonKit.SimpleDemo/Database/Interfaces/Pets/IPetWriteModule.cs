namespace LemonKit.SimpleDemo.Database.Interfaces.Pets;

public partial interface IPetWriteModule
{

    public Task<Result<Pet, Exception>> Create(Pet pet, CancellationToken cancellationToken = default);

}