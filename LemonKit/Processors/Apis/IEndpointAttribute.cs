
namespace LemonKit.Processors.Apis;

/// <summary>
/// Basic information to register the endpoint to
/// </summary>
public interface IEndpointAttribute {

    /// <summary>
    /// The HTTP method the handler should be registered to
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// The path the handler should be registered to
    /// </summary>
    public string Path { get; }

}
