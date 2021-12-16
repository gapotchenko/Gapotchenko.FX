using System;
using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    sealed class NotNullList<T> : IList<T>
        where T : class
    {
        readonly List<T> _list = new();

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public void Add(T item) => _list.Add(item ?? throw new ArgumentNullException(nameof(item)));

        public void Clear() => _list.Clear();

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item) => _list.Insert(index, item ?? throw new ArgumentNullException(nameof(item)));

        public bool Remove(T item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
