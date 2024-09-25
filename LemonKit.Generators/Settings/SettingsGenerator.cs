
namespace LemonKit.Generators.Settings;

[Generator]
internal sealed partial class SettingsGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {

//#if DEBUG
//        if(!Debugger.IsAttached) {
//            Debugger.Launch();
//        }
//#endif

        var settings = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "LemonKit.Settings.Attributes.SettingsAttribute",
                predicate: (_, _) => true,
                transform: TransformSetting
            )
            .Where(static e => e is not null)
            .Select(static (x, _) => x!);

        var assemblyName = context.CompilationProvider
            .Select(static (c, _) => c.AssemblyName!
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty)
                .Trim());

        var combinedAll = assemblyName.Combine(settings.Collect());
        var combined = settings.Combine(assemblyName);

        context.RegisterSourceOutput(
            combined, static (spc, e) => RenderSetting(spc, e.Left, e.Right));

    }

}
