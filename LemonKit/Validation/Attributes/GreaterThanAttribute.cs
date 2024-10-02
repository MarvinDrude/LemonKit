
namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a property that the value needs to be greater than the given value
/// </summary>
public sealed class GreaterThanAttribute : ValidationAttribute {

    [SuppressMessage("IDE", "IDE0052", Justification = "Used by code generators")]
    private readonly object? _Comparison;

    /// <summary>
    /// Use a const for comparison
    /// </summary>
    /// <param name="compare"></param>
    /// <param name="errorCode"></param>
    public GreaterThanAttribute(
        object compare,
        string errorCode = ValidationDefaultCodes.ErrorGreaterThan) {

        _Comparison = compare;
        _ErrorCode = errorCode;

    }

    /// <summary>
    /// Use for a service based value, for example SettingsContainer
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="accessPath"></param>
    /// <param name="errorCode"></param>
    public GreaterThanAttribute(
        Type serviceType,
        string[] accessPath,
        string errorCode = ValidationDefaultCodes.ErrorGreaterThan) {

        _Type = serviceType;
        _AccessPath = accessPath;

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T target, T comparison) {

        return Comparer<T>.Default.Compare(target, comparison) > 0;

    }

    public static string TemplateError<T>(string errorCodeTemplate, T compare) {

        return errorCodeTemplate
            .Replace("{Number}", compare + string.Empty);

    }

}
