using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Collections
{
    /// <summary>
    /// Collection extensions.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Indicates whether the specified collection is null or empty.
        /// </summary>
        /// <param name="value">The collection to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty collection; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this ICollection value) => value == null || value.Count == 0;
    }
}
