
namespace LemonKit.Generators.Collections;

internal struct ArrayBuilder<T> : IDisposable
{

    private static readonly ObjectPool<ArrayWriter> _WriterPool = new(static () => new ArrayWriter(), 64);

    public readonly ReadOnlySpan<T> Span => Writer.Span;

    public readonly int Count => Writer.Count;

    private bool _Disposed = false;

    private ArrayWriter Writer { get; set; }

    public ArrayBuilder()
    {

        Writer = _WriterPool.Get();

    }

    public readonly Span<T> Advance(int size)
    {
        return Writer.Advance(size);
    }

    public readonly void Add(T item)
    {
        Writer.Add(item);
    }

    public readonly void AddRange(ReadOnlySpan<T> items)
    {
        Writer.AddRange(items);
    }

    public readonly void Insert(int index, T item)
    {
        Writer.Insert(index, item);
    }

    public readonly T[] ToArray()
    {
        return Writer.Span.ToArray();
    }

    public override readonly string ToString()
    {
        return Writer.Span.ToString();
    }

    public readonly void Clear()
    {
        Writer.Clear();
    }

    public void Dispose()
    {

        if(_Disposed)
        {
            return;
        }

        Writer.Clear();
        _WriterPool.Return(Writer);

        Writer = default!;

    }

    private sealed class ArrayWriter
    {

        public int Count => Index;

        public T this[int index] => Span[Index];

        public ReadOnlySpan<T> Span
        {
            get => new(Contents, 0, Index);
        }

        private T[] Contents;
        private int Index;

        public ArrayWriter()
        {

            uint size = (uint)(typeof(T) == typeof(char) ?
                1024 : 16);
            Contents = new T[size];
            Index = 0;

        }

        public void Add(T value)
        {

            EnsureCapacity(1);
            Contents[Index++] = value;

        }

        public Span<T> Advance(int size)
        {

            EnsureCapacity(size);
            Span<T> span = Contents.AsSpan(Index, size);

            Index += size;

            return span;

        }

        public void Insert(int index, T item)
        {

            if(index < 0 || index > Index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            EnsureCapacity(1);

            if(Index < index)
            {
                Array.Copy(Contents, index, Contents, index + 1, Index - index);
            }

            Contents[index] = item;
            Index++;

        }

        public void AddRange(ReadOnlySpan<T> items)
        {

            EnsureCapacity(items.Length);
            items.CopyTo(Contents.AsSpan(Index));

            Index += items.Length;

        }

        public void Clear()
        {

            Contents.AsSpan(0, Index).Clear();
            Index = 0;

        }

        private void EnsureCapacity(int capacity)
        {

            if(capacity > Contents.Length - Index)
            {
                ResizeContents(capacity);
            }

        }

        private void ResizeContents(int capacity)
        {

            int minCapacity = Index + capacity;
            int newCapacity = Math.Max(minCapacity, Contents.Length * 2);

            T[] newContents = new T[newCapacity];
            Array.Copy(Contents, newContents, Index);

            Contents = newContents;

        }

    }
}
