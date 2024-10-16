
namespace LemonKit.Services.Attributes;

/// <summary>
/// Mark a partial property that should be generated (module constructors should only be one + all parameter types need to be present as fields in base service)
/// <para>
/// Backing field instances are only created on demand.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ModulePropertyAttribute : Attribute
{

    private Type _Type;

    /// <summary>
    /// Provide the module implementation type
    /// </summary>
    /// <param name="moduleImplementationType">Implementation Type (must be inside the same project and have the module attribute)</param>
    public ModulePropertyAttribute(
        Type moduleImplementationType)
    {

        _Type = moduleImplementationType;

    }

}
