// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial record Interval<T>
{
    /// <inheritdoc/>
    public int CompareTo(T? value) => IntervalEngine.CompareTo(this, value, m_Comparer);

    /// <summary>
    /// Determines whether the left specified <see cref="Interval{T}"/> object precedes
    /// the right specified <typeparamref name="T"/> object in the sort order.
    /// </summary>
    /// <param name="left">The left <see cref="Interval{T}"/> object.</param>
    /// <param name="right">The right <typeparamref name="T"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object precedes <paramref name="right"/> in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(Interval<T>? left, T? right) => (left ?? Empty).CompareTo(right) < 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> object precedes
    /// the right specified <see cref="Interval{T}"/> object in the sort order.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> object.</param>
    /// <param name="right">The right <see cref="Interval{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object precedes <paramref name="right"/> in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(T? left, Interval<T>? right) => (right ?? Empty).CompareTo(left) > 0;

    /// <summary>
    /// Determines whether the left specified <see cref="Interval{T}"/> object follows
    /// the right specified <typeparamref name="T"/> object in the sort order.
    /// </summary>
    /// <param name="left">The left <see cref="Interval{T}"/> object.</param>
    /// <param name="right">The right <typeparamref name="T"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object follows <paramref name="right"/> in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(Interval<T>? left, T? right) => (left ?? Empty).CompareTo(right) > 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> object follows
    /// the right specified <see cref="Interval{T}"/> object in the sort order.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> object.</param>
    /// <param name="right">The right <see cref="Interval{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object follows <paramref name="right"/> in the sort order;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(T? left, Interval<T>? right) => (right ?? Empty).CompareTo(left) < 0;

    /// <summary>
    /// Determines whether the left specified <see cref="Interval{T}"/> object precedes
    /// or occurs in the same position in the sort order as
    /// the right specified <typeparamref name="T"/> object.
    /// </summary>
    /// <param name="left">The left <see cref="Interval{T}"/> object.</param>
    /// <param name="right">The right <typeparamref name="T"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object precedes or occurs in the same position in the sort order as <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(Interval<T>? left, T? right) => (left ?? Empty).CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> object precedes
    /// or occurs in the same position in the sort order as
    /// the right specified <see cref="Interval{T}"/> object.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> object.</param>
    /// <param name="right">The right <see cref="Interval{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object precedes or occurs in the same position in the sort order as <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(T? left, Interval<T>? right) => (right ?? Empty).CompareTo(left) >= 0;

    /// <summary>
    /// Determines whether the left specified <see cref="Interval{T}"/> object follows
    /// or occurs in the same position in the sort order as
    /// the right specified <typeparamref name="T"/> object.
    /// </summary>
    /// <param name="left">The left <see cref="Interval{T}"/> object.</param>
    /// <param name="right">The right <typeparamref name="T"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object follows or occurs in the same position in the sort order as <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(Interval<T>? left, T? right) => (left ?? Empty).CompareTo(right) >= 0;

    /// <summary>
    /// Determines whether the left specified <typeparamref name="T"/> object follows
    /// or occurs in the same position in the sort order as
    /// the right specified <see cref="Interval{T}"/> object.
    /// </summary>
    /// <param name="left">The left <typeparamref name="T"/> object.</param>
    /// <param name="right">The right <see cref="Interval{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> object follows or occurs in the same position in the sort order as <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(T? left, Interval<T>? right) => (right ?? Empty).CompareTo(left) <= 0;
}
