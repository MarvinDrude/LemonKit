
namespace LemonKit.Generators.Services;

[Generator]
internal sealed partial class ServiceGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {

//#if DEBUG
//        if(!Debugger.IsAttached) {
//            Debugger.Launch();
//        }
//#endif

        var assemblyName = context.CompilationProvider
            .Select(static (c, _) => c.AssemblyName!
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty)
                .Trim());

        var services = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Services.Attributes.ModuleServiceAttribute",
                predicate: static (_, _) => true,
                transform: Transform
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var modules = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Services.Attributes.ModuleAttribute",
                predicate: static (_, _) => true,
                transform: TransformModule
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var serviceInfos = services
            .Combine(modules.Collect())
            .Combine(assemblyName);

        //var moduleInfos = modules
        //    .Combine(assemblyName);

        context.RegisterSourceOutput(
            serviceInfos,
            static (spc, node) => Render(
                context: spc,
                serviceInfo: node.Left.Left,
                moduleInfos: node.Left.Right));

    }

}