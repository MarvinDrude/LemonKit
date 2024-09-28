
namespace LemonKit.Generators.Extensions;

internal static class ITypeSymbolExtensions {

    public static bool IsTaskOneGeneric(this ITypeSymbol symbol) {

        return symbol is INamedTypeSymbol {
            MetadataName: "Task`1",
            ContainingNamespace: { 
                Name: "Tasks",
                ContainingNamespace: {
                    Name: "Threading",
                    ContainingNamespace: {
                        Name: "System",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };

    }

    public static bool IsProcedureWithTwoGenerics(this ITypeSymbol symbol) {

        return symbol is {
            MetadataName: "Procedure`2",
            ContainingNamespace: {
                Name: "Processors",
                ContainingNamespace: {
                    Name: "LemonKit",
                    ContainingNamespace.IsGlobalNamespace: true
                }
            }
        };

    }

    public static bool IsProcedure(this INamedTypeSymbol symbol) {

        return symbol.IsProcedureWithTwoGenerics() || 
            (symbol.BaseType is not null && symbol.BaseType.OriginalDefinition.IsProcedure());

    }

    public static bool IsCancellationToken(this ITypeSymbol symbol) {

        return symbol is INamedTypeSymbol {
            Name: "CancellationToken",
            ContainingNamespace: {
                Name: "Threading",
                ContainingNamespace: {
                    Name: "System",
                    ContainingNamespace.IsGlobalNamespace: true
                }
            }
        };

    }

    public static RTypeInfo GetRTypeInfo(this ITypeSymbol symbol) {

        var fullTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var baseTypes = new List<string>();

        AddBaseTypes(symbol, baseTypes);

        return new RTypeInfo(
            fullTypeName,
            [..baseTypes]);

    }

    private static void AddBaseTypes(ITypeSymbol symbol, List<string> bases) {

        if(symbol.SpecialType is SpecialType.System_Collections_IEnumerable
            or SpecialType.System_Object) {
            return;
        }

        bases.Add(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        if(symbol.BaseType is not null) {
            AddBaseTypes(symbol.BaseType, bases);
        }

        foreach(var interFace in symbol.Interfaces) {
            AddBaseTypes(interFace, bases);
        }

    }

}
