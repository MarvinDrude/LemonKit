
namespace LemonKit.Generators.Models;

internal readonly record struct ParameterInfo
{

    public readonly string Name;
    public readonly string Type;

    public readonly int Index;

    public readonly EquatableArray<ParaAttributeInfo> Attributes;

    public ParameterInfo(
        string name,
        string type,
        int index,
        ParaAttributeInfo[] attributes)
    {

        Name = name;
        Type = type;

        Index = index;

        Attributes = new EquatableArray<ParaAttributeInfo>(attributes);

    }

}


