
namespace LemonKit.Observe;

/// <summary>
/// Handles automatic generated meter and acitivity source creation and holds them in order to register them at startup
/// </summary>
public static class ObserveContainer {

    /// <summary>
    /// List of all acitvity sources created by this container
    /// </summary>
    private static readonly List<ActivitySource> _ActivitySources = [];

    /// <summary>
    /// List of all meters created by this container
    /// </summary>
    private static readonly List<Meter> _Meters = [];

    /// <summary>
    /// Create a new activity source and add it to the internal list
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static ActivitySource CreateActivitySource(
        string name,
        string version) {

        var source = new ActivitySource(name, version);
        _ActivitySources.Add(source);

        return source;

    }

    /// <summary>
    /// Create a new meter and add it to the internal list
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static Meter CreateMeter(
        string name,
        string version) {

        var meter = new Meter(name, version);
        _Meters.Add(meter);

        return meter;

    }

    /// <summary>
    /// Get all names of the activity sources
    /// </summary>
    public static IEnumerable<string> ActivitySourceNames {
        get {
            foreach(var aSource in _ActivitySources) {
                yield return aSource.Name;
            }
        }
    }

    /// <summary>
    /// Get all names of the meters
    /// </summary>
    public static IEnumerable<string> MeterNames {
        get {
            foreach(var meter in _Meters) {
                yield return meter.Name;
            }
        }
    }

    /// <summary>
    /// Initialize all static fields of created meters and activity sources. (this is required before adding them to configuration)
    /// </summary>
    public static void Initialize() {

        if(Assembly.GetEntryAssembly() is not { } assembly ||
            assembly
                .GetTypes()
                .FirstOrDefault(x => x.Name == "ObserveContainerExtensions") is not { } type) {

            return;

        }

        if(type.GetMethod("Init", BindingFlags.Static | BindingFlags.Public) is not { } method) {
            return;
        }

        method.Invoke(null, []);

    }

}
