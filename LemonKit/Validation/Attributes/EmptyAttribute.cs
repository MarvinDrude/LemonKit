
using System.Collections;

namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a <see cref="string"/> / <see cref="ICollection"/> or entity with <see cref="EqualityComparer{T}"/> property to be empty.
/// <para>
/// <see langword="null"/> will count as empty, for strings it uses <see cref="string.IsNullOrWhiteSpace(string?)"/>
/// </para>
/// </summary>
public sealed class EmptyAttribute : ValidationAttribute {

    public EmptyAttribute(
        string errorCode = ValidationDefaultCodes.ErrorEmpty) {

        _ErrorCode = errorCode;

    }

    /// <summary>
    /// Use for a service based value, for example SettingsContainer
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="accessPath"></param>
    /// <param name="errorCode"></param>
    public EmptyAttribute(
        Type serviceType,
        string[] accessPath,
        string errorCode = ValidationDefaultCodes.ErrorEmpty) {

        _Type = serviceType;
        _AccessPath = accessPath;

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T? target) {

        return target switch {

            null => true,
            string str when string.IsNullOrWhiteSpace(str) => true,
            ICollection { Count: 0 } => true,
            _ => EqualityComparer<T>.Default.Equals(target, default)

        };

    }

    public static string TemplateError(string errorCodeTemplate) {

        return errorCodeTemplate;

    }

}
