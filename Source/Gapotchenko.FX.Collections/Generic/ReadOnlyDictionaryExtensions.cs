namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Generic read-only dictionary extensions.
/// </summary>
public static class ReadOnlyDictionaryExtensions
{
    /// <summary>
    /// Tries to get the value associated with the specified key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">A dictionary with keys of type <typeparamref name="TKey"/> and values of type <typeparamref name="TValue"/>.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>
    /// A <typeparamref name="TValue"/> instance.
    /// When the method is successful, the returned object is the value associated with the specified key.
    /// When the method fails, it returns the default value for <typeparamref name="TValue"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
#if TFF_DICTIONARY_GETVALUEORDEFAULT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static TValue? GetValueOrDefault<TKey, TValue>(
#if !TFF_DICTIONARY_GETVALUEORDEFAULT
        this
#endif
        IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key)
#if !TFF_DICTIONARY_NULL_KEY
        where TKey : notnull
#endif
#if TFF_DICTIONARY_GETVALUEORDEFAULT
        => dictionary.GetValueOrDefault(key);
#else
    {
        return GetValueOrDefault(dictionary, key, default!);
    }
#endif

    /// <summary>
    /// Tries to get the value associated with the specified key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">A dictionary with keys of type <typeparamref name="TKey"/> and values of type <typeparamref name="TValue"/>.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="defaultValue">The default value to return when the dictionary cannot find a value associated with the specified key.</param>
    /// <returns>
    /// A <typeparamref name="TValue"/> instance.
    /// When the method is successful, the returned object is the value associated with the specified key.
    /// When the method fails, it returns <paramref name="defaultValue"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
#if TFF_DICTIONARY_GETVALUEORDEFAULT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
#if NETCOREAPP3_0
    [return: MaybeNull]
#endif
    public static TValue GetValueOrDefault<TKey, TValue>(
#if !TFF_DICTIONARY_GETVALUEORDEFAULT
        this
#endif
        IReadOnlyDictionary<
            TKey,
            TValue
#if NETCOREAPP3_0
            ?
#endif
            >
            dictionary,
        TKey key,
#if NETCOREAPP3_0
        [AllowNull]
#endif
        TValue defaultValue)
#if !TFF_DICTIONARY_NULL_KEY
        where TKey : notnull
#endif
#if TFF_DICTIONARY_GETVALUEORDEFAULT
        => dictionary.GetValueOrDefault(key, defaultValue);
#else
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));

        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
#endif
}
