
namespace LemonKit.Generators.Models;

internal readonly record struct ClassInfo
{

    public readonly string Name;
    public readonly string? NameSpace;
    public readonly string Type;

    public readonly string FullTypeName;

    public readonly bool IsStatic;
    public readonly bool IsSealed;
    public readonly bool IsInternal;

    public ClassInfo(
        string name,
        string? nameSpace,
        string type,
        string fullTypeName,
        bool isStatic,
        bool isSealed,
        bool isInternal)
    {

        Name = name;
        NameSpace = nameSpace;
        Type = type;
        FullTypeName = fullTypeName;

        IsStatic = isStatic;
        IsSealed = isSealed;
        IsInternal = isInternal;

    }

}


internal static class ClassInfoBuilder
{

    public static ClassInfo GetInfo(INamedTypeSymbol symbol)
    {

        string? nameSpace = symbol.ContainingNamespace.ToString();
        if(nameSpace == "<global namespace>")
        {
            nameSpace = null;
        }

        string name = symbol.Name;
        string type = GetType(symbol);

        bool isStatic = symbol.IsStatic;
        bool isSealed = symbol.IsSealed;
        bool isInternal = symbol.DeclaredAccessibility == Accessibility.Internal;

        return new ClassInfo(
            name,
            nameSpace,
            type,
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isStatic,
            isSealed,
            isInternal);

    }

    public static string GetClassString(ClassInfo classInfo, bool isPartial = true)
    {

        List<string> modifiers = [
            classInfo.IsInternal ? "internal" : "public"
        ];

        if(classInfo.IsSealed)
        {
            modifiers.Add("sealed");
        }
        else if(classInfo.IsStatic)
        {
            modifiers.Add("static");
        }

        if(isPartial)
        {
            modifiers.Add("partial");
        }

        modifiers.Add(classInfo.Type);
        modifiers.Add(classInfo.Name);

        return string.Join(" ", modifiers);

    }

    private static string GetType(INamedTypeSymbol symbol)
    {

        return symbol switch
        {

            { IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
            { IsRecord: true } => "record",
            { TypeKind: TypeKind.Interface } => "interface",
            { TypeKind: TypeKind.Struct } => "struct",

            _ => "class"

        };

    }

}