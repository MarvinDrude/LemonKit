namespace LemonKit.MainDemo.Database.Interfaces.Pets;

public partial interface IPetWriteModule
{
    public Task<Result<Guid, Error>> Insert(Pet toCreate);
}
