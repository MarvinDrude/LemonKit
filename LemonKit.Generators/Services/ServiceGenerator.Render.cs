
namespace LemonKit.Generators.Services;

internal sealed partial class ServiceGenerator {

    private static void Render(
        SourceProductionContext context,
        ServiceInfo? serviceInfo,
        ImmutableArray<ModuleInfo?> moduleInfos) {

        var token = context.CancellationToken;
        if(serviceInfo is not { } service) {
            return;
        }

        Dictionary<string, FieldInfo> fields = service.Fields.ToDictionary(x => x.TypeFullName);

        token.ThrowIfCancellationRequested();
        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using System.Text;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");

        if(service.ClassInfo.NameSpace is { } nameSpace) {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace};");

        }
        cw.WriteLine();

        cw.WriteLine($"{ClassInfoBuilder.GetClassString(service.ClassInfo, true)} {{");
        cw.UpIndent();
        cw.WriteLine();

        foreach(var property in service.Modules) {

            cw.WriteLine($"private {property.TypeFullName}? _pf{property.Name};");
            cw.WriteLine();

            cw.WriteLine($"public partial {property.TypeFullName} {property.Name} {{");
            cw.UpIndent();

            cw.WriteLine($"get {{");
            cw.UpIndent();

            cw.WriteLine($"if(_pf{property.Name} is null) {{");
            cw.UpIndent();

            if(moduleInfos.FirstOrDefault(x => x is { 
                InterfaceTypeFullName: { } interfaceType } && interfaceType == property.TypeFullName) is not { } module) {
                return;
            }

            var parameters = module.ConstructorArgs.Select(
                x => fields.TryGetValue(x.Type, out var field) ? field.Name : "null");

            cw.WriteLine($"_pf{property.Name} = new {property.ImplementationTypeFullName}({string.Join(", ", parameters)});");

            cw.DownIndent();
            cw.WriteLine($"}}");

            cw.WriteLine($"return _pf{property.Name};");

            cw.DownIndent();
            cw.WriteLine($"}}");

            cw.DownIndent();
            cw.WriteLine($"}}");
        
        }

        cw.WriteLine();

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        context.AddSource($"{service.ClassInfo.NameSpace ?? "Global"}.{service.ClassInfo.Name}.g.cs", cw.ToString());

        token.ThrowIfCancellationRequested();
        RenderInterface(context, service);

    }

    private static void RenderInterface(
        SourceProductionContext context,
        ServiceInfo service) {

        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using System.Text;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");

        if(service.InterfaceInfo.NameSpace is { } nameSpace) {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace};");

        }
        cw.WriteLine();

        cw.WriteLine($"{ClassInfoBuilder.GetClassString(service.InterfaceInfo, true)} {{");
        cw.UpIndent();
        cw.WriteLine();

        foreach(var property in service.Modules) {

            cw.WriteLine($"public {property.TypeFullName} {property.Name} {{ get; }}");
            cw.WriteLine();

        }

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        context.AddSource($"{service.InterfaceInfo.NameSpace ?? "Global"}.{service.InterfaceInfo.Name}.g.cs", cw.ToString());

    }

}
