
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

        [Contains(["#000000"])]
        public required List<string> Colors { get; set; }

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
