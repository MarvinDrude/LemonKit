
namespace LemonKit.Cache;

/// <summary>
/// Implements a cache that is not necessarily thread safe
/// </summary>
public interface ICache<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    public TValue? this[TKey key] { get; set; }

    public TValue? Get(TKey key);

    public void Set(TKey key, TValue val);

    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);

    public void Clear();
}
