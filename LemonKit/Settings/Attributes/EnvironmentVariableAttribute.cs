
namespace LemonKit.Settings.Attributes;

/// <summary>
/// Mark a property in a settings class to be read from the environment variables
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EnvironmentVariableAttribute(string name) : Attribute
{

    public string Name { get; set; } = name;

}
