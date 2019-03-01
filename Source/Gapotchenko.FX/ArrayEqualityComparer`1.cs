using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Optimized and fast equality comparer for one-dimensional arrays.
    /// </summary>
    public sealed partial class ArrayEqualityComparer<T> : EqualityComparer<T[]>
    {
        internal ArrayEqualityComparer(IEqualityComparer<T> elementEqualityComparer)
        {
            _ElementEqualityComparer = elementEqualityComparer ?? EqualityComparer<T>.Default;
        }

        readonly IEqualityComparer<T> _ElementEqualityComparer;

        /// <summary>
        /// Determines whether the specified arrays are equal.
        /// </summary>
        /// <param name="x">The first array to compare.</param>
        /// <param name="y">The second array to compare.</param>
        /// <returns><c>true</c> if the specified arrays are equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(T[] x, T[] y)
        {
            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
                if (!_ElementEqualityComparer.Equals(x[i], y[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns a hash code for the specified array.
        /// </summary>
        /// <param name="obj">The array for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified array.</returns>
        public override int GetHashCode(T[] obj)
        {
            if (obj == null)
                return 0;

            // FNV-1a
            uint hash = 2166136261;
            foreach (var i in obj)
                hash = (hash ^ (uint)_ElementEqualityComparer.GetHashCode(i)) * 16777619;
            return (int)hash;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="ArrayEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="ArrayEqualityComparer{T}"/>.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="ArrayEqualityComparer{T}"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is ArrayEqualityComparer<T>;

        /// <summary>
        /// Returns a hash code for <see cref="ArrayEqualityComparer{T}"/>.
        /// </summary>
        /// <returns>A hash code for <see cref="ArrayEqualityComparer{T}"/>.</returns>
        public override int GetHashCode() => GetType().Name.GetHashCode();

        static class DefaultFactory
        {
            public static readonly IEqualityComparer<T[]> Instance = ArrayEqualityComparer.Create<T>(null);
        }

        /// <summary>
        /// Returns a default equality comparer for one-dimensional array with an element type specified by the generic argument <typeparamref name="T"/>.
        /// </summary>
        public new static IEqualityComparer<T[]> Default => DefaultFactory.Instance;
    }
}
