
namespace LemonKit.Generators.Models;

internal readonly record struct RTypeInfo {

    public readonly string Name;

    /// <summary>
    /// Included is base classes (all) and all their interfaces
    /// </summary>
    public readonly EquatableArray<string> ParentTypes;

    public RTypeInfo(
        string name,
        string[] parentTypes) {

        Name = name;
        ParentTypes = new EquatableArray<string>(parentTypes);

    }

}