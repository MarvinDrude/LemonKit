
namespace LemonKit.Generators.Extensions;

internal static class IParameterSymbolExtensions {

    public static ParameterInfo[] GetParameterInfos(this ImmutableArray<IParameterSymbol> parameters) {

        ParameterInfo[] result = new ParameterInfo[parameters.Length];

        for(int e = 0; e < parameters.Length; e++) {

            IParameterSymbol symbol = parameters[e];
            string fullTypeName = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string name = symbol.Name;

            result[e] = new ParameterInfo(
                name, fullTypeName, e, GetAttributes(symbol.GetAttributes()));

        }

        return result;

    }

    private static ParaAttributeInfo[] GetAttributes(ImmutableArray<AttributeData> attrs) {

        if(attrs.Length == 0) {
            return [];
        }

        ParaAttributeInfo[] result = new ParaAttributeInfo[attrs.Length];

        for(int e = 0; e < attrs.Length; e++) {

            AttributeData attr = attrs[e];
            if(attr.AttributeClass is null) {
                return [];
            }

            string fullTypeName = attr.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            result[e] = new ParaAttributeInfo(
                fullTypeName,
                attr.AttributeClass.Name);

        }

        return result;

    }

}
