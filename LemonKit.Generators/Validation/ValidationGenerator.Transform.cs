
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator
{

    private static ValidationInfo? Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();

        ParameterInfo[]? extraParameters = null;

        if(GetExtraValidationMethod(symbol) is { } extraValidate)
        {
            extraParameters = GetExtraValidationParameters(extraValidate, symbol);
        }

        bool hasExtraValidateMethod = extraParameters != null;
        string validatorName = GetValidatorName(symbol);

        token.ThrowIfCancellationRequested();

        return new ValidationInfo(
            classInfo,
            hasExtraValidateMethod,
            symbol.IsReferenceType,
            validatorName,
            extraParameters,
            GetProperties(symbol, token));

    }

    private static string GetValidatorName(INamedTypeSymbol symbol)
    {

        StringBuilder sb = new();
        sb.Append(symbol.Name);

        while(symbol.ContainingType is { } containing)
        {

            sb.Append(containing.OriginalDefinition.Name);
            symbol = symbol.ContainingType;

        }

        return sb.ToString();

    }

    private static ValidationPropertyInfo[] GetProperties(INamedTypeSymbol symbol, CancellationToken token)
    {

        var result = new List<ValidationPropertyInfo>();
        token.ThrowIfCancellationRequested();

        foreach(var member in symbol.GetMembers())
        {

            if(member is not IPropertySymbol
                {
                    DeclaredAccessibility: Accessibility.Public,
                    IsStatic: false,
                    Name: not "EqualityContract",
                } property)
            {
                continue;
            }

            if(symbol.TypeKind is not TypeKind.Interface
                && property.SetMethod is null)
            {
                continue;
            }

            token.ThrowIfCancellationRequested();

            if(GetPropertyValidation(
                property.Name,
                property,
                property.Type,
                property.NullableAnnotation,
                token) is not { } target)
            {

                continue;

            }

            token.ThrowIfCancellationRequested();

            result.Add(target);

        }

        token.ThrowIfCancellationRequested();
        return [.. result];

    }

    private static ValidationPropertyInfo? GetPropertyValidation(
        string name, IPropertySymbol property, ITypeSymbol type, NullableAnnotation nullable, CancellationToken token)
    {

        var attributes = property.GetAttributes();
        var isReferenceType = type.IsReferenceType;
        var isNullable = isReferenceType ? nullable is NullableAnnotation.Annotated : type.IsNullableType();

        token.ThrowIfCancellationRequested();

        var baseType = isNullable && !isReferenceType
            ? ((INamedTypeSymbol)type).TypeArguments[0]
            : type;

        List<ValidatePropertyInfo> validations = [];

        foreach(var attribute in attributes)
        {

            if(GetValidationFromAttribute(type, baseType, attribute) is not { } attrValidation)
            {
                continue;
            }

            token.ThrowIfCancellationRequested();
            validations.Add(attrValidation);

        }

        if(validations is { Count: 0 })
        {
            return null;
        }

        token.ThrowIfCancellationRequested();

        return new ValidationPropertyInfo(
            name,
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isNullable,
            validations.Count > 0,
            isReferenceType,
            baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            [.. validations]);

    }

    private static ValidatePropertyInfo? GetValidationFromAttribute(
        ITypeSymbol type, ITypeSymbol baseType, AttributeData attribute)
    {

        if(attribute.AttributeClass is not { } attrClass)
        {
            return null;
        }

        if(!attrClass.ImplementsValidationAttribute())
        {
            return null;
        }

        if(attrClass.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(static method => method is
            {
                DeclaredAccessibility: Accessibility.Public,
                IsStatic: true,
                Name: "Validate",
            })
            .FirstOrDefault() is not
            {
                Parameters: [{ Type: { } targetParamType }, ..]
            } validateMethod)
        {
            return null;
        }

        if(attribute.ConstructorArguments is not [.., { } errorCodeOb]
            || errorCodeOb.Value is not string errorCode)
        {
            return null;
        }

        bool isService = false;
        string? serviceTypeFullName = null;
        string[]? servicePath = null;
        List<ConstructorArgInfo> args = [];

        if(attribute.ConstructorArguments is [
            {
                Type: INamedTypeSymbol
                {
                    Name: "Type",
                    ContainingNamespace:
                    {
                        Name: "System",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            } serviceArg,
            {
                Type: IArrayTypeSymbol
                {
                    ElementType.SpecialType: SpecialType.System_String
                }
            } servicePathArg, ..])
        {

            if(serviceArg.Value is not INamedTypeSymbol symbol)
            {
                return null;
            }

            if(symbol.OriginalDefinition is not { } definition)
            {
                return null;
            }

            isService = true;
            serviceTypeFullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            List<string> serviceAccessPath = [];

            foreach(var pathValue in servicePathArg.Values)
            {

                if(pathValue.Value is not string path)
                {
                    return null;
                }

                serviceAccessPath.Add(path);

            }

            servicePath = [.. serviceAccessPath];

        }
        else
        {

            var arguments = attribute.ConstructorArguments;

            foreach(var argument in arguments)
            {

                if(GetArgValue(argument) is not { } constArg)
                {
                    return null;
                }

                args.Add(constArg);

            }

        }

        List<ValidateParameterInfo> parameterInfos = [];
        foreach(var parameter in validateMethod.Parameters)
        {

            parameterInfos.Add(new ValidateParameterInfo(
                parameter.Name,
                parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

        }

        return new ValidatePropertyInfo(
            validateMethod.IsGenericMethod,
            targetParamType.IsReferenceType ? targetParamType.NullableAnnotation is NullableAnnotation.Annotated : type.IsNullableType(),
            isService,
            serviceTypeFullName,
            servicePath,
            [.. parameterInfos],
            [.. args],
            errorCode,
            ClassInfoBuilder.GetInfo(attrClass));

    }

    private static ConstructorArgInfo? GetArgValue(TypedConstant constant)
    {

        return new ConstructorArgInfo(
            constant.GetAsString());

    }

    private static IMethodSymbol? GetExtraValidationMethod(INamedTypeSymbol symbol)
    {

        return symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(static method => method is
            {
                Name: "ExtraValidate",
                IsStatic: true,
                ReturnsVoid: true,
                DeclaredAccessibility: Accessibility.Public
            });

    }

    private static ParameterInfo[]? GetExtraValidationParameters(IMethodSymbol symbol, INamedTypeSymbol classSymbol)
    {

        List<ParameterInfo> result = [];
        bool foundResult = false;
        bool foundInput = false;

        for(int e = 0; e < symbol.Parameters.Length; e++)
        {

            var parameter = symbol.Parameters[e];

            if(parameter.Type is INamedTypeSymbol checkSymbol)
            {
                if(checkSymbol.IsValidationResult())
                {
                    foundResult = true;
                }
                if(SymbolEqualityComparer.Default.Equals(checkSymbol, classSymbol))
                {
                    foundInput = true;
                }
            }

            result.Add(new ParameterInfo(
                parameter.Name,
                parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                e,
                default!));

        }

        return (foundResult && foundInput) ? [.. result] : null;

    }

}
