
namespace LemonKit.Generators.Extensions;

public static class IMethodSymbolExtensions {

    public static ITypeSymbol? GetTaskInnerReturnType(this IMethodSymbol method) {

        if(!method.ReturnType.IsTaskOneGeneric()) {
            return null;
        }

        return ((INamedTypeSymbol)method.ReturnType).TypeArguments.FirstOrDefault();

    }

}
