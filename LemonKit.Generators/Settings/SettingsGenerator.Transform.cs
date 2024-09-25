
namespace LemonKit.Generators.Settings;

internal sealed partial class SettingsGenerator {

    private static SettingsInfo? TransformSetting(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;

        token.ThrowIfCancellationRequested();

        ClassInfo classInfo = ClassInfoBuilder.GetInfo(symbol);
        token.ThrowIfCancellationRequested();

        EnvironmentVariableInfo[] envVars = GetEnvironmentProperties(symbol);
        token.ThrowIfCancellationRequested();

        JsonFileInfo[] jsonFiles = GetJsonProperties(symbol);
        token.ThrowIfCancellationRequested();

        return new SettingsInfo(
            classInfo,
            envVars,
            jsonFiles);

    }

    private static JsonFileInfo[] GetJsonProperties(INamedTypeSymbol symbol) {

        List<JsonFileInfo> result = [];

        foreach(var member in symbol.GetMembers()) {

            if(member is not IPropertySymbol {
                DeclaredAccessibility: Accessibility.Public,
                IsStatic: false,
                Name: not "EqualityContract" // records
            } property) {
                continue;
            }

            if(symbol.TypeKind is not TypeKind.Interface
                && property.SetMethod is null) {
                continue;
            }

            if(GetJsonProperty(property) is not { } prop) {
                continue;
            }

            result.Add(prop);

        }

        return [..result];

    }

    private static JsonFileInfo? GetJsonProperty(IPropertySymbol property) {

        if(GetJsonAttribute(property) is not { } attr) {
            return null;
        }

        if(attr.ConstructorArguments is not [{ IsNull: false } arg]) {
            return null;
        }

        if(arg.Value is not string str) {
            return null;
        }

        return new JsonFileInfo(
            property.Name,
            str,
            property.Type.Name);

    }

    private static AttributeData? GetJsonAttribute(IPropertySymbol property) {

        return property.GetAttributes().FirstOrDefault(x => IsJsonAttribute(x.AttributeClass));

    }

    private static bool IsJsonAttribute(ITypeSymbol? symbol) {

        return symbol is {
            Name: "JsonFileAttribute",
            ContainingNamespace: {
                Name: "Attributes",
                ContainingNamespace: {
                    Name: "Settings",
                    ContainingNamespace: {
                        Name: "LemonKit",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };

    }

    private static EnvironmentVariableInfo[] GetEnvironmentProperties(INamedTypeSymbol symbol) {

        List<EnvironmentVariableInfo> result = [];

        foreach(var member in symbol.GetMembers()) {

            if(member is not IPropertySymbol { 
                DeclaredAccessibility: Accessibility.Public,
                IsStatic: false,
                Name: not "EqualityContract" // records
            } property) {
                continue;
            }

            if(symbol.TypeKind is not TypeKind.Interface 
                && property.SetMethod is null) {
                continue;
            }

            if(GetEnvironmentProperty(property) is not { } prop) {
                continue;
            }

            result.Add(prop);

        }

        return [..result];

    }

    private static EnvironmentVariableInfo? GetEnvironmentProperty(IPropertySymbol property) {

        if(GetEnvironmentAttribute(property) is not { } attr) {
            return null;
        }

        if(attr.ConstructorArguments is not [{ IsNull: false } arg]) {
            return null;
        }

        if(arg.Value is not string str) {
            return null;
        }

        return new EnvironmentVariableInfo(
            property.Name,
            str,
            property.Type.Name);

    }

    private static AttributeData? GetEnvironmentAttribute(IPropertySymbol property) {

        return property.GetAttributes().FirstOrDefault(x => IsEnvironmentAttribute(x.AttributeClass));

    }

    private static bool IsEnvironmentAttribute(ITypeSymbol? symbol) {

        return symbol is { 
            Name: "EnvironmentVariableAttribute",
            ContainingNamespace: {
                Name: "Attributes",
                ContainingNamespace: {
                    Name: "Settings",
                    ContainingNamespace: {
                        Name: "LemonKit",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };

    }

}
