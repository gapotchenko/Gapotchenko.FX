using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Generic read-only collection extensions.
    /// </summary>
    public static class ReadOnlyCollectionExtensions
    {
        /// <summary>
        /// Indicates whether the specified collection is null or empty.
        /// </summary>
        /// <param name="value">The collection to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty collection; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IReadOnlyCollection<T>? value) => value is null || value.Count == 0;
    }
}
