
namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private static void RenderProcedure(
        SourceProductionContext context,
        ProcedureClassInfo? procedureInfo) {

        var token = context.CancellationToken;
        if(procedureInfo is not { } procedure) {
            return;
        }

        token.ThrowIfCancellationRequested();
        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using LemonKit.Processors;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");

        if(procedure.ClassInfo.NameSpace is { } nameSpace) {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace};");

        }
        cw.WriteLine();

        cw.WriteLine($"{ClassInfoBuilder.GetClassString(procedure.ClassInfo, true)}<TInput, TOutput> : Procedure<TInput, TOutput> {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"public override async Task<{procedure.OutputType.Name}> Process(");
        cw.UpIndent();
        cw.WriteLine($"{procedure.InputType.Name} request,");
        cw.WriteLine($"IServiceProvider serviceProvider,");
        cw.WriteLine($"CancellationToken cancellationToken) {{");

        cw.WriteLine();
        List<string> parameterNames = [];

        foreach(var parameter in procedure.Parameters) {

            if(parameter.Type is "global::System.IServiceProvider") {
                parameterNames.Add("serviceProvider");
                continue;
            }
            if(parameter.Type is "global::System.Threading.CancellationToken") {
                parameterNames.Add("cancellationToken");
                continue;
            }
            if(parameter.Type == procedure.InputType.Name) {
                parameterNames.Add("request");
                continue;
            }

            cw.WriteLine($"var param_{parameter.Name} = serviceProvider.GetRequiredService<{parameter.Type}>();");
            parameterNames.Add($"param_{parameter.Name}");

        }

        cw.WriteLine();
        cw.WriteLine($"return await Execute({string.Join(", ", parameterNames)});");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{procedure.ClassInfo.NameSpace ?? "Global"}.{procedure.ClassInfo.Name}.g.cs", cw.ToString());

    }

    private static void RenderExtensions(
        SourceProductionContext context,
        ImmutableArray<ProcessorInfo?> processors,
        ImmutableArray<EquatableArray<ProcedureInfo>?> procedures) {

        var token = context.CancellationToken;

        token.ThrowIfCancellationRequested();
        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using System.Text;");
        cw.WriteLine($"using LemonKit.Processors;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");

        cw.WriteLine();
        cw.WriteLine($"namespace LemonKit.Extensions;");
        cw.WriteLine();

        cw.WriteLine($"internal static class IServiceCollectionExtensions {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"public static IServiceCollection AddKitProcessors(this IServiceCollection collection) {{");
        cw.UpIndent();
        cw.WriteLine();

        List<ProcessorInfo> apis = [];
        foreach(var processorInfo in processors) {

            if(processorInfo is not { } processor) {
                continue;
            }

            cw.WriteLine($"collection.AddSingleton<{processor.ClassInfo.FullTypeName}>();");

            if(processor.ApiInfo is not null) {
                apis.Add(processor);
            }

        }

        cw.WriteLine();
        cw.WriteLine($"return collection;");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.WriteLine($"public static string DisplayPipelines(this IServiceProvider serviceProvider) {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"var sb = new StringBuilder();");

        foreach(var processorInfo in processors) {

            if(processorInfo is not { } processor) {
                continue;
            }

            cw.WriteLine($"sb.Append({processor.ClassInfo.FullTypeName}.DisplayPipeline());");
            cw.WriteLine($"sb.AppendLine();");
            cw.WriteLine();

        }

        cw.WriteLine();
        cw.WriteLine($"return sb.ToString();");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{"LemonKit.Extensions"}.{"IServiceCollectionExtensions"}.g.cs", cw.ToString());

        if(apis is { Count: > 0 }) {
            RenderApiExtensions(context, apis, token);
        } else {
            context.AddSource($"{"LemonKit.Extensions"}.{"WebApplicationExtensions"}.g.cs", string.Empty);
        }

    }

    private static void RenderApiExtensions(
        SourceProductionContext context,
        List<ProcessorInfo> processors,
        CancellationToken token) {

        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using LemonKit.Processors;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");
        cw.WriteLine($"using Microsoft.AspNetCore.Builder;");

        cw.WriteLine();
        cw.WriteLine($"namespace LemonKit.Extensions;");
        cw.WriteLine();

        cw.WriteLine($"public static class WebApplicationExtensions {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"public static void UseKitProcessorEndpoints(this WebApplication app) {{");
        cw.UpIndent();
        cw.WriteLine();

        foreach(var processor in processors) {

            if(processor.ApiInfo is not { }) {
                continue;
            }

            cw.WriteLine($"{processor.ClassInfo.FullTypeName}.UseMinimalEndpoint(app);");

        }

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{"LemonKit.Extensions"}.{"WebApplicationExtensions"}.g.cs", cw.ToString());

    }

}
