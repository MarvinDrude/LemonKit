
namespace LemonKit.Processors.Apis;

/// <inheritdoc />
public sealed class DeleteEndpointAttribute([StringSyntax("Route")] string path, string method = "DELETE")
    : EndpointAttribute(method, path) {

}
