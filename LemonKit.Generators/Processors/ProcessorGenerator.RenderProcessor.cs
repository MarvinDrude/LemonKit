
namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private static void RenderProcessor(
        SourceProductionContext context,
        ProcessorInfo? processorInfo,
        ImmutableArray<EquatableArray<ProcedureInfo>?> assemblyProcedureInfos) {

        var token = context.CancellationToken;
        if(processorInfo is not { } processor) {
            return;
        }

        List<ProcedureInfo> allProcs = [];

        if(processor.UseAssemblyProcedures) {

            foreach(var procInfos in assemblyProcedureInfos) {

                if(procInfos is null) {
                    continue;
                }

                allProcs.AddRange([..procInfos]);

            }

        }

        allProcs.AddRange(processor.Procedures ?? []);
        var procs = GetValidProcedures(processor, allProcs);

        token.ThrowIfCancellationRequested();
        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using System.Text;");
        cw.WriteLine($"using LemonKit.Processors;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");

        if(processor.ApiInfo is not null) {
            cw.WriteLine($"using Microsoft.AspNetCore.Builder;");
        }

        if(processor.ClassInfo.NameSpace is { } nameSpace) {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace};");

        }
        cw.WriteLine();

        cw.WriteLine($"{ClassInfoBuilder.GetClassString(processor.ClassInfo, true)}");
        cw.UpIndent();
        cw.WriteLine($": IProcessor<{processor.InputType.Name}, {processor.OutputType.Name}> {{");

        cw.WriteLine();

        cw.WriteLine($"public async Task<{processor.OutputType.Name}> Process(");
        cw.UpIndent();
        cw.WriteLine($"{processor.InputType.Name} request,");
        cw.WriteLine($"IServiceProvider serviceProvider,");
        cw.WriteLine($"CancellationToken cancellationToken) {{");

        cw.WriteLine();
        List<string> parameterNames = [];

        foreach(var parameter in processor.Parameters) {

            if(parameter.Type is "global::System.IServiceProvider") {
                parameterNames.Add("serviceProvider");
                continue;
            }
            if(parameter.Type is "global::System.Threading.CancellationToken") {
                parameterNames.Add("cancellationToken");
                continue;
            }
            if(parameter.Type == processor.InputType.Name) {
                parameterNames.Add("request");
                continue;
            }
            if(parameter.Type is "global::Microsoft.AspNetCore.Http.HttpContext") {

                cw.WriteLine($"var param_{parameter.Name} = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().HttpContext!;");
                parameterNames.Add($"param_{parameter.Name}");

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
        cw.WriteLine($"public Func<{processor.InputType.Name}, IServiceProvider, CancellationToken, Task<{processor.OutputType.Name}>> BuildProcess(IServiceProvider serviceProvider) {{");
        cw.UpIndent();
        cw.WriteLine();

        for(int e = 0; e < procs.Count; e++) {

            var proc = procs[e];

            cw.WriteLine($"{proc.TypeNoGenerics}<{processor.InputType.Name}, {processor.OutputType.Name}> proc_{e}");
            cw.UpIndent();
            cw.WriteLine($"= new({string.Join(", ", proc.ConstructorParameters.Select(x => $"serviceProvider.GetRequiredService<{x.Type.Replace("<TInput", $"<{processor.InputType.Name}").Replace("TOutput>", $"{processor.OutputType.Name}>")}>()"))});");
            cw.DownIndent();
            cw.WriteLine();

        }

        for(int e = 0; e < procs.Count - 1; e++) {

            cw.WriteLine($"proc_{e}.SetNextFunc(proc_{(e + 1)}.Process);");

        }
        if(procs.Count > 0) {
            cw.WriteLine($"proc_{(procs.Count - 1)}.SetNextFunc(Process);");
        }

        cw.WriteLine();
        cw.WriteLine($"return {(procs.Count == 0 ? "Process" : "proc_0.Process")};");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.WriteLine($"public static string DisplayPipeline() {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"var sb = new StringBuilder();");
        cw.WriteLine($"sb.AppendLine(\"{processor.ClassInfo.Name}\");");
        cw.WriteLine();

        foreach(var proc in procs) {

            cw.WriteLine($"sb.AppendLine(\"-> {proc.ClassInfo.Name}\");");

        }

        cw.WriteLine();
        cw.WriteLine($"return sb.ToString();");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        if(processor.ApiInfo is { } api) {

            cw.WriteLine();
            cw.WriteLine($"public static void UseMinimalEndpoint(WebApplication app) {{");
            cw.UpIndent();
            cw.WriteLine();

            cw.WriteLine($"var service = app.Services.GetRequiredService<{processor.ClassInfo.FullTypeName}>();");
            cw.WriteLine($"var endpoint = app.Map{api.HttpMethod.LowerCapitalize()}(\"{api.Path}\", service.BuildProcess(app.Services));");

            cw.WriteLine();
            cw.WriteLineIf(api.HasConfigure, $"service.Configure(endpoint);");
            cw.WriteLineIf(api.AllowAnonymous, $"_ = endpoint.AllowAnonymous();");
            cw.WriteLineIf(api.Authorize, $"_ = endpoint.RequireAuthorization({(!string.IsNullOrEmpty(api.AuthorizePolicy) ? $"\"{api.AuthorizePolicy}\"" : string.Empty)});");

            cw.WriteLine();
            cw.DownIndent();
            cw.WriteLine($"}}");

        }

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{processor.ClassInfo.NameSpace ?? "Global"}.{processor.ClassInfo.Name}.g.cs", cw.ToString());

    }

    private static List<ProcedureInfo> GetValidProcedures(ProcessorInfo processor, List<ProcedureInfo> all) {

        return all
            .Where(x => x.InputType is null || processor.InputType.ParentTypes.Contains(x.InputType))
            .Where(x => x.OutputType is null || processor.OutputType.ParentTypes.Contains(x.OutputType))
            .ToList();

    }

}
