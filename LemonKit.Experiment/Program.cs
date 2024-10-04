
using LemonKit.Processors;
using LemonKit.Processors.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test;
using LemonKit.Extensions;
using LemonKit.Processors.Apis;
using LemonKit.Validation.Attributes;
using LemonKit.Validation;

[assembly: Procedures(
    typeof(LanguageProcedure<,>)
)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKitProcessors();
builder.Services.AddKitValidators();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CurrentLanguage>();
builder.Services.AddSingleton<TestValidationHelper>();

var app = builder.Build();
app.UseKitProcessorEndpoints();

var validate = app.Services.GetRequiredService<IValidate<TestValidation>>();
var result = validate.Validate(new TestValidation() {
    Contains = ["a", "b", "c", "d"],
    NotContains = ["a", "b", "c", "d"],
    Enum = TestValidationEnum.B,
    Empty = [],
    Equal = 2,
    GreaterThan = 21,
    GreaterThanOrEqual = 20,
    LessThan = 10,
    LessThanOrEqual = 10,
    MaxLength = "adsa",
    MinLength = new Dictionary<string, string>() {
        ["adsa"] = "a",
        ["adsa1"] = "a",
        ["adsa2"] = "a"
    },
    NotEmpty = ["dsadsa"],
    NotEqual = "bba"
});

Console.WriteLine(app.Services.DisplayPipelines());

app.Run();


namespace Test {

    [Validate]
    public sealed class TestValidation {

        [Contains(["a", "b"])]
        [Contains(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.Contains)]
        )]
        public required List<string> Contains { get; set; }

        [NotContains(["g", "h"])]
        [NotContains(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.NotContains)]
        )]
        public required List<string> NotContains { get; set; }

        [EnumValue]
        public required TestValidationEnum Enum { get; set; }

        [Equal(2)]
        [Equal(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.Equal)]
        )]
        public required int Equal { get; set; }

        [NotEqual("test")]
        [NotEqual(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.NotEqual)]
        )]
        public required string NotEqual { get; set; }

        [Empty]
        public required string[] Empty { get; set; }

        [NotEmpty]
        public required string[] NotEmpty { get; set; }

        [GreaterThan(20d)]
        [GreaterThan(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.GreaterThan)]
        )]
        public required double GreaterThan { get; set; }

        [GreaterThanOrEqual(20d)]
        [GreaterThanOrEqual(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.GreaterThanOrEqual)]
        )]
        public required double GreaterThanOrEqual { get; set; }

        [LessThan(20f)]
        [LessThan(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.LessThan)]
        )]
        public required float LessThan { get; set; }

        [LessThanOrEqual(20f)]
        [LessThanOrEqual(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.LessThanOrEqual)]
        )]
        public required float LessThanOrEqual { get; set; }

        [MaxLength(20)]
        [MaxLength(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.MaxLength)]
        )]
        public required string MaxLength { get; set; }

        [MinLength(2)]
        [MinLength(
            typeof(TestValidationHelper),
            [nameof(TestValidationHelper.MinLength)]
        )]
        public required Dictionary<string, string> MinLength { get; set; }

        public static void ExtraValidate(
            ValidationResult result,
            TestValidation input,
            IServiceProvider provider) {

            

        }

    }

    public sealed class TestValidationHelper {

        public object[] Contains { get; set; } = ["c", "d"];

        public object[] NotContains { get; set; } = ["g", "j"];

        public int Equal { get; set; } = 2;

        public string NotEqual { get; set; } = "bb";

        public int GreaterThan { get; set; } = 20;

        public int GreaterThanOrEqual { get; set; } = 20;

        public int LessThan { get; set; } = 20;

        public int LessThanOrEqual { get; set; } = 20;

        public int MaxLength { get; set; } = 200;

        public int MinLength { get; set; } = 3;

    }

    public enum TestValidationEnum {

        A = 1,
        B = 2

    }

    [Procedure()]
    public sealed partial class LanguageProcedure<TInput, TOutput>
        : Procedure<TInput, TOutput>
        where TInput : HttpContext {

        private readonly ILogger<LanguageProcedure<TInput, TOutput>> _Logger;

        public LanguageProcedure(
            ILogger<LanguageProcedure<TInput, TOutput>> logger) {

            _Logger = logger;

        }

        public async Task<TOutput> Execute(
            [Input] TInput request, 
            CurrentLanguage currentLanuage,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken) {

            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine("A");

            var response = await Next(request, serviceProvider, cancellationToken);
            return response;

        }

    }

    [Procedure()]
    public sealed partial class LanguageTwoProcedure<TInput, TOutput>
        : Procedure<TInput, TOutput>
        where TInput : HttpContext {

        private readonly ILogger<LanguageTwoProcedure<TInput, TOutput>> _Logger;

        public LanguageTwoProcedure(
            ILogger<LanguageTwoProcedure<TInput, TOutput>> logger) {

            _Logger = logger;

        }

        public async Task<TOutput> Execute(
            [Input] TInput request,
            CurrentLanguage currentLanuage,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken) {

            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine("B");
            currentLanuage.Code = "ba";

            var response = await Next(request, serviceProvider, cancellationToken);
            return response;

        }

    }

    [Processor()]
    [GetEndpoint("/testa")]
    [Procedures(
        typeof(LanguageTwoProcedure<,>)    
    )]
    public sealed partial class SignInProcessor {

        private readonly ILogger<SignInProcessor> _Logger;

        public SignInProcessor(
            ILogger<SignInProcessor> logger) {

            _Logger = logger;

        }

        public async Task<Response> Execute(
            [Input] HttpContext request,
            CurrentLanguage currentLanuage,
            CancellationToken cancellationToken) {

            cancellationToken.ThrowIfCancellationRequested();

            LogTest();

            Console.WriteLine("C: " + currentLanuage.Code);

            return new Response();

        }

        public void Configure(IEndpointConventionBuilder endpoint) =>
            endpoint
                .WithDisplayName("CreateCard")
                .WithDescription("Creates a new card with given properties");

        public sealed class Request {



        }

        public sealed class Response : ResponseBase {



        }

        [LoggerMessage(0, LogLevel.Information, "Here is a test log")]
        partial void LogTest();

    }

    public sealed class CurrentLanguage {

        public required string Code { get; set; }

    }


    public class Module {

    }

}