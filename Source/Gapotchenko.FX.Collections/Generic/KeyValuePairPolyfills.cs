#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Collections.Generic;

/// <summary>
/// Provides polyfill methods for <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class KeyValuePairPolyfills
{
#if !TFF_KEYVALUEPAIR || BINARY_COMPATIBILITY
    /// <summary>
    /// Deconstructs a <see cref="KeyValuePair{TKey, TValue}"/> value.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="pair">The pair.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Deconstruct<TKey, TValue>(
#if !TFF_KEYVALUEPAIR
        this
#endif
        KeyValuePair<TKey, TValue> pair,
        out TKey key,
        out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
#endif
}
