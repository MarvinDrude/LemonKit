
namespace LemonKit.Settings;

public interface ISettingsContainer
{

    public Dictionary<string, ISettingsProvider> FileProviders { get; }

    public EnvironmentSettingsProvider? EnvironmentProvider { get; }

    public void OnUpdate(ISettingsProvider provider);

}
