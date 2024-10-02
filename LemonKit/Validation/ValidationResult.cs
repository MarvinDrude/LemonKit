
namespace LemonKit.Validation;

/// <summary>
/// Represents the result of a validation request
/// </summary>
public sealed class ValidationResult {

    /// <summary>
    /// Dictionary of all error codes by property name
    /// </summary>
    private readonly Dictionary<string, ValidationErrorCodes> _ErrorCodes = [];

    /// <summary>
    /// Whether Validation was valid or invalid
    /// </summary>
    public bool IsValid => _ErrorCodes is { Count: 0 };

    /// <summary>
    /// All error codes by property name
    /// </summary>
    public Dictionary<string, ValidationErrorCodes> ErrorCodes => _ErrorCodes;

    /// <summary>
    /// Get copy of error codes with fixed length for responses for example
    /// </summary>
    public Dictionary<string, string[]> MaterializedErrorCodes => _ErrorCodes.ToDictionary(x => x.Key, x => x.Value.ToArray());

    /// <summary>
    /// Adds new error code to specific property name
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="errorCode"></param>
    public void AddErrorCode(
        string propertyName,
        string errorCode) {

        if(!_ErrorCodes.TryGetValue(propertyName, out var codes)) {
            codes = _ErrorCodes[propertyName] = [];
        }

        codes.Add(errorCode);

    }

    /// <summary>
    /// Set all error codes for specific property name
    /// </summary>
    /// <param name="propertyName"></param>
    public void SetErrorCodes(
        string propertyName,
        ValidationErrorCodes errorCodes) {

        _ErrorCodes[propertyName] = errorCodes;

    }

}

/// <summary>
/// List of all error codes for specific property
/// </summary>
public sealed class ValidationErrorCodes : HashSet<string> {

}

