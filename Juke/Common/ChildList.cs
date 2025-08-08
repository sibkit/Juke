using System.Collections;
using Juke.Querying;

namespace Juke.Common;


public class ChildList<P,T> : IList<T>
    where P: QueryElement
    where T: QueryElement, IChild<P> {
    private readonly IList<T> _list = [];

    public ChildList(P owner) {
        Owner = owner;
    }
    private void Charge(T item) {
        item.Parent = Owner;
    }

    private void Discharge(T item) {
        item.Parent = null;
    }
    
    public P Owner { get; }

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