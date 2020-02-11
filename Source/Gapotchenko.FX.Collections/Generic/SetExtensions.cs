using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Generic set extensions.
    /// </summary>
    public static class SetExtensions
    {
        /// <summary>
        /// Indicates whether the specified set is null or empty.
        /// </summary>
        /// <param name="value">The set to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty set; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(
#if !TFF_HASHSET_IREADONLYCOLLECTION
            this
#endif
            ISet<T> value) =>
            value == null || value.Count == 0;
    }
}
