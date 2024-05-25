using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IComparable = System.IComparable;

namespace CofyEngine
{
    public class SortedSeq<T>: IReadOnlyList<T> 
    {
        private Func<T, IComparable> _keyFn;
        private SortedList<IComparable, T> _sorted;
        public int Count => _sorted.Count;
        public bool IsReadOnly { get; }

        public SortedSeq(Func<T, IComparable> keyFn, bool ascending = true)
        {
            this._sorted = new SortedList<IComparable, T>(
                Comparer<IComparable>.Create((x, y) => ascending ? x.CompareTo(y) : y.CompareTo(x))
                );
            this._keyFn = keyFn;
        }

        public void Add(T newEl) { _sorted.Add(_keyFn(newEl), newEl); }

        public void Clear() { _sorted.Clear(); }

        public bool Contains(T item) { return _sorted.ContainsValue(item); }

        public T FirstOrDefault() { return _sorted.FirstOrDefault().Value; }

        public int IndexOf(T item) { return _sorted.IndexOfValue(item); }

        public void RemoveAt(int index) { _sorted.RemoveAt(index); }
        
        public bool Remove(T item)
        {
            return _sorted.Values.Remove(item);
        }

        public IEnumerator<T> GetEnumerator() { return _sorted.Values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public T this[int index]
        {
            get => _sorted.Values[index];
        }
    }
}