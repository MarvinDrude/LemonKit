
namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private static EquatableArray<ProcedureInfo>? TransformProcedures(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {

        token.ThrowIfCancellationRequested();

        if(context.Attributes is not [var attribute, ..]) {
            return [];
        }

        return new EquatableArray<ProcedureInfo>(GetProcedures(attribute, token));

    }

    private static ProcedureInfo[] GetProcedures(AttributeData attribute, CancellationToken token) {

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

            if(constant.Value is not INamedTypeSymbol { IsUnboundGenericType: true } symbol) {
                continue;
            }

            if(symbol.OriginalDefinition is not { TypeParameters.Length: 2, IsAbstract: false } definition) {
                continue;
            }

            if(!definition.IsProcedure()) {
                continue;
            }

            token.ThrowIfCancellationRequested();

            var fullTypeName = definition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var fullTypeNameNoGenerics = definition.OriginalDefinition.ToDisplayString(DisplayFormats.NonGenericFullFormat);

            token.ThrowIfCancellationRequested();

            var classInfo = ClassInfoBuilder.GetInfo(definition);

            if(GetContraints(definition) is not (true, var inputType, var outputType)) {
                continue;
            }

            if(definition.Constructors is not [var constructor]) {
                continue;
            }

            infos.Add(new ProcedureInfo(
                classInfo,
                fullTypeName,
                fullTypeNameNoGenerics,
                inputType,
                outputType,
                constructor.Parameters.GetParameterInfos()));

        }

        return [..infos];

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

        AttributeData? attrProcedure = null;

        foreach(var attribute in symbol.GetAttributes()) {

            if(attribute.AttributeClass is not { } attrClass) {
                continue;
            }

            switch(attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) {

                case "global::LemonKit.Processors.Attributes.ProcedureAttribute":
                    attrProcedure = attribute;
                    break;

            }

        }

        if(attrProcedure is not { } processor) {
            return null;
        }

        token.ThrowIfCancellationRequested();

        return new ProcedureClassInfo(
            classInfo,
            inputType,
            outputType,
            parameters);

    }

}
