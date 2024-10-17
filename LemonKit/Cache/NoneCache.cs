
namespace LemonKit.Cache;

/// <summary>
/// Cache used to disable caching by not providing functionality
/// </summary>
public sealed class NoneCache<TKey, TValue> : IConcurrentCache<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    public TValue? this[TKey key] { get => default; set { } }

    public void Clear()
    {

    }

    public TValue? Get(TKey key)
    {
        return default;
    }

    public void Set(TKey key, TValue val)
    {

    }

    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        value = default;
        return false;
    }
}
