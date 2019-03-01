using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides extended functionality for building, combining and diffusing hash codes.
    /// </summary>
    public static class HashCodeEx
    {
        /// <summary>
        /// Combines hash codes of the elements of the specified collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The collection of elements. The collection itself can be null, and it can contain elements that are null.</param>
        public static int Combine<T>(IEnumerable<T> source)
        {
            var hc = new HashCode();
            hc.AddRange(source);
            return hc.ToHashCode();
        }

        /// <summary>
        /// Adds hash codes of the elements of the specified collection to this instance.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="hashCode">The hash code.</param>
        /// <param name="source">The collection of elements. The collection itself can be null, and it can contain elements that are null.</param>
        public static void AddRange<T>(this ref HashCode hashCode, IEnumerable<T> source)
        {
            if (source != null)
                foreach (var i in source)
                    hashCode.Add(i);
        }

        /// <summary>
        /// Adds hash codes of the elements of the specified collection to this instance.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="hashCode">The hash code.</param>
        /// <param name="source">The collection of elements. The collection itself can be null, and it can contain elements that are null.</param>
        /// <param name="comparer">The equality comparer.</param>
        public static void AddRange<T>(this ref HashCode hashCode, IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            if (source != null)
                foreach (var i in source)
                    hashCode.Add(i, comparer);
        }
    }
}
