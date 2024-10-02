
using LemonKit.Processors;
using LemonKit.Processors.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test;
using LemonKit.Extensions;
using LemonKit.Processors.Apis;

[assembly: Procedures(
    typeof(LanguageProcedure<,>)
)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKitProcessors();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CurrentLanguage>();

var app = builder.Build();
app.UseKitProcessorEndpoints();

Console.WriteLine(app.Services.DisplayPipelines());

app.Run();


namespace Test {



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