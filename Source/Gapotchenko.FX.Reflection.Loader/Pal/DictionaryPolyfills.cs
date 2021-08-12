using System.Collections.Generic;

namespace Gapotchenko.FX.Reflection.Pal
{
    static class DictionaryPolyfills
    {
#if !NETCOREAPP
        public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (!dictionary.TryGetValue(key, out value))
                return false;
            dictionary.Remove(key);
            return true;
        }
#endif
    }
}
