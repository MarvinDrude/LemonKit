﻿
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

    /// <summary>
    /// Main Settings container with the newest version
    /// </summary>
    private readonly SettingsContainer<MainSettings> _MainSettings;

    /// <summary>
    /// Shortcut to use always newest pet settings of main settings
    /// </summary>
    private JsonPetsSettings _PetSettings => _MainSettings.Current.PetSettings;

    public CreatePetEndpoint(
        ILogger<CreatePetEndpoint> logger,
        IValidate<Request> validator,
        SettingsContainer<MainSettings> settings) {

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
        CancellationToken cancellationToken) {

        if(cancellationToken.IsCancellationRequested) { // request already aborted? early exit
            LogCancel();
            return ResponseBase
                .CreateCancelledResponse<Response>() // creates a 400 code response with default message
                .ApplyToContext<Response>(context); // applies code to http protocol
        }

        if(_Validator.Validate(request) is { IsValid: false } validation) { // request has invalid data
            LogInvalid(request);
            return ResponseBase
                .CreateInvalidRequest<Response>(validation) // creates a 400 code response with validation errors (Response.ErrorCodes) as response and default message
                .ApplyToContext<Response>(context); // applies code to http protocol
        }



        return new Response();

    }

    [Validate] // generates a class that implements IValidate<Request> and is registered with AddKitValidators to services
    public sealed class Request {

        [MinLength(2)] // use validate attributes with constant values
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



        /// <summary>
        /// If you need more than the attributes, additional logic can go in here. (usually if there is more sophisticated validation needed)
        /// All parameters beside <see cref="ValidationResult"/> and <see cref="Request"/> are taken from ServiceProviders (only singletons should be injected)
        /// </summary>
        public static void ExtraValidate(
            ValidationResult result,
            Request input,
            SettingsContainer<MainSettings> settings) {

            if(!input.Colors.AreHexColors()) { // check if all colors provided are valid hex colors
                result.AddErrorCode(nameof(input.Colors), "E_ONLY_HEX_COLROS");
            }

        }

    }

    /// <summary>
    /// Response that is sent as json to client
    /// </summary>
    public sealed class Response : ResponseBase {



    }

    /// <summary>
    /// Log method section
    /// </summary>
    [LoggerMessage(0, LogLevel.Information, "Request was cancelled before execution.")]
    partial void LogCancel();

    [LoggerMessage(0, LogLevel.Information, "Request object is invalidated: {Request}.")]
    partial void LogInvalid(Request request);

}
