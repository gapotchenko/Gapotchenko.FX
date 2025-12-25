// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Math.Utils;

namespace Gapotchenko.FX.Math;

using Math = System.Math;

/// <summary>
/// Provides extension methods for the <see cref="Math"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class MathExtensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Math"/> class.
    /// </summary>
    extension(Math)
    {
        /// <summary>
        /// Returns a <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <typeparam name="T">The type of the value to clamp.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(value))]
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>?
        {
            if (value is null || min is null || max is null)
                return Clamp(value, min, max, null);

            if (min.CompareTo(max) > 0)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value.CompareTo(min) < 0)
                return min;
            else if (value.CompareTo(max) > 0)
                return max;

            return value;
        }

        /// <summary>
        /// Returns a <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <typeparam name="T">The type of the value to clamp.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <param name="comparer">
        /// The optional comparer.
        /// When this parameter is <see langword="null"/>, a default comparer for type <typeparamref name="T"/> is used.
        /// </param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
        public static T Clamp<T>(T value, T min, T max, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            if (comparer.Compare(min, max) > 0)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (comparer.Compare(value, min) < 0)
                return min;
            else if (comparer.Compare(value, max) > 0)
                return max;

            return value;
        }
    }
}
