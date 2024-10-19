
namespace LemonKit.Startup;

public interface IStartupModule<TPre, TPost>
{
    public Task OnPreStart(TPre pre);

    public Task OnPostStart(TPost post);
}
