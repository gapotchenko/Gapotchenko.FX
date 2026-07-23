// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.InteropServices;

/// <summary>
/// Provides operations for calculating process-local hash codes.
/// </summary>
/// <remarks>
/// <para>
/// Hash codes produced by this type are intended for use only within the
/// currently running process.
/// </para>
/// <para>
/// The values are not guaranteed to remain stable across process executions,
/// runtime versions, operating systems, processor architectures, or library
/// versions, and therefore should not be persisted or used as external
/// identifiers.
/// </para>
/// </remarks>
public static class HashOperations
{
    /// <summary>
    /// Calculates a 32-bit process-local hash code for a sequence of elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The sequence of elements to hash.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when computing hash codes for elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>A 32-bit hash code for <paramref name="source"/>.</returns>
    /// <remarks>
    /// The returned value is not guaranteed to be stable outside the
    /// currently running process and should not be persisted.
    /// </remarks>
    public static int GetHashCode<T>(ReadOnlySpan<T> source, IEqualityComparer<T>? comparer = null)
    {
        // Obtaining hash codes from individual elements generally dominates
        // this operation. Buffering those hash codes for a block-oriented hash
        // algorithm adds memory traffic and overhead without a corresponding
        // throughput benefit.

        // Use a simple custom hash algorithm inspired by FNV-1a.

        comparer = Empty.Nullify(comparer);
        if (comparer is null)
        {
            if (CodeSafetyStrategy.UnsafeCodeAllowed && typeof(T).IsPrimitive)
            {
                // Process blittable types using an accelerated path.
                return GetBitwiseHashCodeCore(source);
            }
            else
            {
                uint hash = 2166136261;
                foreach (var i in source)
                {
                    int hashCode = i?.GetHashCode() ?? 0;
                    hash = (hash ^ (uint)hashCode) * 16777619;
                }
                return (int)hash;
            }
        }
        else
        {
            uint hash = 2166136261;
            foreach (var i in source)
            {
                int hashCode = i is null ? 0 : comparer.GetHashCode(i);
                hash = (hash ^ (uint)hashCode) * 16777619;
            }
            return (int)hash;
        }
    }

    static unsafe int GetBitwiseHashCodeCore<T>(ReadOnlySpan<T> source)
    {
        var bytes = new ReadOnlySpan<byte>(
            Unsafe.AsPointer(ref MemoryMarshal.GetReference(source)),
            checked(source.Length * Marshal.SizeOf<T>()));
        return GetHashCode(bytes);
    }

    /// <summary>
    /// Calculates a 32-bit process-local hash code for a sequence of bytes.
    /// </summary>
    /// <param name="source">The byte sequence to hash.</param>
    /// <returns>A 32-bit hash code for <paramref name="source"/>.</returns>
    /// <remarks>
    /// The returned value is not guaranteed to be stable outside the
    /// currently running process and should not be persisted.
    /// </remarks>
    public static int GetHashCode(ReadOnlySpan<byte> source)
    {
#if NET
        // On 32-bit processes, XXHash32 is faster for short inputs.
        // Benchmarks show the crossover with XXHash3 at approximately 384 bytes.
        if (IntPtr.Size >= 8 || source.Length >= 384)
            return (int)XxHash3.HashToUInt64(source);
        else
            return (int)XxHash32.HashToUInt32(source);
#else
        // On .NET Framework, XXHash3 is the fastest hash function according to benchmarks.
        return (int)XxHash3.HashToUInt64(source);
#endif
    }
}
