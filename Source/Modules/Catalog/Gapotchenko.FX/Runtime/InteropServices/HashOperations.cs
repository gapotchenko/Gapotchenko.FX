// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.IO.Hashing;

namespace Gapotchenko.FX.Runtime.InteropServices;

/// <summary>
/// Provides process-local hashing functions.
/// </summary>
/// <remarks>
/// The functions provided by this type calculate process-local hash values which is not guaranteed to be stable outside of the currently running process.
/// </remarks>
public static class HashOperations
{
    /// <summary>
    /// Calculates a 32-bit hash value for the specified span of bytes.
    /// </summary>
    /// <remarks>
    /// The function calculates a process-local hash value which is not guaranteed to be stable outside of the currently running process.
    /// </remarks>
    /// <param name="source">The span of bytes.</param>
    /// <returns>A calculated 32-bit hash value.</returns>
    public static int GetHashCode(ReadOnlySpan<byte> source)
    {
        return (int)XxHash3.HashToUInt64(source);
    }

    /// <summary>
    /// Calculates a 32-bit hash value for the specified span of elements.
    /// </summary>
    /// <remarks>
    /// The function calculates a process-local hash value which is not guaranteed to be stable outside of the currently running process.
    /// </remarks>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The span of elements.</param>
    /// <param name="comparer">The comparer used to obtain the element hash code.</param>
    /// <returns>A calculated 32-bit hash value.</returns>
    public static int GetHashCode<T>(ReadOnlySpan<T> source, IEqualityComparer<T>? comparer = null)
    {
        // The function is dominated by obtaining hash codes from the elements.
        // Any attempt to use a hash function tailored for processing of contiguous memory blocks
        // is going to be slower than a simple streaming hash algorithm.
        // Buffer preparation is going to eat up all potential benefits.

        // Uses a simple custom hash algorithm inspired by FNV-1a.

        uint hash = 2166136261;

        if (comparer is null)
        {
            foreach (var i in source)
            {
                int hashCode = i?.GetHashCode() ?? 0;
                hash = (hash ^ (uint)hashCode) * 16777619;
            }
        }
        else
        {
            foreach (var i in source)
            {
                int hashCode = i is null ? 0 : comparer.GetHashCode(i);
                hash = (hash ^ (uint)hashCode) * 16777619;
            }
        }

        return (int)hash;
    }
}
