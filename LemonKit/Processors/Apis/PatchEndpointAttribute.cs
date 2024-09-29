
namespace LemonKit.Processors.Apis;

/// <inheritdoc />
public sealed class PatchEndpointAttribute([StringSyntax("Route")] string path, string method = "PATCH")
    : EndpointAttribute(method, path) {

}