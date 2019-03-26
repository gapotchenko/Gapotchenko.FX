using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Supports an optional value represented by <see cref="Optional{T}"/> type.
    /// This class cannot be inherited.
    /// </summary>
    public static class Optional
    {
        /// <summary>
        /// <para>
        /// Creates a new <see cref="Optional{T}"/> object initialized to a specified value.
        /// </para>
        /// <para>
        /// Use <seealso cref="Optional{T}.None"/> property to get an <see cref="Optional{T}"/> object without a value.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="value">A value.</param>
        /// <returns>
        /// A <see cref="Optional{T}"/> object whose <see cref="Optional{T}.Value"/> property is initialized with the value parameter.
        /// </returns>
        public static Optional<T> Some<T>(T value) => new Optional<T>(value);

        /// <summary>
        /// Determines whether the specified optional values are equal.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="x">The first optional value to compare.</param>
        /// <param name="y">The second optional value to compare.</param>
        /// <param name="valueComparer">The value equality comparer.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public static bool Equals<T>(Optional<T> x, Optional<T> y, IEqualityComparer<T> valueComparer) =>
            OptionalEqualityComparer<T>.EqualsCore(x, y, valueComparer ?? EqualityComparer<T>.Default);

        /// <summary>
        /// Determines whether the specified optional values are equal.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="x">The first optional value to compare.</param>
        /// <param name="y">The second optional value to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public static bool Equals<T>(Optional<T> x, Optional<T> y) => Equals(x, y, null);

        /// <summary>
        /// Returns a hash code for the specified optional value.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <param name="valueComparer">The value equality comparer.</param>
        /// <returns>A hash code for the specified optional value.</returns>
        public static int GetHashCode<T>(Optional<T> option, IEqualityComparer<T> valueComparer) =>
            OptionalEqualityComparer<T>.GetHashCodeCore(option, valueComparer ?? EqualityComparer<T>.Default);

        /// <summary>
        /// Returns a hash code for the specified optional value.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="option">The optional value.</param>
        /// <returns>A hash code for the specified optional value.</returns>
        public static int GetHashCode<T>(Optional<T> option) => GetHashCode(option, null);

        /// <summary>
        /// Compares two optional values using a specified comparer for values.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="x">The first optional value to compare.</param>
        /// <param name="y">The second optional value to compare.</param>
        /// <param name="valueComparer">The value comparer.</param>
        /// <returns>The comparison result.</returns>
        public static int Compare<T>(Optional<T> x, Optional<T> y, IComparer<T> valueComparer) =>
            OptionalComparer<T>.CompareCore(x, y, valueComparer ?? Comparer<T>.Default);

        /// <summary>
        /// Compares two optional values.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="x">The first optional value to compare.</param>
        /// <param name="y">The second optional value to compare.</param>
        /// <returns>The comparison result.</returns>
        public static int Compare<T>(Optional<T> x, Optional<T> y) => Compare(x, y, null);

        /// <summary>
        /// Creates a new comparer for <see cref="Optional{T}"/> values with a specified comparer for <typeparamref name="T"/> values.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="valueComparer">The value comparer.</param>
        /// <returns>A new comparer for <see cref="Optional{T}"/> objects.</returns>
        public static IComparer<Optional<T>> CreateComparer<T>(IComparer<T> valueComparer) =>
            new OptionalComparer<T>(valueComparer);

        /// <summary>
        /// Creates a new equality comparer for <see cref="Optional{T}"/> values with a specified comparer for <typeparamref name="T"/> values.
        /// </summary>
        /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
        /// <param name="valueComparer">The value equality comparer.</param>
        /// <returns>A new equality comparer for <see cref="Optional{T}"/> objects.</returns>
        public static IEqualityComparer<Optional<T>> CreateEqualityComparer<T>(IEqualityComparer<T> valueComparer) =>
            new OptionalEqualityComparer<T>(valueComparer);
    }
}
