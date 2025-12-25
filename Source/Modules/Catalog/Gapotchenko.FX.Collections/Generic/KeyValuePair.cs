using System.Runtime.CompilerServices;

#if !TFF_KEYVALUEPAIR

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Collections.Generic;

/// <summary>
/// <para>
/// Creates instances of the <see cref="KeyValuePair{TKey, TValue}"/> structure.
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

#else

[assembly: TypeForwardedTo(typeof(KeyValuePair))]

#endif
