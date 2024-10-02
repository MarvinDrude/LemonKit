
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator {

    private static ValidationInfo? Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();



    }

}
