
namespace LemonKit.Cache;

/// <summary>
/// A cache that is strictly thread safe
/// <para>
/// But doesn't need to make sure that double creation is forbidden in race conditions (concurrent setting)
/// </para>
/// </summary>
public interface IConcurrentCache<TKey, TValue> : ICache<TKey, TValue>
    where TKey : IEquatable<TKey>
{

}
