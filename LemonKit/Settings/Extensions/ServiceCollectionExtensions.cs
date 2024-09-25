
namespace LemonKit.Settings.Extensions;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Add a settings container for a specific settings class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configure"></param>
    public static void AddSettingsContainer<T>(this IServiceCollection services, Action<SettingsBuilder<T>> configure)
        where T : class, ISettings {

        var builder = new SettingsBuilder<T>();
        configure.Invoke(builder);

        var container = builder.Build();
        services.AddSingleton(container);

    }

}
