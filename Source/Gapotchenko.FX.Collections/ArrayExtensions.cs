using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Gapotchenko.FX.Collections
{
    /// <summary>
    /// Array extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Indicates whether the specified array is <c>null</c> or empty.
        /// </summary>
        /// <param name="value">The array to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty array; otherwise, <c>false</c>.</returns>        
        public static bool IsNullOrEmpty([NotNullWhen(false)] this Array? value) => value is null || value.Length == 0;
    }
}
