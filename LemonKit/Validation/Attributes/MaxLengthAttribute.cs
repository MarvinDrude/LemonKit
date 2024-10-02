
using System.Collections;

namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a <see cref="string"/> or <see cref="ICollection"/> property to have a max length.
/// If nullable, it will return false for <see cref="null"/> 
/// </summary>
public sealed class MaxLengthAttribute : ValidationAttribute {

    [SuppressMessage("IDE", "IDE0052", Justification = "Used by code generators")]
    private readonly int _MaxLength;

    /// <summary>
    /// Use for a const as max length
    /// </summary>
    /// <param name="maxLength">Maximum valid length of a string inclusive</param>
    /// <param name="errorCode">Custom error code</param>
    public MaxLengthAttribute(
        int maxLength,
        string errorCode = ValidationDefaultCodes.ErrorMaxLength) {

        _MaxLength = maxLength;
        _ErrorCode = errorCode;

    }

    /// <summary>
    /// Use for a service based value, for example SettingsContainer
    /// </summary>
    /// <param name="serviceType">Needs to be present in DI Container</param>
    /// <param name="accessPath">Type.Access.Path.To.Variable</param>
    public MaxLengthAttribute(
        Type serviceType,
        string[] accessPath,
        string errorCode = ValidationDefaultCodes.ErrorMaxLength) {

        _Type = serviceType;
        _AccessPath = accessPath;

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T? target, int maxLength) {

        return target is { } && target switch {

            string str => str.Length <= maxLength,
            ICollection coll => coll.Count <= maxLength,
            _ => false

        };

    }

    public static string TemplateError(string errorCodeTemplate, int maxLength) {

        return errorCodeTemplate
            .Replace("{MaxLength}", maxLength.ToString());

    }

}

