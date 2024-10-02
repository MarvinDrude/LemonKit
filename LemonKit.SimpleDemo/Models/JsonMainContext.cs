using LemonKit.SimpleDemo.Endpoints.Pets.Basic;

namespace LemonKit.SimpleDemo.Models;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonElement,
    AllowTrailingCommas = true,
    IgnoreReadOnlyFields = true,
    UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(ResponseBase))]

[JsonSerializable(typeof(CreatePetEndpoint.Request))]
[JsonSerializable(typeof(CreatePetEndpoint.Response))]
internal partial class JsonMainContext : JsonSerializerContext {

}
