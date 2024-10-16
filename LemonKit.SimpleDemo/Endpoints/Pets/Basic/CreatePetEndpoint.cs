
namespace LemonKit.SimpleDemo.Endpoints.Pets.Basic;

/// <summary>
/// Endpoint responsible 
/// </summary>
[Processor]
[PostEndpoint("/api/pets/create")]
[Observe(
    activitySourceName: "CreatePetEndpoint.Activity",
    meterName: "CreatePetEndpoint.Meter"
)]
public sealed partial class CreatePetEndpoint
{

    /// <summary>
    /// Counter to track pet creations
    /// </summary>
    private static readonly Counter<long> _CreationCounter;

    /// <summary>
    /// Counters need to be defined in static constructor, to make sure that it is ran after the partial static field initializers in other partials
    /// </summary>
    static CreatePetEndpoint()
    {

        _CreationCounter = _Meter.CreateCounter<long>("Pet.Creation");

    }

    /// <summary>
    /// Default logger
    /// </summary>
    private readonly ILogger<CreatePetEndpoint> _Logger;

    /// <summary>
    /// Validator for the request class
    /// </summary>
    private readonly IValidate<Request> _Validator;

    /// <summary>
    /// Main Settings container with the newest version
    /// </summary>
    private readonly SettingsContainer<MainSettings> _MainSettings;

    /// <summary>
    /// Shortcut to always use newest pet settings of main settings
    /// </summary>
    private JsonPetsSettings PetSettings => _MainSettings.Current.PetSettings;

    public CreatePetEndpoint(
        ILogger<CreatePetEndpoint> logger,
        IValidate<Request> validator,
        SettingsContainer<MainSettings> settings)
    {

        _Logger = logger;
        _Validator = validator;
        _MainSettings = settings;

    }

    /// <summary>
    /// Handle incoming requests to create pets
    /// </summary>
    /// <returns></returns>
    public async Task<Response> Execute(
        [Input] Request request,
        HttpContext context,
        IPetService petService,
        CancellationToken cancellationToken)
    {

        using var activity = StartActivity(); // providing no name will default to ClassName.MethodName

        if(cancellationToken.IsCancellationRequested)
        { // request already aborted? early exit
            LogCancel();
            return ResponseBase
                .CreateCancelledResponse(new Response()) // creates a 400 code response with default message
                .ApplyToContext<Response>(context); // applies code to http protocol
        }

        if(_Validator.Validate(request) is { IsValid: false } validation)
        { // request has invalid data
            LogInvalid(request);
            return ResponseBase
                .CreateInvalidRequest(new Response(), validation) // creates a 400 code response with validation errors (Response.ErrorCodes) as response and default message
                .ApplyToContext<Response>(context); // applies code to http protocol
        }

        await petService.Write.Create(Mapper.ToPet(request), cancellationToken);
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
            [nameof(SettingsContainer<MainSettings>.Current), nameof(MainSettings.PetSettings), nameof(JsonPetsSettings.MinPetNameLength)]
        )]
        [MaxLength( // this for example validates that Name.Length <= MainSettings.Current.PetSettings.MaxPetNameLength
            typeof(SettingsContainer<MainSettings>),
            [nameof(SettingsContainer<MainSettings>.Current), nameof(MainSettings.PetSettings), nameof(JsonPetsSettings.MaxPetNameLength)]
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
            { // check if all colors provided are valid hex colors
                result.AddErrorCode(nameof(input.Colors), "E_ONLY_HEX_COLROS");
            }

        }

    }

    /// <summary>
    /// Map request to a new pet model for the database
    /// </summary>
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

    /// <summary>
    /// Response that is sent as json to client
    /// </summary>
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
