
namespace LemonKit.Processors.Apis;

/// <inheritdoc cref="IEndpointAttribute" />
[AttributeUsage(AttributeTargets.Class)]
public class EndpointAttribute(
    string method, 
    [StringSyntax("Route")] string path) 

    : Attribute, IEndpointAttribute {

    /// <inheritdoc />
    public string Method { get; } = method;

    /// <inheritdoc />
    public string Path { get; } = path;

}
