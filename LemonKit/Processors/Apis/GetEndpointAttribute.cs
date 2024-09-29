
namespace LemonKit.Processors.Apis;

/// <inheritdoc />
public sealed class GetEndpointAttribute([StringSyntax("Route")] string path, string method = "GET") 
    : EndpointAttribute(method, path) {

}
