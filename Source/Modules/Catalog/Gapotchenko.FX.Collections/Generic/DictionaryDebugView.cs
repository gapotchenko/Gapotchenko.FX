using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic;

sealed class DictionaryDebugView<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<TKey, TValue>[] Items
    {
        get
        {
            var items = new KeyValuePair<TKey, TValue>[dictionary.Count];
            dictionary.CopyTo(items, 0);
            return items;
        }
    }
}
