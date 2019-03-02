using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Provides static methods for creating <see cref="KeyValuePair{TKey, TValue}"/> objects.
    /// </summary>
    public static class KeyValuePair
    {
        /// <summary>
        /// Creates a new <see cref="KeyValuePair{TKey, TValue}"/> value.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>A new key/value pair.</returns>
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) =>
            new KeyValuePair<TKey, TValue>(key, value);

        /// <summary>
        /// Deconstructs a <see cref="KeyValuePair{TKey, TValue}"/> value.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="pair">The pair.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
