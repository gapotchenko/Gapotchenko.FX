using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Memory
{
    /// <summary>
    /// Equality comparer for contiguous regions of memory represented by <see cref="ReadOnlyMemory{T}"/> type.
    /// </summary>
    public static partial class MemoryEqualityComparer
    {
        /// <summary>
        /// Determines whether specified read-only memory regions are equal.
        /// </summary>
        /// <typeparam name="T">The memory element type.</typeparam>
        /// <param name="x">The first read-only memory region to compare.</param>
        /// <param name="y">The second read-only memory region to compare.</param>
        /// <returns><c>true</c> if the specified read-only memory regions are equal; otherwise, <c>false</c>.</returns>
        public static bool Equals<T>(in ReadOnlyMemory<T> x, in ReadOnlyMemory<T> y) => MemoryEqualityComparer<T>.Default.Equals(x, y);

        /// <summary>
        /// Returns a hash code for specified read-only memory region.
        /// </summary>
        /// <typeparam name="T">The memory element type.</typeparam>
        /// <param name="memory">The memory.</param>
        /// <returns>A hash code for the specified array.</returns>
        public static int GetHashCode<T>(in ReadOnlyMemory<T> memory) => MemoryEqualityComparer<T>.Default.GetHashCode(memory);

        /// <summary>
        /// Creates a new equality comparer for contiguous regions of memory with a specified comparer for memory elements.
        /// </summary>
        /// <typeparam name="T">The type of memory elements.</typeparam>
        /// <param name="elementComparer">The equality comparer for memory elements.</param>
        /// <returns>A new equality comparer for contiguous regions of memory with elements of type <typeparamref name="T"/>.</returns>
        public static MemoryEqualityComparer<T> Create<T>(IEqualityComparer<T>? elementComparer)
        {
            var type = typeof(T);
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte when IsDefaultEqualityComparer(elementComparer) => (new ByteComparer() as MemoryEqualityComparer<T>)!,
                _ when typeof(IEquatable<T>).IsAssignableFrom(type) && IsDefaultEqualityComparer(elementComparer) => (MemoryEqualityComparer<T>)Activator.CreateInstance(typeof(EquatableComparer<>).MakeGenericType(type))!,
                _ => new DefaultComparer<T>(elementComparer)
            };
        }

        static bool IsDefaultEqualityComparer<T>(IEqualityComparer<T>? comparer) =>
            comparer is null ||
            comparer == EqualityComparer<T>.Default;
    }
}
