

namespace LemonKit.Settings;

public sealed class SettingsContainer<TOptions> : ISettingsContainer
    where TOptions : class, ISettings {

    public EnvironmentSettingsProvider? EnvironmentProvider {
        get {
            return _EnvironmentProvider;
        }
        internal set {
            _EnvironmentProvider = value;
        }
    }

    public Dictionary<string, ISettingsProvider> FileProviders {
        get {
            return _FileProviders;
        }
        internal set {
            _FileProviders = value;
        }
    }

    public TOptions Current => _Current;

    private EnvironmentSettingsProvider? _EnvironmentProvider;
    private Dictionary<string, ISettingsProvider> _FileProviders = [];

    private TOptions _Current = default!;

    public SettingsContainer() {

    }

    /// <summary>
    /// Atomic creation and assignment of new version after a provider was updated
    /// </summary>
    /// <param name="provider"></param>
    public void OnUpdate(ISettingsProvider provider) {

        Interlocked.Exchange(ref _Current, (TOptions)TOptions.Create(this));

    }

    internal async Task Init() {

        await Reload(); // will be called in startup builder -> shouldn't lead to problems
        _Current = (TOptions)TOptions.Create(this);

    }

    /// <summary>
    /// Reload all providers
    /// </summary>
    /// <returns></returns>
    private async Task Reload() {

        List<Task> tasks = [];

        if(_EnvironmentProvider is not null) {
            tasks.Add(_EnvironmentProvider.Reload());
        }

        foreach(var (key, fileProvider) in _FileProviders) {
            tasks.Add(fileProvider.Reload());
        }

        await Task.WhenAll(tasks);

    }

}
