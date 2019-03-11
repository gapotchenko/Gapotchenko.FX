using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Collections
{
    /// <summary>
    /// Array extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Indicates whether the specified array is null or empty.
        /// </summary>
        /// <param name="value">The array to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty array; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this Array value) => value == null || value.Length == 0;
    }
}
