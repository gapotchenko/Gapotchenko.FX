using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math
{
    partial class MathEx
    {
        /// <summary>
        /// Returns a <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <typeparam name="T">The type of value to clamp.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>
        /// -or-
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>
        /// -or-
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
        [return: NotNullIfNotNull("value")]
        public static T? Clamp<T>(T? value, T? min, T? max) where T : IComparable<T>
        {
            if (min == null && max == null)
                return value;

            if (min != null && max != null && min.CompareTo(max) > 0)
                ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value == null)
                return value;

            if (min != null && value.CompareTo(min) < 0)
                return min;
            else if (max != null && value.CompareTo(max) > 0)
                return max;

            return value;
        }

        /// <summary>
        /// Returns a <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <typeparam name="T">The type of value to clamp.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <param name="comparer">
        /// The optional comparer.
        /// When this parameter is <c>null</c>, a default comparer for type <typeparamref name="T"/> is used.
        /// </param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>
        /// -or-
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>
        /// -or-
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
        [return: NotNullIfNotNull("value")]
        public static T? Clamp<T>(T? value, T? min, T? max, IComparer<T>? comparer)
        {
            if (min == null && max == null)
                return value;

            comparer ??= Comparer<T>.Default;

            if (min != null && max != null && comparer.Compare(min, max) > 0)
                ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value == null)
                return value;

            if (min != null && comparer.Compare(value, min) < 0)
                return min;
            else if (max != null && comparer.Compare(value, max) > 0)
                return max;

            return value;
        }

        [DoesNotReturn]
        static void ThrowMinCannotBeGreaterThanMaxException<T>(T min, T max) =>
            throw new ArgumentException(string.Format(Properties.Resources.MinCannotBeGreaterThanMax, min, max));
    }
}
