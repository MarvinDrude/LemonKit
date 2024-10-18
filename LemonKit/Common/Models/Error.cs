
namespace LemonKit.Common.Models;

public class Error(string message)
{
    public string Message { get; } = message;
}
