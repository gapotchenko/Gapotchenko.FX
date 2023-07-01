using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic;

sealed class DictionaryDebugView<TKey, TValue>
{
    public DictionaryDebugView(IDictionary<TKey, TValue> dictionary) =>
        m_Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    readonly IDictionary<TKey, TValue> m_Dictionary;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<TKey, TValue>[] Items
    {
        get
        {
            var items = new KeyValuePair<TKey, TValue>[m_Dictionary.Count];
            m_Dictionary.CopyTo(items, 0);
            return items;
        }
    }
}
