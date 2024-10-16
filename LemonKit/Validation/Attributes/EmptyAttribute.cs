
using System.Collections;

namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a <see cref="string"/> / <see cref="ICollection"/> or entity with <see cref="EqualityComparer{T}"/> property to be empty.
/// <para>
/// <see langword="null"/> will count as empty, for strings it uses <see cref="string.IsNullOrWhiteSpace(string?)"/>
/// </para>
/// </summary>
public sealed class EmptyAttribute : ValidationAttribute
{

    public EmptyAttribute(
        string errorCode = ValidationDefaultCodes.ErrorEmpty)
    {

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T? target)
    {

        return target switch
        {

            null => true,
            string str when string.IsNullOrWhiteSpace(str) => true,
            ICollection { Count: 0 } => true,
            _ => EqualityComparer<T>.Default.Equals(target, default)

        };

    }

    public static string TemplateError(string errorCodeTemplate)
    {

        return errorCodeTemplate;

    }

}
