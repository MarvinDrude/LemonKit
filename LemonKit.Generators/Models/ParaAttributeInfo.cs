
namespace LemonKit.Generators.Models;

internal readonly record struct ParaAttributeInfo {

    public readonly string FullName;
    public readonly string Name;

    public ParaAttributeInfo(
        string fullName,
        string name) {

        FullName = fullName;
        Name = name;

    }

}
