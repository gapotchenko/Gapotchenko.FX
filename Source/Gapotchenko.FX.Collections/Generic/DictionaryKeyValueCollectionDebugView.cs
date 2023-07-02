using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic;

sealed class DictionaryKeyValueCollectionDebugView<TKey, TValue, T>
{
    public DictionaryKeyValueCollectionDebugView(ICollection<T> collection)
    {
        m_Collection = collection;
    }

    readonly ICollection<T> m_Collection;

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
