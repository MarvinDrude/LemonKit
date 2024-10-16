
namespace LemonKit.Validation;

/// <summary>
/// Validates <see cref="T"/> instances
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IValidate<T>
{

    /// <summary>
    /// Validates the <see cref="T"/> input with given attribute rules
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public ValidationResult Validate(T? input);

}
