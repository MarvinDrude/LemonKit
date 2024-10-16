
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Configures and adds a singleton of SettingsContainer<MainSettings>
builder.Services.AddSettingsContainer<MainSettings>((builder) =>
{
    builder
        .AddEnvironmentVariables() // environment variables can only be refreshed by restart of program
        .AddJsonFile<JsonPetsSettings>("petSettings.json", JsonSettingsContext.Default); // json files listen to updates to files by default
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    //options.SerializerOptions.TypeInfoResolver = JsonMainContext.Default;
});

builder.Services.AddKitProcessors();
builder.Services.AddKitValidators();
builder.Services.AddScoped<IPetService, PetService>();

builder.Services.AddDbContext<MainDbContext>((serviceProvider, dbBuilder) =>
{

    var settings = serviceProvider.GetRequiredService<SettingsContainer<MainSettings>>();
    dbBuilder.UseNpgsql(settings.Current.DatabaseConnectionString);

});

builder.AddOpenTelemetrySample();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseKitProcessorEndpoints();

app.Run();
