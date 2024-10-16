
namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a <see cref="string"/> / <see cref="ICollection"/> or entity with <see cref="EqualityComparer{T}"/> property to not be empty.
/// <para>
/// <see langword="null"/> will count as empty, for strings it uses <see cref="string.IsNullOrWhiteSpace(string?)"/>
/// </para>
/// </summary>
public sealed class NotEmptyAttribute : ValidationAttribute
{

    public NotEmptyAttribute(
        string errorCode = ValidationDefaultCodes.ErrorNotEmpty)
    {

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T? target)
    {

        return !EmptyAttribute.Validate(target);

    }

    public static string TemplateError(string errorCodeTemplate)
    {

        return errorCodeTemplate;

    }

}
