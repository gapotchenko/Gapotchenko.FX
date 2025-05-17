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
    }
}
