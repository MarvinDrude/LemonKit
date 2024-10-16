
namespace LemonKit.Settings.Providers;

/// <summary>
/// Common methods needed for every settings provider
/// </summary>
public interface ISettingsProvider
{

    /// <summary>
    /// Reload the data of the provider
    /// </summary>
    /// <returns></returns>
    public Task Reload();

}
