
using System.Collections;

namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a property that the collection should atleast contain all the provided values
/// </summary>
public sealed class ContainsAttribute : ValidationAttribute
{

    [SuppressMessage("IDE", "IDE0052", Justification = "Used by code generators")]
    private readonly object[]? _Elements;

    /// <summary>
    /// Use a const for comparison
    /// </summary>
    /// <param name="errorCode"></param>
    public ContainsAttribute(
        object[] elements,
        string errorCode = ValidationDefaultCodes.ErrorContains)
    {

        _ErrorCode = errorCode;
        _Elements = elements;

    }

    /// <summary>
    /// Use for a service based value, for example SettingsContainer
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="accessPath"></param>
    /// <param name="errorCode"></param>
    public ContainsAttribute(
        Type serviceType,
        string[] accessPath,
        string errorCode = ValidationDefaultCodes.ErrorContains)
    {

        _Type = serviceType;
        _AccessPath = accessPath;

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T target, object[] elements)
        where T : ICollection
    {

        HashSet<object> elementSet = new(elements);

        if(target.Count < elementSet.Count)
        {
            return false;
        }

        foreach(var item in target)
        {

            elementSet.Remove(item);

            if(elementSet.Count == 0)
            {
                return true;
            }

        }

        return elementSet.Count == 0;

    }

    public static string TemplateError<T>(string errorCodeTemplate, T[] elements)
    {

        return errorCodeTemplate
            .Replace("{ToString}", string.Join(",", elements.Select(x => x?.ToString())));

    }

}
