
namespace LemonKit.Settings.Builders;

/// <summary>
/// Used to build a settings container for services
/// </summary>
public sealed class SettingsBuilder<T>
    where T : class, ISettings {

    private EnvironmentSettingsProvider? _EnvironmentProvider;
    private SettingsContainer<T> _Container;

    private Dictionary<string, ISettingsProvider> _FileProviders;

    public SettingsBuilder() {

        _Container = new SettingsContainer<T>();
        _FileProviders = [];

    }

    /// <summary>
    /// Adds environment variables, keep in mind they only refresh after restart
    /// </summary>
    public SettingsBuilder<T> AddEnvironmentVariables() {

        _EnvironmentProvider = new EnvironmentSettingsProvider() {
            _SettingsContainer = _Container
        };
        return this;

    }

    /// <summary>
    /// Add a json file to reading, default folder is "AppDomain.CurrentDomain.BaseDirectory"
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="folderPath"></param>
    /// <param name="watch"></param>
    /// <returns></returns>
    public SettingsBuilder<T> AddJsonFile<E>(
        string fileName, 
        JsonSerializerContext jsonContext, 
        string? folderPath = null, 
        bool watch = true)
        where E : class {

        _FileProviders[fileName] = new JsonFileProvider<E>(fileName, jsonContext, folderPath, watch) { 
            _SettingsContainer = _Container
        };
        return this;

    }

    /// <summary>
    /// Sets the actual providers of the container
    /// </summary>
    /// <returns></returns>
    internal SettingsContainer<T> Build() {

        _Container.EnvironmentProvider = _EnvironmentProvider;
        _Container.FileProviders = _FileProviders;

        _Container.Init().GetAwaiter().GetResult(); // should be ok for only builder at startup

        return _Container;

    }

}
