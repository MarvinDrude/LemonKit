
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

}
