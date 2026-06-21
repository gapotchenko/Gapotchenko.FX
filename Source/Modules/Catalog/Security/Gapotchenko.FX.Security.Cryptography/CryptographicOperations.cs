// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
#define TFF_CRYPTOGRAPHIC_OPERATIONS
#endif

#if !TFF_CRYPTOGRAPHIC_OPERATIONS

#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.Runtime.CompilerServices;

namespace System.Security.Cryptography;

/// <summary>
/// Provides methods for common cryptographic operations and reducing side-channel information leakage.
/// </summary>
/// <remarks>
/// This is a polyfill provided by Gapotchenko.FX.
/// </remarks>
public static class CryptographicOperations
{
    /// <summary>
    /// Determine the equality of two byte sequences in an amount of time which depends on
    /// the length of the sequences, but not the values.
    /// </summary>
    /// <param name="left">The first buffer to compare.</param>
    /// <param name="right">The second buffer to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same
    /// values for <see cref="ReadOnlySpan{T}.Length"/> and the same contents,
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method compares two buffers' contents for equality in a manner which does not
    /// leak timing information, making it ideal for use within cryptographic routines.
    /// This method will short-circuit and return <c>false</c> only if <paramref name="left"/>
    /// and <paramref name="right"/> have different lengths.
    /// </para>
    /// <para>
    /// Fixed-time behavior is guaranteed in all other cases, including if <paramref name="left"/>
    /// and <paramref name="right"/> reference the same address.
    /// </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static bool FixedTimeEquals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        // NoOptimization because we want this method to be exactly as non-short-circuiting
        // as written.
        //
        // NoInlining because the NoOptimization would get lost if the method got inlined.

        int length = left.Length;
        if (right.Length != length)
            return false;

        int accumulator = 0;

        for (int i = 0; i < length; i++)
            accumulator |= left[i] - right[i];

        return accumulator == 0;
    }

    /// <summary>
    /// Fills the provided buffer with zeros.
    /// </summary>
    /// <remarks>
    /// This method exists to future-proof against potential optimizations in the .NET runtime that could eliminate memory writes that are not followed by memory reads. 
    /// </remarks>
    /// <param name="buffer">The buffer to fill with zeros.</param>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ZeroMemory(Span<byte> buffer)
    {
        // NoOptimize to prevent the optimizer from deciding this call is unnecessary
        // NoInlining to prevent the inliner from forgetting that the method was no-optimize
        buffer.Clear();
    }
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(CryptographicOperations))]

#endif
