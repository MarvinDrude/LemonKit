namespace LemonKit.SimpleDemo.Settings;

[Settings]
public sealed partial class MainSettings : ISettings {

    [EnvironmentVariable("ASPNETCORE_ENVIRONMENT")]
    public required string AspNetEnvironment { get; set; }

    [JsonFile("petSettings.json")]
    public required JsonPetsSettings PetSettings { get; set; }

}

public sealed class JsonPetsSettings {

    public required int MaxPetsInDatabase { get; set; }

    public required int MaxPetNameLength { get; set; }
    public required int MinPetNameLength { get; set; }

}
