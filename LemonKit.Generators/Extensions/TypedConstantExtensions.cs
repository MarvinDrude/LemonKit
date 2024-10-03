
namespace LemonKit.Generators.Extensions;

public static class TypedConstantExtensions {

    public static string GetAsString(this TypedConstant constant) {

        if(constant.Kind is TypedConstantKind.Array) {

            var elementStrings = constant.Values.Select(x => x.GetValueAsString());
            return $"[{string.Join(", ", elementStrings)}]";

        } else {

            return constant.GetValueAsString();

        }

    }

    private static string GetValueAsString(this TypedConstant constant) {

        if(constant is { IsNull: true } or { Value: null }) {
            return "null";
        }

        return constant.Value switch {

            string str => $"\"{str}\"",
            char cha => $"'{cha}'",
            bool bo => bo ? "true" : "false",
            double dbl => dbl + "d",
            float flt => flt + "f",
            int integer => integer + string.Empty,
            long longus => longus + string.Empty,

            // Type into typeof(global::NameSpace.Type)
            INamedTypeSymbol symbol when symbol.OriginalDefinition is { } definition => $"typeof({definition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})",

            _ => constant.Value.ToString()

        };

    }

}
