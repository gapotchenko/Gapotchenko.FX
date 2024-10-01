namespace Gapotchenko.FX.Linq.Expressions;

static class Util
{
    public static IEnumerable<KeyValuePair<TKey, TValue>> Zip<TKey, TValue>(this IEnumerable<TKey> keys, IEnumerable<TValue> values) =>
        keys.Zip(values, (key, value) => new KeyValuePair<TKey, TValue>(key, value));
}
