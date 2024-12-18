﻿namespace LemonKit.MainDemo.Settings;

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

[JsonSerializable(typeof(JsonPetSettings))]
internal partial class JsonSettingsContext : JsonSerializerContext
{

}
