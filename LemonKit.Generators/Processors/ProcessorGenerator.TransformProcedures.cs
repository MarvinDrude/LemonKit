
namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private static EquatableArray<ProcedureInfo>? TransformProcedures(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {

        token.ThrowIfCancellationRequested();

        if(context.Attributes is not [var attribute, ..]) {
            return [];
        }

        if(attribute.ConstructorArguments is not [var arg] ||
            arg.Type is not IArrayTypeSymbol {
                ElementType: {
                    Name: "Type",
                    ContainingNamespace: {
                        Name: "System",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }) {
            return [];
        }

        token.ThrowIfCancellationRequested();
        List<ProcedureInfo> infos = [];

        foreach(var constant in arg.Values) {

            if(constant.Value is not INamedTypeSymbol { IsUnboundGenericType: false } symbol) {
                continue;
            }

            if(symbol.OriginalDefinition is not { TypeParameters.Length: 2, IsAbstract: false } definition) {
                continue;
            }

            if(!symbol.IsProcedure()) {
                continue;
            }

            token.ThrowIfCancellationRequested();

            var fullTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var fullTypeNameNoGenerics = symbol.OriginalDefinition.ToDisplayString(DisplayFormats.NonGenericFullFormat);

            token.ThrowIfCancellationRequested();

            var classInfo = ClassInfoBuilder.GetInfo(definition);
            
            if(GetContraints(symbol) is not (true, string inputType, string outputType)) {
                continue;
            }

            infos.Add(new ProcedureInfo(
                classInfo, 
                fullTypeName, 
                fullTypeNameNoGenerics,
                inputType,
                outputType));

        }

        return new EquatableArray<ProcedureInfo>([..infos]);

    }

    private static (bool IsValid, string? InputType, string? OutputType) GetContraints(INamedTypeSymbol symbol) {

        var definition = symbol.OriginalDefinition;

        if(GetConstraint(definition.TypeParameters[0]) is not (true, var inputType)) {
            return default;
        }

        if(GetConstraint(definition.TypeParameters[1]) is not (true, var outputType)) {
            return default;
        }

        return (true, inputType, outputType);

    }

    private static (bool IsValid, string? Constraint) GetConstraint(ITypeParameterSymbol parameter) {

        if(parameter.ConstraintTypes is []) {
            return (true, default);
        }

        if(parameter.ConstraintTypes is not [var target]) { // only allow one constraint for now
            return (false, default);
        }

        var fullTypeName = target.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return (true, fullTypeName);

    }

    private static ProcedureClassInfo? TransformProcedure(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {



    }

}
