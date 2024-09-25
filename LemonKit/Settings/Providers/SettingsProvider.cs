
namespace LemonKit.Settings.Providers;

/// <inheritdoc cref="ISettingsProvider" />
public abstract class SettingsProvider : ISettingsProvider {

    /// <summary>
    /// The container this provider is created in
    /// </summary>
    public required ISettingsContainer _SettingsContainer { get; init; }

    /// <inheritdoc cref="ISettingsProvider.Reload" />
    public abstract Task Reload();

    /// <summary>
    /// Call this to notify change of data
    /// </summary>
    protected void OnUpdate() {

        _SettingsContainer.OnUpdate(this);

    }

}
