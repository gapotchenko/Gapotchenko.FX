using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Collections
{
    /// <summary>
    /// Dictionary extensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Indicates whether the specified dictionary is null or empty.
        /// </summary>
        /// <param name="value">The dictionary to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty dictionary; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this IDictionary value) => value == null || value.Count == 0;
    }
}
