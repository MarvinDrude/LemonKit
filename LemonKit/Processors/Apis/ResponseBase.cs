
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

    /// <summary>
    /// Apply status code to http protocol context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public T ApplyToContext<T>(HttpContext context)
        where T : ResponseBase {

        context.Response.StatusCode = Code;
        return (T)this;

    }

    /// <summary>
    /// Should be returned if the request was cancelled
    /// </summary>
    /// <returns></returns>
    public static T CreateCancelledResponse<T>()
        where T : ResponseBase {

        return (T)new ResponseBase() {
            Code = 400,
            ErrorMessage = "[ABORT] Request was cancelled."
        };

    }

    /// <summary>
    /// Should be returned if the validator returned false for given request data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateInvalidRequest<T>(Dictionary<string, string[]>? errorCodes = null)
        where T : ResponseBase {

        return (T)new ResponseBase() {
            Code = 400,
            ErrorMessage = "[INVALID] Request data is not valid.",
            ErrorCodes = errorCodes
        };

    }

    /// <summary>
    /// Should be returned if the validator returned false for given request data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static T CreateInvalidRequest<T>(ValidationResult result)
        where T : ResponseBase {

        return CreateInvalidRequest<T>(result.MaterializedErrorCodes);

    }

}
