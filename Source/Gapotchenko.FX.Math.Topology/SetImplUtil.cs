using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    static class SetImplUtil
    {
        public static void UnionWith<TSet, T>(TSet s, IEnumerable<T> other) where TSet : ISet<T>
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (var i in other)
                s.Add(i);
        }

        public static void CopyTo<TSet, T>(TSet s, T[] array, int arrayIndex) where TSet : IReadOnlySet<T>
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            foreach (var i in s)
                array[arrayIndex++] = i;
        }
    }
}
