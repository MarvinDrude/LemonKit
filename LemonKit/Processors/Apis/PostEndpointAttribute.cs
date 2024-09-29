
namespace LemonKit.Processors.Apis;

/// <inheritdoc />
public sealed class PostEndpointAttribute([StringSyntax("Route")] string path, string method = "POST")
    : EndpointAttribute(method, path) {

}
