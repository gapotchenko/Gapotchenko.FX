// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Diagnostics;

namespace Gapotchenko.FX.Memory;

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
    /// <returns><see langword="true"/> if the specified read-only memory regions are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals<T>(in ReadOnlyMemory<T> x, in ReadOnlyMemory<T> y) => MemoryEqualityComparer<T>.Default.Equals(x, y);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("MemoryEqualityComparer.Equals(object, object) method cannot be used. Use MemoryEqualityComparer.Equals<T>(ReadOnlyMemory<T>, ReadOnlyMemory<T>) method instead.", true)]
    public static new bool Equals(object? objA, object? objB) => throw new NotSupportedException();

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
        if (Empty.Nullify(elementComparer) is null)
        {
            return
                Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Byte => (MemoryEqualityComparer<T>)(object)ByteMemoryComparer.Instance,
                    TypeCode.SByte => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<sbyte>.Instance,
                    TypeCode.UInt16 => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<ushort>.Instance,
                    TypeCode.Int16 => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<short>.Instance,
                    TypeCode.UInt32 => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<uint>.Instance,
                    TypeCode.Int32 => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<int>.Instance,
                    TypeCode.Int64 => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<long>.Instance,
                    TypeCode.UInt64 => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<ulong>.Instance,
                    TypeCode.Boolean => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<bool>.Instance,
                    TypeCode.Char => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<char>.Instance,
                    TypeCode.Decimal => (MemoryEqualityComparer<T>)(object)StructMemoryComparer<decimal>.Instance,
                    _ => DefaultMemoryComparer<T>.Instance
                };
        }
        else
        {
            Debug.Assert(elementComparer != null);
            return new CustomMemoryComparer<T>(elementComparer);
        }
    }
}
