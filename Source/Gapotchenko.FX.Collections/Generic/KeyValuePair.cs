using System.Runtime.CompilerServices;

#if TFF_KEYVALUEPAIR
[assembly: TypeForwardedTo(typeof(System.Collections.Generic.KeyValuePair))]
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Collections.Generic;

#if !TFF_KEYVALUEPAIR

/// <summary>
/// <para>
/// Creates instances of the <see cref="KeyValuePair{TKey, TValue}"/> struct.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
public static class KeyValuePair
{
    /// <summary>
    /// Creates a new key/value pair instance using provided values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="key">The key of the new <see cref="KeyValuePair{TKey, TValue}" /> to be created.</param>
    /// <param name="value">The value of the new <see cref="KeyValuePair{TKey, TValue}" /> to be created.</param>
    /// <returns>A key/value pair containing the provided arguments as values.</returns>
    public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) => new(key, value);
}

#endif

/// <summary>
/// <see cref="KeyValuePair{TKey, TValue}"/> polyfills.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class KeyValuePairPolyfills
{
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
        KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
