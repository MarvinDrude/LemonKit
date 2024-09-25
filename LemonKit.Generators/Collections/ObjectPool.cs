
namespace LemonKit.Generators.Collections;

internal sealed class ObjectPool<T>
    where T : class {

    private readonly Element[] _Items;
    private readonly Func<T> _Factory;
    private T? FirstItem;


    public ObjectPool(Func<T> factory, int capacity) {

        _Factory = factory;
        _Items = new Element[capacity];

    }

    public void Return(T ob) {

        if(FirstItem is null) {

            FirstItem = ob;

        } else {

            ReturnLoop(ob);

        }

    }

    public T Get() {

        T? item = FirstItem;

        if(item is null || item != Interlocked.CompareExchange(ref FirstItem, null, item)) {

            item = GetLoop();

        }

        return item;

    }

    private void ReturnLoop(T ob) {

        foreach(ref Element element in _Items.AsSpan()) {

            if(element.Value is not null) {
                continue;
            }

            element.Value = ob;
            break;

        }

    }

    private T GetLoop() {

        foreach(ref Element element in _Items.AsSpan()) {

            if(element.Value is not { } current) {
                continue;
            }

            if(current == Interlocked.CompareExchange(ref element.Value, null, current)) {
                return current;
            }

        }

        return _Factory();

    }

    private struct Element {

        internal T? Value;

    }

}
