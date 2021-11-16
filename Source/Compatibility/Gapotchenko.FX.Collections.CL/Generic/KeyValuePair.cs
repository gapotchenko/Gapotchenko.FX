using System.Collections.Generic;
using System.ComponentModel;

namespace Gapotchenko.FX.Collections.Generic;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class KeyValuePair
{
    public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) => new(key, value);

    public static void Deconstruct<TKey, TValue>(KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
