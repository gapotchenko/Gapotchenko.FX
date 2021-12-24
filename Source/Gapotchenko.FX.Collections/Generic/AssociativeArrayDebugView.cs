using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic
{
    sealed class AssociativeArrayDebugView<TKey, TValue>
    {
        readonly AssociativeArray<TKey, TValue> _associativeArray;

        public AssociativeArrayDebugView(AssociativeArray<TKey, TValue> associativeArray) =>
            _associativeArray = associativeArray ?? throw new ArgumentNullException(nameof(associativeArray));

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                var items = new KeyValuePair<TKey, TValue>[_associativeArray.Count];
#pragma warning disable CS8714
                ((IDictionary<TKey, TValue>)_associativeArray).CopyTo(items, 0);
#pragma warning restore CS8714
                return items;
            }
        }
    }

    sealed class AssociativeArrayKeyValueCollectionDebugView<T>
    {
        readonly ICollection<T> _collection;

        public AssociativeArrayKeyValueCollectionDebugView(ICollection<T> collection) =>
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {

                var items = new T[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}
