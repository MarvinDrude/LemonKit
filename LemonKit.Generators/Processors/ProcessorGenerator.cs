
namespace LemonKit.Generators.Processors;

[Generator]
public sealed partial class ProcessorGenerator : IIncrementalGenerator {

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

        var processors = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Processors.Attributes.ProcessorAttribute",
                predicate: static (_, _) => true,
                transform: TransformProcessor
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var assemblyProcedures = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Processors.Attributes.ProceduresAttribute",
                predicate: static (SyntaxNode node, CancellationToken token) => node is CompilationUnitSyntax,
                transform: TransformProcedures
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!)
            .Collect();

        var procedures = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Processors.Attributes.ProcedureAttribute",
                predicate: static (_, _) => true,
                transform: TransformProcedure
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var processorInfos = processors
            .Combine(assemblyName)
            .Combine(assemblyProcedures);

        var procedureInfos = procedures
            .Combine(assemblyName);

        context.RegisterSourceOutput(
            processorInfos,
            static (spc, node) => RenderProcessor(
                context: spc,
                processorInfo: node.Left.Left,
                assemblyProcedureInfos: node.Right));

        context.RegisterSourceOutput(
            procedureInfos,
            static (spc, node) => RenderProcedure(
                context: spc,
                procedureInfo: node.Left));

    }

}
