// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial struct ValueInterval<T>
{
    /// <inheritdoc/>
    public int CompareTo(T? value) => IntervalEngine.CompareTo(this, value, Comparer<T>.Default);

    /// <summary>
    /// Determines whether the left specified <see cref="ValueInterval{T}"/> precedes
    /// the right specified <typeparamref name="T"/> value in the sort order.
    /// </summary>
    /// <param name="left">The left <see cref="ValueInterval{T}"/>.</param>
    /// <param name="right">The right <typeparamref name="T"/> value.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> interval precedes <paramref name="right"/> value in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(ValueInterval<T> left, T? right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> value precedes
    /// the right specified <see cref="ValueInterval{T}"/> in the sort order.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> value.</param>
    /// <param name="right">The right <see cref="ValueInterval{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> value precedes <paramref name="right"/> interval in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(T? left, ValueInterval<T> right) => right.CompareTo(left) > 0;

    /// <summary>
    /// Determines whether the left specified <see cref="ValueInterval{T}"/> follows
    /// the right specified <typeparamref name="T"/> value in the sort order.
    /// </summary>
    /// <param name="left">The left <see cref="ValueInterval{T}"/>.</param>
    /// <param name="right">The right <typeparamref name="T"/> value.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> interval follows <paramref name="right"/> value in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(ValueInterval<T> left, T? right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> value follows
    /// the right specified <see cref="ValueInterval{T}"/> in the sort order.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> value.</param>
    /// <param name="right">The right <see cref="ValueInterval{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> value follows <paramref name="right"/> interval in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(T? left, ValueInterval<T> right) => right.CompareTo(left) < 0;

    /// <summary>
    /// Determines whether the left specified <see cref="ValueInterval{T}"/> precedes
    /// or occurs in the same position in the sort order as
    /// the right specified <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="left">The left <see cref="ValueInterval{T}"/>.</param>
    /// <param name="right">The right <typeparamref name="T"/> value.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> interval precedes or occurs in the same position in the sort order as <paramref name="right"/> value;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(ValueInterval<T> left, T? right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> value precedes
    /// or occurs in the same position in the sort order as
    /// the right specified <see cref="ValueInterval{T}"/>.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> value.</param>
    /// <param name="right">The right <see cref="ValueInterval{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> value precedes or occurs in the same position in the sort order as <paramref name="right"/> interval;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(T? left, ValueInterval<T> right) => right.CompareTo(left) >= 0;

    /// <summary>
    /// Determines whether the left specified <see cref="ValueInterval{T}"/> follows
    /// or occurs in the same position in the sort order as
    /// the right specified <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="left">The left <see cref="ValueInterval{T}"/>.</param>
    /// <param name="right">The right <typeparamref name="T"/> value.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> interval follows or occurs in the same position in the sort order as <paramref name="right"/> value;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(ValueInterval<T> left, T? right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> value follows
    /// or occurs in the same position in the sort order as
    /// the right specified <see cref="ValueInterval{T}"/>.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> value.</param>
    /// <param name="right">The right <see cref="ValueInterval{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> value follows or occurs in the same position in the sort order as <paramref name="right"/> interval;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(T? left, ValueInterval<T> right) => right.CompareTo(left) <= 0;
}
