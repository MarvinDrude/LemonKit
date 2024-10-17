
namespace LemonKit.Cache;

/// <summary>
/// Uses concurrent dictionary which is thread safe but allows unnecessary double instantiation at concurrent sets
/// </summary>
public sealed class DictionaryCache<TKey, TValue>
    : IConcurrentCache<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    private readonly ConcurrentDictionary<TKey, TValue> _Entries = [];

    public TValue? this[TKey key]
    {
        get
        {
            return Get(key);
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            Set(key, value);
        }
    }

    public void Clear()
    {
        _Entries.Clear();
    }

    public TValue? Get(TKey key)
    {
        if(_Entries.TryGetValue(key, out var value))
        {
            return value;
        }

        return default;
    }

    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _Entries.Remove(key, out value);
    }

    public void Set(TKey key, TValue val)
    {
        _Entries[key] = val;
    }
}

