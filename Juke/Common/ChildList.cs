using System.Collections;

namespace Juke.Common;

public class ChildList<T, P> : IList<T>
where T : class, IChild<P> 
where P : class {
    private readonly List<T> _list = [];
    private P? _owner;

    private void Charge(T item) {
        item.Parent = _owner;
    }

    private void Discharge(T item) {
        item.Parent = null;
    }
    
    private P? Owner {
        get => _owner;
        set {
            _owner = value;
            foreach (var item in _list) {
                Charge(item);
            }
        }
    }

    public IEnumerator<T> GetEnumerator() {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return _list.GetEnumerator();
    }
    public void Add(T item) {
        Charge(item);
        _list.Add(item);
    }
    public void Clear() {
        foreach (var item in _list) {
            Discharge(item);
        }
        _list.Clear();
    }
    public bool Contains(T item) {
        return _list.Contains(item);
    }
    public void CopyTo(T[] array, int arrayIndex) {
        _list.CopyTo(array, arrayIndex);
    }
    public bool Remove(T item) {
        Discharge(item);
        return _list.Remove(item);
    }

    public int Count => _list.Count;
    public bool IsReadOnly => false;
    public int IndexOf(T item) {
        return _list.IndexOf(item);
    }
    public void Insert(int index, T item) {
        Charge(item);
        _list.Insert(index, item);
    }
    public void RemoveAt(int index) {
        Discharge(_list[index]);
        _list.RemoveAt(index);
    }

    public T this[int index] {
        get => _list[index];
        set => _list[index] = value;
    }
}