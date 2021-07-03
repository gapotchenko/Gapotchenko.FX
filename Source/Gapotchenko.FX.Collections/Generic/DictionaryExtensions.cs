using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Generic dictionary extensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to add the specified key and value to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary with keys of type <typeparamref name="TKey"/> and values of type <typeparamref name="TValue"/>.</param>
        /// <param name="key">The key of the value to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>
        /// <c>true</c> when the key and value are successfully added to the dictionary;
        /// <c>false</c> when the dictionary already contains the specified key, in which case nothing gets added.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
#if TFF_DICTIONARY_TRYADD
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static bool TryAdd<TKey, TValue>(
#if !TFF_DICTIONARY_TRYADD
            this
#endif
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
#if !TFF_DICTIONARY_NULL_KEY
            where TKey : notnull
#endif
#if TFF_DICTIONARY_TRYADD
            => dictionary.TryAdd(key, value);
#else
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.ContainsKey(key))
                return false;

            dictionary.Add(key, value);
            return true;
        }
#endif

        /// <summary>
        /// Tries to remove the value with the specified key from the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">A dictionary with keys of type <typeparamref name="TKey"/> and values of type <typeparamref name="TValue"/>.</param>
        /// <param name="key">The key of the value to remove.</param>
        /// <param name="value">
        /// When this method returns <c>true</c>, the removed value;
        /// when this method returns <c>false</c>, the default value for <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> when a value is found in the dictionary with the specified key;
        /// <c>false</c> when the dictionary cannot find a value associated with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
#if TFF_DICTIONARY_REMOVEANDGETVALUE
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static bool Remove<TKey, TValue>(
#if !TFF_DICTIONARY_REMOVEANDGETVALUE
            this
#endif
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            [MaybeNullWhen(false)] out TValue value)
#if !TFF_DICTIONARY_NULL_KEY
            where TKey : notnull
#endif
#if TFF_DICTIONARY_REMOVEANDGETVALUE
            => dictionary.Remove(key, out value);
#else
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            value = default;
            return false;
        }
#endif
    }
}
