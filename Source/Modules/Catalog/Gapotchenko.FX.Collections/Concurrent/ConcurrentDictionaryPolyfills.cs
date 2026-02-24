// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_CONCURRENTDICTIONARY_GETORADD_FACTORYARG
#endif

using System.Collections.Concurrent;

namespace Gapotchenko.FX.Collections.Concurrent;

/// <summary>
/// Provides polyfill extension methods for <see cref="ConcurrentDictionary{TKey, TValue}"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ConcurrentDictionaryPolyfills
{
    /// <summary>
    /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// by using the specified function and an argument if the key does not already exist,
    /// or returns the existing value if the key exists.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <typeparam name="TArg">The type of an argument to pass into <paramref name="valueFactory"/>.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="valueFactory">The function used to generate a value for the key.</param>
    /// <param name="factoryArgument">An argument value to pass into <paramref name="valueFactory"/>.</param>
    /// <returns>
    /// The value for the key.
    /// This will be either the existing value for the key if the key is already in the dictionary,
    /// or the new value if the key was not in the dictionary.
    /// </returns>
#if TFF_CONCURRENTDICTIONARY_GETORADD_FACTORYARG
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static TValue GetOrAdd<TKey, TValue, TArg>(
#if !TFF_CONCURRENTDICTIONARY_GETORADD_FACTORYARG
        this
#endif
        ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TArg, TValue> valueFactory,
        TArg factoryArgument)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);

#if TFF_CONCURRENTDICTIONARY_GETORADD_FACTORYARG
        return dictionary.GetOrAdd(key, valueFactory, factoryArgument);
#else
        return dictionary.GetOrAdd(key, key => valueFactory(key, factoryArgument));
#endif
    }
}
