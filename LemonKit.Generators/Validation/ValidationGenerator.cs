
namespace LemonKit.Generators.Validation;

[Generator]
internal sealed partial class ValidationGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

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

        var validates = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Validation.Attributes.ValidateAttribute",
                predicate: static (_, _) => true,
                transform: Transform
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var validateInfos = validates
            .Combine(assemblyName);

        var extensionInfos = validates.Collect()
            .Combine(assemblyName);

        context.RegisterSourceOutput(
            validateInfos,
            static (spc, node) => RenderValidate(
                context: spc,
                validationInfo: node.Left));

        context.RegisterSourceOutput(
            extensionInfos,
            static (spc, node) => RenderExtensions(
                context: spc,
                validationInfos: node.Left));

    }

}