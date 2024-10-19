
namespace LemonKit.MainDemo.Startup;

[StartupModule(
    prePriority: 1,
    postPriority: 1
)]
public sealed class DbSeederModule
    : IStartupModule<WebApplicationBuilder, WebApplication>
{
    public async Task OnPreStart(WebApplicationBuilder builder)
    {

    }

    public async Task OnPostStart(WebApplication app)
    {

    }
}
