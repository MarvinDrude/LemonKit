
namespace LemonKit.Contexts;

public sealed class ContextProvider<T>
    where T : new()
{
    private static readonly AsyncLocal<T> _ContextLocal = new();

    public T Current => _ContextLocal.Value ?? StartContext();

    public T StartContext()
    {
        _ContextLocal.Value = new T();
        return _ContextLocal.Value;
    }
}
