
namespace LemonKit.Processors.Apis;

/// <summary>
/// Base for all JSON responses
/// </summary>
public class ResponseBase {

    /// <summary>
    /// Indicates the state of the reponse
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Error codes by property names
    /// </summary>
    public Dictionary<string, string[]>? ErrorCodes { get; set; }

    /// <summary>
    /// Additional information in case of 
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Set error codes in builder way
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="errorCodes"></param>
    /// <returns></returns>
    public T SetErrorCodes<T>(Dictionary<string, string[]> errorCodes)
        where T : ResponseBase {

        ErrorCodes = errorCodes;
        return (T)this;

    }

}
