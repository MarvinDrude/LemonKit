
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

        var parameters = execute.Parameters.GetParameterInfos();
        token.ThrowIfCancellationRequested();

        ParameterInfo? inputParameter = null;

        foreach(var parameter in parameters) {
            foreach(var attr in parameter.Attributes) {
                if(attr.FullName is "global::LemonKit.Processors.Attributes.InputAttribute") {
                    inputParameter = parameter;
                    break;
                }
            }
            if(inputParameter is { }) {
                break;
            }
        }

        if(inputParameter is not { } input) {
            return null;
        }

        token.ThrowIfCancellationRequested();
        var originInput = execute.Parameters[input.Index];
        var inputType = originInput.Type.GetRTypeInfo();

        token.ThrowIfCancellationRequested();

        if(execute.GetTaskInnerReturnType() is not { } outputSymbol) {
            return null;
        }

        var outputType = outputSymbol.GetRTypeInfo();
        token.ThrowIfCancellationRequested();

        AttributeData? attrProcessor = null;
        AttributeData? attrProcedures = null;

        foreach(var attribute in symbol.GetAttributes()) {

            if(attribute.AttributeClass is not { } attrClass) {
                continue;
            }

            switch(attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) {

                case "global::LemonKit.Processors.Attributes.ProcessorAttribute":
                    attrProcessor = attribute;
                    break;
                case "global::LemonKit.Processors.Attributes.ProceduresAttribute":
                    attrProcedures = attribute;
                    break;

            }

        }

        if(attrProcessor is not { } processor) {
            return null;
        }

        token.ThrowIfCancellationRequested();
        bool useAssemblyProcedures = processor.ConstructorArguments[0].Value is true;
        ProcedureInfo[]? procedures = null;

        if(attrProcedures is not null) {
            procedures = GetProcedures(attrProcedures, token);
        }

        return new ProcessorInfo(
            classInfo,
            inputType,
            outputType,
            useAssemblyProcedures,
            procedures,
            parameters);

    }

}
