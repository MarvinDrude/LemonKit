
namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a property that the value should equal a given value
/// </summary>
public sealed class EqualAttribute : ValidationAttribute
{

    [SuppressMessage("IDE", "IDE0052", Justification = "Used by code generators")]
    private readonly object? _Comparison;

    /// <summary>
    /// Use a const for comparison
    /// </summary>
    /// <param name="compare"></param>
    /// <param name="errorCode"></param>
    public EqualAttribute(
        object compare,
        string errorCode = ValidationDefaultCodes.ErrorEqual)
    {

        _Comparison = compare;
        _ErrorCode = errorCode;

    }

    /// <summary>
    /// Use for a service based value, for example SettingsContainer
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="accessPath"></param>
    /// <param name="errorCode"></param>
    public EqualAttribute(
        Type serviceType,
        string[] accessPath,
        string errorCode = ValidationDefaultCodes.ErrorEqual)
    {

        _Type = serviceType;
        _AccessPath = accessPath;

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T target, T comparison)
    {

        return EqualityComparer<T>.Default.Equals(target, comparison);

    }

    public static string TemplateError<T>(string errorCodeTemplate, T compare)
    {

        return errorCodeTemplate
            .Replace("{ToString}", compare + string.Empty);

    }

}

