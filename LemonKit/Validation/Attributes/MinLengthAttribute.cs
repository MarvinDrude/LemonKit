
using System.Collections;

namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a <see cref="string"/> or <see cref="ICollection"/> property to have a min length.
/// If nullable, it will return false for <see cref="null"/> 
/// </summary>
public sealed class MinLengthAttribute : ValidationAttribute {

    [SuppressMessage("IDE", "IDE0052", Justification = "Used by code generators")]
    private readonly int _MinLength;

    /// <summary>
    /// Use for a const as min length
    /// </summary>
    /// <param name="minLength">Minimum valid length of a string inclusive</param>
    /// <param name="errorCode">Custom error code</param>
    public MinLengthAttribute(
        int minLength,
        string errorCode = ValidationDefaultCodes.ErrorMinLength) {

        _MinLength = minLength;
        _ErrorCode = errorCode;

    }

    /// <summary>
    /// Use for a service based value, for example SettingsContainer
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="accessPath"></param>
    /// <param name=""></param>
    public MinLengthAttribute(
        Type serviceType,
        string[] accessPath,
        string errorCode = ValidationDefaultCodes.ErrorMinLength) {

        _Type = serviceType;
        _AccessPath = accessPath;

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T? target, int minLength) {

        return target is { } && target switch {

            string str => str.Length >= minLength,
            ICollection coll => coll.Count >= minLength,
            _ => false

        };

    }

    public static string TemplateError(string errorCodeTemplate, int minLength) {

        return errorCodeTemplate
            .Replace("{MinLength}", minLength.ToString());

    }

}