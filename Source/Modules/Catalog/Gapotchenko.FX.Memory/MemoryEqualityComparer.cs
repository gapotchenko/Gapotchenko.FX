// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Equality comparer for contiguous regions of memory represented by <see cref="ReadOnlyMemory{T}"/> type.
/// </summary>
public abstract partial class MemoryEqualityComparer :
#pragma warning disable CS0618 // Type or member is obsolete
    _CompatibleMemoryEqualityComparer
#pragma warning restore CS0618 // Type or member is obsolete
{
    // Prevents inheritance.
    MemoryEqualityComparer()
    {
    }

    #region Equals

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("MemoryEqualityComparer.Equals(object, object) method cannot be used. Use MemoryEqualityComparer.Equals<T>(ReadOnlyMemory<T>, ReadOnlyMemory<T>) method instead.", true)]
    public static new bool Equals(object? objA, object? objB) => throw new NotSupportedException();

    /// <summary>
    /// Determines whether two memory regions are equal
    /// by comparing the elements using <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory regions.</typeparam>
    /// <param name="x">The first memory region to compare.</param>
    /// <param name="y">The second memory regions to compare.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the two memory regions are equal; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals<T>(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y, IEqualityComparer<T>? comparer = null)
    {
        return EqualsCore(x, y, comparer);
    }

    /// <summary>
    /// Determines whether two memory regions are equal
    /// by comparing the elements using <see cref="IEquatable{T}.Equals(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory regions.</typeparam>
    /// <param name="x">The first memory region to compare.</param>
    /// <param name="y">The second memory region to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two memory regions are equal; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals<T>(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) where T : IEquatable<T>?
    {
        return EqualsCore(x, y);
    }

    static bool EqualsCore<T>(in ReadOnlyMemory<T> x, in ReadOnlyMemory<T> y) where T : IEquatable<T>?
    {
        return
            DistinguishableEquals(x, y) ??
            SpanEqualityComparer.Equals(x.Span, y.Span);
    }

    static bool EqualsCore<T>(in ReadOnlyMemory<T> x, in ReadOnlyMemory<T> y, IEqualityComparer<T>? comparer = null)
    {
        return
            DistinguishableEquals(x, y) ??
            SpanEqualityComparer.Equals(x.Span, y.Span, comparer);
    }

    static bool? DistinguishableEquals<T>(in ReadOnlyMemory<T> x, in ReadOnlyMemory<T> y)
    {
        if (x.Length != y.Length)
        {
            // Length mismatch.
            return false;
        }

        if (x.Equals(y))
        {
            // Regions point to the same location.
            return true;
        }

        // Inconclusive.
        return null;
    }

    #endregion

    #region GetHashCode

    /// <summary>
    /// Returns a hash code for the specified memory region by combining the hash codes
    /// returned by <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
    /// for its elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory region.</typeparam>
    /// <param name="memory">The memory region to compute a hash code for.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when computing hash codes for elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>A hash code representing the content of the specified memory region.</returns>
    public static int GetHashCode<T>(ReadOnlyMemory<T> memory, IEqualityComparer<T>? comparer = null)
    {
        comparer = Empty.Nullify(comparer);
        if (comparer is null)
            return MemoryEqualityComparer<T>.Default.GetHashCode(memory);
        else
            return GetHashCodeCore(memory, comparer);
    }

    static int GetHashCodeCore<T>(in ReadOnlyMemory<T> memory, IEqualityComparer<T>? comparer = null)
    {
        return SpanEqualityComparer.GetHashCode(memory.Span, comparer);
    }

    static int GetBitwiseHashCodeCore<T>(in ReadOnlyMemory<T> memory) where T : struct
    {
        return SpanEqualityComparer.GetHashCode(MemoryMarshal.AsBytes(memory.Span));
    }

    static int GetHashCodeCore(in ReadOnlyMemory<byte> memory)
    {
        return SpanEqualityComparer.GetHashCode(memory.Span);
    }

    #endregion

    /// <summary>
    /// Creates a new equality comparer for contiguous regions of memory with a specified comparer for memory elements.
    /// </summary>
    /// <typeparam name="T">The type of memory elements.</typeparam>
    /// <param name="elementComparer">The equality comparer for memory elements.</param>
    /// <returns>A new equality comparer for contiguous regions of memory with elements of type <typeparamref name="T"/>.</returns>
    public static MemoryEqualityComparer<T> Create<T>(IEqualityComparer<T>? elementComparer)
    {
        elementComparer = Empty.Nullify(elementComparer);
        if (elementComparer is null)
        {
            return
                Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Byte => (MemoryEqualityComparer<T>)(object)ByteMemoryComparer.Instance,
                    TypeCode.SByte => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<sbyte>.Instance,
                    TypeCode.Int16 => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<short>.Instance,
                    TypeCode.UInt16 => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<ushort>.Instance,
                    TypeCode.Int32 => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<int>.Instance,
                    TypeCode.UInt32 => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<uint>.Instance,
                    TypeCode.Int64 => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<long>.Instance,
                    TypeCode.UInt64 => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<ulong>.Instance,
                    TypeCode.Boolean => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<bool>.Instance,
                    TypeCode.Char => (MemoryEqualityComparer<T>)(object)BitwiseMemoryComparer<char>.Instance,
                    _ => DefaultMemoryComparer<T>.Instance
                };
        }
        else
        {
            return new CustomMemoryComparer<T>(elementComparer);
        }
    }
}
