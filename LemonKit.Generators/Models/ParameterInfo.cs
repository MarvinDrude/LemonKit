
namespace LemonKit.Generators.Models;

internal readonly record struct ParameterInfo {

    public readonly string Name;
    public readonly string Type;

    public ParameterInfo(
        string name,
        string type) {

        Name = name;
        Type = type;

    }

}
