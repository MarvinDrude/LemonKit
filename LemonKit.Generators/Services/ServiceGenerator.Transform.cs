
namespace LemonKit.Generators.Services;

internal sealed partial class ServiceGenerator
{

    private static ServiceInfo? Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();

        if(symbol.InstanceConstructors is not [var constructor])
        {
            return null;
        }

        if(symbol.Interfaces is not [var interfaceSymbol])
        {
            return null;
        }

        var interfaceInfo = ClassInfoBuilder.GetInfo(interfaceSymbol);
        token.ThrowIfCancellationRequested();

        List<FieldInfo> fields = [];

        foreach(var field in symbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(x => x is { }))
        { // allow static ones, why not?

            fields.Add(new FieldInfo(
                field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                field.Name));

        }

        List<ModulePropertyInfo> modules = [];

        foreach(var property in symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x is
            {
                DeclaredAccessibility: Accessibility.Public,
                IsStatic: false,
                Name: not "EqualityContract",
            }))
        { // allow static ones, why not?

            if(property.Type.TypeKind is not TypeKind.Interface
                || property.SetMethod is not null)
            {
                continue;
            }

            if(GetPropertyAttribute(property.GetAttributes()) is not { } attr)
            {
                continue;
            }

            if(attr.ConstructorArguments is not [
                {
                    Type: INamedTypeSymbol
                    {
                        Name: "Type",
                        ContainingNamespace:
                        {
                            Name: "System",
                            ContainingNamespace.IsGlobalNamespace: true
                        }
                    },
                    Value: INamedTypeSymbol { } implArg
                }])
            {
                continue;
            }

            modules.Add(new ModulePropertyInfo(
                property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                implArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                property.Name));

        }

        return new ServiceInfo(
            classInfo,
            interfaceInfo,
            [.. fields],
            [.. modules]);

    }

    private static AttributeData? GetPropertyAttribute(ImmutableArray<AttributeData> attributes)
    {

        return attributes.FirstOrDefault(x => x.AttributeClass is
        {
            Name: "ModulePropertyAttribute",
            ContainingNamespace:
            {
                Name: "Attributes",
                ContainingNamespace:
                {
                    Name: "Services",
                    ContainingNamespace:
                    {
                        Name: "LemonKit",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        });

    }

    private static ModuleInfo? TransformModule(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();

        if(symbol.InstanceConstructors is not [var constructor])
        {
            return null;
        }

        if(symbol.Interfaces is not [var interfaceSymbol])
        {
            return null;
        }

        return new ModuleInfo(
            classInfo,
            constructor.Parameters.GetParameterInfos(),
            interfaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

    }



}
