
namespace LemonKit.Collections.Refs;

/// <summary>
/// Ref struct to build a "dynamic" list of <see cref="T"/>, stackalloc first and fall back to ArrayPool
/// <para>
/// Make sure to call Dispose at the end!
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public ref struct SpanListBuilder<T> {

    /// <summary>
    /// Initial stack allocated span used until needs grow
    /// </summary>
    private Span<T> _Span;

    /// <summary>
    /// Array taken from ArrayPool if grow was needed
    /// </summary>
    private T[]? _Array;

    /// <summary>
    /// Current position in the span or array
    /// </summary>
    private int _Position;

    /// <summary>
    /// Typically used with a stack allocated initial span
    /// </summary>
    /// <param name="initial">Typically stack allocated</param>
    public SpanListBuilder(Span<T> initial) {

        _Span = initial;
        _Array = null;

        _Position = 0;

    }

    /// <summary>
    /// Get <see cref="T"/> at index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ref T this[int index] {
        get {
            return ref _Span[index];
        }
    }

    /// <summary>
    /// Current length dynamically
    /// </summary>
    public int Length {
        readonly get => _Position;
        set {
            _Position = value;
        }
    }

    /// <summary>
    /// Add new item at the current position and increases it
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item) {

        int position = _Position;
        Span<T> span = _Span;

        if(position < span.Length) {

            span[position] = item;
            _Position = position + 1;

        } else {

            AddWithGrow(item);

        }

    }

    /// <summary>
    /// Add new span to current position and increase it
    /// </summary>
    /// <param name="source"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(scoped ReadOnlySpan<T> source) {

        int position = _Position;
        Span<T> span = _Span;

        if(source.Length == 1 && position < span.Length) {

            span[position] = source[0];
            _Position = position + 1;

        } else {

            AddWithGrowSpan(source);

        }

    }

    /// <summary>
    /// As readonly span
    /// </summary>
    /// <returns></returns>
    public readonly ReadOnlySpan<T> AsSpan() {

        return _Span[.._Position];

    }

    /// <summary>
    /// Call after use
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {

        T[]? shouldReturn = _Array;

        if(shouldReturn is not null) {

            _Array = null;
            ArrayPool<T>.Shared.Return(shouldReturn);

        }

    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithGrowSpan(scoped ReadOnlySpan<T> source) {

        if((_Position + source.Length) > _Span.Length) {
            Grow(_Span.Length - _Position + source.Length);
        }

        source.CopyTo(_Span[_Position..]);
        _Position += source.Length;

    }

    /// <summary>
    /// Add with grow before
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithGrow(T item) {

        int position = _Position;
        Grow(1);

        _Span[position] = item;
        _Position = position + 1;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="addCapacity"></param>
    private void Grow(int addCapacity) {

        // atleast double it
        int capacity = Math.Max(
            _Span.Length != 0 ? _Span.Length * 2 : 4,
            _Span.Length + addCapacity);

        if(capacity > ArrayMaxLength) {
            capacity = Math.Max(Math.Max(_Span.Length + 1, ArrayMaxLength), _Span.Length);
        }

        T[] array = ArrayPool<T>.Shared.Rent(capacity);
        _Span.CopyTo(array);

        T[]? shouldReturn = _Array;
        _Span = _Array = array;

        if(shouldReturn is not null) {
            ArrayPool<T>.Shared.Return(shouldReturn);
        }

    }

    private const int ArrayMaxLength = 2147483591;

}
