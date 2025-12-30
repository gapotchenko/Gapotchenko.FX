// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Defines the interval operations for <see cref="IInterval{T}"/>.
/// This interface is not intended to be used directly, use <see cref="IInterval{T}"/> instead.
/// </summary>
/// <typeparam name="T">The type of interval values.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IIntervalOperations<T> : IIntervalOperations, IIntervalModel<T>
{
    /// <summary>
    /// Gets the <see cref="IComparer{T}"/> object that is used to compare the values in the interval.
    /// </summary>
    IComparer<T> Comparer { get; }

    /// <summary>
    /// <para>
    /// Gets the interval interior.
    /// </para>
    /// <para>
    /// The interior of an interval <c>I</c> is the largest open interval that is contained in <c>I</c>;
    /// it is also the set of points in <c>I</c> which are not the endpoints of <c>I</c>.
    /// </para>
    /// </summary>
    IInterval<T> Interior { get; }

    /// <summary>
    /// <para>
    /// Gets the interval enclosure.
    /// </para>
    /// <para>
    /// The enclosure of an interval <c>I</c> is the smallest closed interval that contains <c>I</c>;
    /// which is also the set <c>I</c> augmented with its finite endpoints.
    /// </para>
    /// </summary>
    IInterval<T> Enclosure { get; }

    /// <summary>
    /// Determines whether the specified value is contained within the interval.
    /// </summary>
    /// <param name="value">The value to check for containment.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> is contained within the interval;
    /// otherwise, <see langword="false"/>.</returns>
    bool Contains(T value);

    /// <summary>
    /// Returns an integer that indicates a zone of the specified value in relation to the interval range.
    /// </summary>
    /// <param name="value">The value to get a zone for.</param>
    /// <returns>
    /// <para>
    /// An integer number that indicates a zone of the <paramref name="value"/>, as shown in the following table.
    /// </para>
    /// <para>
    /// <c>0</c> if the interval contains the <paramref name="value"/> or is empty.
    /// <br/>-or-<br/>
    /// <c>1</c> if the <paramref name="value"/> is greater than the right boundary of the interval.
    /// <br/>-or-<br/>
    /// <c>-1</c> if the <paramref name="value"/> is less than the left boundary of the interval.
    /// </para>
    /// </returns>
    int Zone(T value);

    /// <summary>
    /// Produces the intersection of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the intersection with.</param>
    /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    IInterval<T> Intersect(IInterval<T> other);

    /// <summary>
    /// Produces the union of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the union with.</param>
    /// <returns>A new interval representing a union of the current and specified intervals.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    IInterval<T> Union(IInterval<T> other);

    /// <summary>
    /// Determines whether this and the specified intervals overlap.
    /// </summary>
    /// <param name="other">The interval to check for overlapping.</param>
    /// <returns><see langword="true"/> if this and <paramref name="other"/> intervals overlap; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    bool Overlaps(IInterval<T> other);

    /// <summary>
    /// Determines whether the current interval is a subinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a subinterval of the <paramref name="other"/> interval; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    bool IsSubintervalOf(IInterval<T> other);

    /// <summary>
    /// Determines whether the current interval is a superinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a superinterval of the <paramref name="other"/> interval; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    bool IsSuperintervalOf(IInterval<T> other);

    /// <summary>
    /// Determines whether the current interval is a proper subinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a proper subinterval of the <paramref name="other"/> interval; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    bool IsProperSubintervalOf(IInterval<T> other);

    /// <summary>
    /// Determines whether the current interval is a proper superinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a proper superinterval of the <paramref name="other"/> interval; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    bool IsProperSuperintervalOf(IInterval<T> other);

    /// <inheritdoc cref="IIntervalOperations.IntervalEquals(IInterval?)"/>
    bool IntervalEquals([NotNullWhen(true)] IInterval<T>? other);
}
