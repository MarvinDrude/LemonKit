
namespace LemonKit.Settings.Attributes;

/// <summary>
/// Mark property in settings class to be read and deserialize from json file
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class JsonFileAttribute(string fileName) : Attribute
{

    public string FileName { get; set; } = fileName;

}
