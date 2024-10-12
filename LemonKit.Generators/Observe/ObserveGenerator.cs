namespace LemonKit.Generators.Observe;

[Generator]
internal sealed partial class ObserveGenerator : IIncrementalGenerator {

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

        var observers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Observe.Attributes.ObserveAttribute",
                predicate: static (_, _) => true,
                transform: Transform
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var extensionInfos = observers.Collect()
            .Combine(assemblyName);

        var observerInfos = observers
            .Combine(assemblyName);

        context.RegisterSourceOutput(
            observerInfos,
            static (spc, node) => Render(
                context: spc,
                observeInfo: node.Left));

        context.RegisterSourceOutput(
            extensionInfos,
            static (spc, node) => RenderContainer(
                context: spc,
                observeInfos: node.Left,
                assemblyName: node.Right));


    }

}
