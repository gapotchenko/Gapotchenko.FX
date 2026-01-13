// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_ARRAY_FILL
#endif

#if NET6_0_OR_GREATER
#define TFF_ARRAY_CLEAR
#define TFF_ARRAY_MAXLENGTH
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension methods for <see cref="Array"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ArrayPolyfills
{
    /// <inheritdoc cref="ArrayPolyfills"/>
    extension(Array)
    {
        /// <summary>
        /// Assigns the given <paramref name="value"/> of type <typeparamref name="T"/> to each element of the specified <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="value">The value to assign to each array element.</param>
#if TFF_ARRAY_FILL
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Fill<T>(T[] array, T value)
        {
#if TFF_ARRAY_FILL
            Array.Fill(array, value);
#else
            ArgumentNullException.ThrowIfNull(array);

            if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
            {
                for (int i = 0; i < array.Length; ++i)
                    array[i] = value;
            }
            else
            {
                array.AsSpan().Fill(value);
            }
#endif
        }

        /// <summary>
        /// Assigns the given <paramref name="value"/> of type <typeparamref name="T"/> to the elements of the specified <paramref name="array"/>
        /// that are within the range of <paramref name="startIndex"/> (inclusive) and the next <paramref name="count"/> number of indices.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="value">The new value for the elements in the specified range.</param>
        /// <param name="startIndex">A 32-bit integer that represents the index in <paramref name="array"/> at which filling begins.</param>
        /// <param name="count">The number of elements to copy.</param>
#if TFF_ARRAY_FILL
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Fill<T>(T[] array, T value, int startIndex, int count)
        {
#if TFF_ARRAY_FILL
            Array.Fill(array, value, startIndex, count);
#else
            ArgumentNullException.ThrowIfNull(array);
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)startIndex, (uint)array.Length, nameof(startIndex));
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)count, (uint)(array.Length - startIndex), nameof(count));

            if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
            {
                for (int i = startIndex, n = startIndex + count; i < n; ++i)
                    array[i] = value;
            }
            else
            {
                array.AsSpan(startIndex, count).Fill(value);
            }
#endif
        }

        /// <summary>
        /// Clears the contents of an array.
        /// </summary>
        /// <param name="array">The array to clear.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
#if TFF_ARRAY_CLEAR
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Clear(Array array)
        {
#if TFF_ARRAY_CLEAR
            Array.Clear(array);
#else
            ArgumentNullException.ThrowIfNull(array);
            Array.Clear(array, 0, array.Length);
#endif
        }

        /// <summary>
        /// Gets the maximum number of elements that may be contained in an array.
        /// </summary>
        /// <value>
        /// The maximum count of elements allowed in any array.
        /// </value>
        /// <remarks>
        /// <para></para>
        /// This property represents a runtime limitation, the maximum number of elements (not bytes) the runtime will allow in an array.
        /// There is no guarantee that an allocation under this length will succeed, but all attempts to allocate a larger array will fail.
        /// <para>
        /// This property only applies to single-dimension, zero-bound (SZ) arrays.
        /// <see cref="Array.Length"/> property may return larger value than this property for multi-dimensional arrays.
        /// </para>
        /// </remarks>
#if TFF_ARRAY_MAXLENGTH
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static int MaxLength =>
#if TFF_ARRAY_MAXLENGTH
            Array.MaxLength;
#else
            0x7fffffc7; // the value is hardcoded in .NET BCL
#endif
    }
}
