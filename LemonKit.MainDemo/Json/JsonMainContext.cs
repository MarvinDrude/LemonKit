
using LemonKit.MainDemo.Endpoints.Pets;

namespace LemonKit.MainDemo.Json;

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

[JsonSerializable(typeof(CreatePetEndpoint.Request))]
internal partial class JsonMainContext : JsonSerializerContext
{

}
