namespace LemonKit.MainDemo.Settings;

[Settings]
public sealed partial class MainSettings : ISettings
{
    [EnvironmentVariable("ASPNETCORE_ENVIRONMENT")]
    public required string AspNetEnvironment { get; set; }

    [EnvironmentVariable("DB_CONNECTION_STRING")]
    public required string ConnectionString { get; set; }

    [JsonFile("petSettings.json")]
    public required JsonPetSettings PetSettings { get; set; }
}

public sealed class JsonPetSettings
{
    public required int MaxPetsInDatabase { get; set; }

    public required int MaxPetNameLength { get; set; }
    public required int MinPetNameLength { get; set; }
}