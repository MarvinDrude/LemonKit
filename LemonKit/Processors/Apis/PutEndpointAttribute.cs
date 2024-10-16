
namespace LemonKit.Processors.Apis;

/// <inheritdoc />
public sealed class PutEndpointAttribute([StringSyntax("Route")] string path, string method = "PUT")
    : EndpointAttribute(method, path)
{

}
