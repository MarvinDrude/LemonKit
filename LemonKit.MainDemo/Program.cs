using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configures and adds a singleton of SettingsContainer<MainSettings>
builder.Services.AddSettingsContainer<MainSettings>((builder) =>
{
    builder
        .AddEnvironmentVariables() // environment variables can only be refreshed by restart of program
        .AddJsonFile<JsonPetSettings>("petSettings.json", JsonSettingsContext.Default); // json files listen to updates to files by default
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.TypeInfoResolver = JsonMainContext.Default;
});

builder.Services.AddCoreObservability(
    serviceName: "MainDemo",
    enableDefaultLocalOtlp: true,
    observerInizialize: () =>
    {
        LemonKit.Observe.LemonKitMainDemo.ObserveContainerExtensions.Init();
    });
builder.Services.AddSerilog((provider, config) =>
{
    config.AddSerilogObservability("MainDemo", provider, onOptions: (options) =>
    {

    });
});

builder.Services
    .AddKitProcessors()
    .AddKitValidators();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseKitProcessorEndpoints();

app.Run();
