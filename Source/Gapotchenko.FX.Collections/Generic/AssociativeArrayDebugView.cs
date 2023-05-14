using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic;

sealed class AssociativeArrayDebugView<TKey, TValue>
{
    readonly AssociativeArray<TKey, TValue> m_AssociativeArray;

    public AssociativeArrayDebugView(AssociativeArray<TKey, TValue> associativeArray) =>
        m_AssociativeArray = associativeArray ?? throw new ArgumentNullException(nameof(associativeArray));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<TKey, TValue>[] Items
    {
        get
        {
            var items = new KeyValuePair<TKey, TValue>[m_AssociativeArray.Count];
#pragma warning disable CS8714
            ((IDictionary<TKey, TValue>)m_AssociativeArray).CopyTo(items, 0);
#pragma warning restore CS8714
            return items;
        }
    }
}

sealed class AssociativeArrayKeyValueCollectionDebugView<TKey, TValue, T>
{
    readonly ICollection<T> m_Collection;

    public AssociativeArrayKeyValueCollectionDebugView(ICollection<T> collection) =>
        m_Collection = collection ?? throw new ArgumentNullException(nameof(collection));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
        get
        {

            var items = new T[m_Collection.Count];
            m_Collection.CopyTo(items, 0);
            return items;
        }
    }
}
