
using LemonKit.Processors;
using LemonKit.Processors.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test;
using LemonKit.Extensions;

[assembly: Procedures(
    typeof(LanguageProcedure<,>)
)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKitProcessors();
builder.Services.AddScoped<CurrentLanguage>();

builder.Services.AddSingleton<IA, A>();

var app = builder.Build();

var procTest = app.Services.GetRequiredService<SignInProcessor>();

app.MapGet("/test", procTest.BuildProcess(app.Services));

app.Run();


namespace Test {

    public class A : IA {

        public Guid Guid = Guid.NewGuid();

    }

    public interface IA {

    }

    public class B { }

    [Procedure()]
    public sealed partial class LanguageProcedure<TInput, TOutput>
        : Procedure<TInput, TOutput>
        where TInput : HttpContext {

        private readonly ILogger<LanguageProcedure<TInput, TOutput>> _Logger;

        public LanguageProcedure(
            ILogger<LanguageProcedure<TInput, TOutput>> logger,
            IA a) {

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

            Console.WriteLine("C: " + currentLanuage.Code);

            return new Response();

        }

        public sealed class Request {



        }

        public sealed class Response {



        }

    }

    public sealed class CurrentLanguage {

        public required string Code { get; set; }

    }

}