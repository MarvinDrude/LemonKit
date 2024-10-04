
namespace LemonKit.SimpleDemo.Endpoints.Pets.Basic;

/// <summary>
/// Endpoint responsible 
/// </summary>
[Processor]
[PostEndpoint("/api/pets/create")]
public sealed partial class CreatePetEndpoint {

    /// <summary>
    /// Default logger
    /// </summary>
    private readonly ILogger<CreatePetEndpoint> _Logger;

    /// <summary>
    /// Validator for the request class
    /// </summary>
    private readonly IValidate<Request> _Validator;

    public CreatePetEndpoint(
        ILogger<CreatePetEndpoint> logger,
        IValidate<Request> validator) {

        _Logger = logger;
        _Validator = validator;

    }

    /// <summary>
    /// Handle incoming requests to create pets
    /// </summary>
    /// <returns></returns>
    public async Task<Response> Execute(
        [Input] Request request,
        HttpContext context,
        CancellationToken cancellationToken) {

        if(cancellationToken.IsCancellationRequested) {
            LogCancel();
            return ResponseBase
                .CreateCancelledResponse<Response>()
                .ApplyToContext<Response>(context);
        }

        if(_Validator.Validate(request) is { IsValid: false } validation) {
            LogInvalid(request);
            return ResponseBase
                .CreateInvalidRequest<Response>(validation)
                .ApplyToContext<Response>(context);
        }



        return new Response();

    }

    [Validate]
    public sealed class Request {

        [MinLength(2)]
        public required List<string> Colors { get; set; }

        [MinLength(
            typeof(SettingsContainer<MainSettings>),
            [nameof(SettingsContainer<MainSettings>.Current), nameof(MainSettings.PetSettings), nameof(JsonPetsSettings.MinPetNameLength)]
        )]
        [MaxLength(
            typeof(SettingsContainer<MainSettings>),
            [nameof(SettingsContainer<MainSettings>.Current), nameof(MainSettings.PetSettings), nameof(JsonPetsSettings.MaxPetNameLength)]
        )]
        public required string Name { get; set; }



        /// <summary>
        /// If you need more than the attributes, additional logic can go in here.
        /// All parameters beside <see cref="ValidationResult"/> and <see cref="Request"/> are taken from ServiceProviders (only singletons should be injected)
        /// </summary>
        public static void ExtraValidate(
            ValidationResult result,
            Request input,
            SettingsContainer<MainSettings> settings) {



        }

    }

    public sealed class Response : ResponseBase {

        //public required Guid Id { get; set; }

    }

    /// <summary>
    /// Log method section
    /// </summary>
    [LoggerMessage(0, LogLevel.Information, "Request was cancelled before execution.")]
    partial void LogCancel();

    [LoggerMessage(0, LogLevel.Information, "Request object is invalidated: {Request}.")]
    partial void LogInvalid(Request request);

}
