
namespace LemonKit.Generators.Common;

internal static class DisplayFormats
{

    public static readonly SymbolDisplayFormat NonGenericFullFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        genericsOptions: SymbolDisplayGenericsOptions.None
    );

}
