﻿namespace Gapotchenko.FX.Reflection.Loader.Polyfills;

static class DictionaryPolyfills
{
#if !TFF_DICTIONARY_REMOVEANDGETVALUE
    public static bool Remove<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        [MaybeNullWhen(false)] out TValue value)
    {
        if (!dictionary.TryGetValue(key, out value))
            return false;
        dictionary.Remove(key);
        return true;
    }
#endif
}
