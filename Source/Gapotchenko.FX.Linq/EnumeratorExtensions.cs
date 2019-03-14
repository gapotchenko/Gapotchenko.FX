using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    /// <summary>
    /// Enumerator extensions.
    /// </summary>
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// Returns the enumeration of remaining elements for the given enumerator.
        /// </summary>
        /// <typeparam name="T">The type of elements in enumerator.</typeparam>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> that contains the remaining elements of the given enumerator.</returns>
        public static IEnumerable<T> Rest<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}
