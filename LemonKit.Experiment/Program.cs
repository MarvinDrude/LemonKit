
using LemonKit.Processors;
using LemonKit.Processors.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test;

[assembly: Procedures(
    typeof(LanguageProcedure<,>)
)]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<A>();

var app = builder.Build();

app.MapGet("/test", (IServiceProvider provider) => {
    return provider.GetRequiredService<A>().Guid;
});

app.Run();


namespace Test {

    public class A {

        public Guid Guid = Guid.NewGuid();

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
            TInput request, 
            CurrentLanguage currentLanuage,
            CancellationToken cancellationToken) {

            cancellationToken.ThrowIfCancellationRequested();

            

            var response = await Next(request, cancellationToken);
            return response;

        }

        public override async Task<TOutput> Process(
            TInput request, 
            IServiceProvider serviceProvider, 
            CancellationToken cancellationToken) {

            var currentLanguage = serviceProvider.GetRequiredService<CurrentLanguage>();

            return await Execute(request, currentLanguage, cancellationToken);

        }

    }

    [Processor()]
    public sealed partial class SignInProcessor 
        : IProcessor<HttpContext, SignInProcessor.Response> {

        private readonly ILogger<SignInProcessor> _Logger;

        public SignInProcessor(
            ILogger<SignInProcessor> logger) {

            _Logger = logger;

        }

        public async Task<Response> Execute(
            HttpContext request,
            CurrentLanguage currentLanuage,
            CancellationToken cancellationToken) {

            cancellationToken.ThrowIfCancellationRequested();



            return new Response();

        }

        public async Task<Response> Process(
            HttpContext request, 
            IServiceProvider serviceProvider, 
            CancellationToken cancellationToken) {

            var currentLanguage = serviceProvider.GetRequiredService<CurrentLanguage>();

            return await Execute(request, currentLanguage, cancellationToken);

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