using LemonKit.Validation;
using LemonKit.Validation.Attributes;

namespace LemonKit.MainDemo.Endpoints.Pets;

[Processor]
[PostEndpoint("/api/pets/create")]
[Observe(
    activitySourceName: "CreatePetEndpoint.Activity",
    meterName: "CreatePetEndpoint.Meter"
)]
public sealed partial class CreatePetEndpoint
{
    private static readonly Counter<long> _CreationCounter;

    static CreatePetEndpoint()
    {
        _CreationCounter = _Meter.CreateCounter<long>("Pet.Creation");
    }

    private readonly ILogger<CreatePetEndpoint> _Logger;
    private readonly IValidate<Request> _Validator;
    private readonly SettingsContainer<MainSettings> _MainSettings;

    private JsonPetSettings PetSettings => _MainSettings.Current.PetSettings;

    public CreatePetEndpoint(
        ILogger<CreatePetEndpoint> logger,
        IValidate<Request> validator,
        SettingsContainer<MainSettings> settings)
    {
        _Logger = logger;
        _Validator = validator;
        _MainSettings = settings;
    }

    public async Task<Response> Execute(
        [Input] Request request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        using var activity = StartActivity();

        if(cancellationToken.IsCancellationRequested)
        {
            LogCancel();
            return ResponseBase
                .CreateCancelledResponse(new Response())
                .ApplyToContext<Response>(context);
        }

        if(_Validator.Validate(request) is { IsValid: false } validation)
        {
            LogInvalid(request);
            return ResponseBase
                .CreateInvalidRequest(new Response(), validation)
                .ApplyToContext<Response>(context);
        }

        Pet toCreate = Mapper.ToPet(request);

        _CreationCounter.Add(1);

        return new Response()
        {
            Code = 200
        };

    }

    [Validate] // generates a class that implements IValidate<Request> and is registered with AddKitValidators to services
    public sealed class Request : RequestBase
    {

        [MinLength(1)] // use validate attributes with constant values
        public required List<string> Colors { get; set; }

        [MinLength( // use validate attributes with values based on services available in IServiceProvider
            typeof(SettingsContainer<MainSettings>),
            [nameof(SettingsContainer<MainSettings>.Current), nameof(MainSettings.PetSettings), nameof(JsonPetSettings.MinPetNameLength)]
        )]
        [MaxLength( // this for example validates that Name.Length <= MainSettings.Current.PetSettings.MaxPetNameLength
            typeof(SettingsContainer<MainSettings>),
            [nameof(SettingsContainer<MainSettings>.Current), nameof(MainSettings.PetSettings), nameof(JsonPetSettings.MaxPetNameLength)]
        )]
        public required string Name { get; set; }

        [GreaterThanOrEqual(1)]
        [LessThan(1800)]
        public required float Height { get; set; }

        /// <summary>
        /// If you need more than the attributes, additional logic can go in here. (usually if there is more sophisticated validation needed)
        /// All parameters beside <see cref="ValidationResult"/> and <see cref="Request"/> are taken from ServiceProviders (only singletons should be injected)
        /// </summary>
        public static void ExtraValidate(
            ValidationResult result,
            Request input,
            SettingsContainer<MainSettings> settings)
        {
            if(!input.Colors.AreHexColors())
            {
                result.AddErrorCode(nameof(input.Colors), "E_ONLY_HEX_COLROS");
            }
        }

    }

    public static class Mapper
    {

        public static Pet ToPet(Request request)
        {

            return new Pet()
            {
                Id = Guid.NewGuid(),
                Colors = request.Colors,
                Height = request.Height,
                Name = request.Name
            };

        }

    }

    public sealed class Response : ResponseBase
    {

    }

    /// <summary>
    /// Log method section
    /// </summary>
    [LoggerMessage(0, LogLevel.Information, "Request was cancelled before execution.")]
    partial void LogCancel();

    [LoggerMessage(0, LogLevel.Information, "Request object is invalidated: {Request}.")]
    partial void LogInvalid(Request request);
}
