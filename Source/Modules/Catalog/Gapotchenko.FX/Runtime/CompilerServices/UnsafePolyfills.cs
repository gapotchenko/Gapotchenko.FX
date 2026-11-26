// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET8_0_OR_GREATER
#define TFF_UNSAFE_BITCAST
#endif

using System.Runtime.CompilerServices;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Provides polyfill members for <see cref="Unsafe"/> type.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class UnsafePolyfills
{
    extension(Unsafe)
    {
        /// <summary>
        /// Reinterprets the given value of type <typeparamref name="TFrom" /> as a value of type <typeparamref name="TTo" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The size of <typeparamref name="TFrom" /> and <typeparamref name="TTo" /> are not the same.
        /// </exception>
#if TFF_UNSAFE_BITCAST
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo BitCast<TFrom, TTo>(TFrom source)
            where TFrom : struct
            where TTo : struct
        {
#if TFF_UNSAFE_BITCAST
            return Unsafe.BitCast<TFrom, TTo>(source);
#else
            unsafe
            {
                if (sizeof(TFrom) != sizeof(TTo))
                    ThrowHelper.ThrowNotSupportedException();
            }
            return Unsafe.ReadUnaligned<TTo>(ref Unsafe.As<TFrom, byte>(ref source));
#endif
        }
    }
}
