// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides polyfill extensions for <see cref="CryptographicOperations"/> class.
/// </summary>
public static class CryptographicOperationsPolyfills
{
    extension(CryptographicOperations)
    {
        /// <summary>
        /// Fills the provided buffer with default <typeparamref name="T"/> values.
        /// </summary>
        /// <remarks>
        /// This method exists to future-proof against potential optimizations in the .NET runtime that could eliminate memory writes that are not followed by memory reads. 
        /// </remarks>
        /// <param name="buffer">The buffer to fill with default <typeparamref name="T"/> values.</param>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void ZeroMemory<T>(Span<T> buffer)
        {
            // NoOptimize to prevent the optimizer from deciding this call is unnecessary
            // NoInlining to prevent the inliner from forgetting that the method was no-optimize
            buffer.Clear();
        }
    }
}
