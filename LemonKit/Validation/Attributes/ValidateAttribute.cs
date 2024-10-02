
namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a class to have validation being generated as IValidate{T} in IServiceProvider
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public sealed class ValidateAttribute : Attribute {



}
