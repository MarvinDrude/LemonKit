
namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private static ProcessorInfo? TransformProcessor(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();

        if(symbol.ContainingType is not null) {
            return null;
        }

        if(symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(static method => !method.IsStatic)
            .Where(static method => method.Name is "Execute")
            .ToList() is not [var execute]) {
            return null;
        }

        if(execute.Parameters is { Length: 0 }) {
            return null;
        }

        if(execute.ReturnsVoid) {
            return null;
        }

        token.ThrowIfCancellationRequested();



    }

}
