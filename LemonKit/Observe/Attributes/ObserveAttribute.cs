
namespace LemonKit.Observe.Attributes;

/// <summary>
/// Used to mark a partial class that should get activity and optionally 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ObserveAttribute : Attribute {

    private string _ActivitySourceName;
    private string? _MeterName;
    private string? _Version;

    /// <summary>
    /// Create observe code for this class
    /// </summary>
    /// <param name="activitySourceName">Name of the activity source that is generated</param>
    /// <param name="meterName">Name of meter that is generated, only generated if provided non null value</param>
    /// <param name="version">Version used to pass to creation of activity source and meter</param>
    public ObserveAttribute(
        string activitySourceName,
        string? meterName = null,
        string? version = null) {

        _ActivitySourceName = activitySourceName;
        _MeterName = meterName;
        _Version = version;

    }

}
