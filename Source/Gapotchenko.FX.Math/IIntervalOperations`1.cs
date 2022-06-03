using System.ComponentModel;

namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Defines the base operations for <see cref="IInterval{T}"/>.
    /// This interface is not intended to be used directly, use <see cref="IInterval{T}"/> instead.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IIntervalOperations<T>
    {
        /// <summary>
        /// Gets the left boundary of the interval.
        /// Represents a boundary the interval starts with.
        /// </summary>
        IntervalBoundary<T> From { get; }

        /// <summary>
        /// Gets the right boundary of the interval.
        /// Represents a boundary the interval ends with.
        /// </summary>
        IntervalBoundary<T> To { get; }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is bounded.
        /// </para>
        /// <para>
        /// A bounded interval is both left- and right-bounded.
        /// </para>
        /// </summary>
        bool IsBounded
#if TFF_DEFAULT_INTERFACE
            => IntervalEngine.IsBounded<IIntervalOperations<T>, T>(this);
#else
        { get; }
#endif

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is half-bounded.
        /// </para>
        /// <para>
        /// A half-bounded interval is either left- or right-bounded.
        /// </para>
        /// </summary>
        bool IsHalfBounded
#if TFF_DEFAULT_INTERFACE
            => IntervalEngine.IsHalfBounded<IIntervalOperations<T>, T>(this);
#else
        { get; }
#endif

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is open.
        /// </para>
        /// <para>
        /// An open interval does not include its endpoints.
        /// </para>
        /// </summary>
        bool IsOpen
#if TFF_DEFAULT_INTERFACE
            => IntervalEngine.IsOpen<IIntervalOperations<T>, T>(this);
#else
        { get; }
#endif

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is closed.
        /// </para>
        /// <para>
        /// A closed interval includes all its limit points.
        /// </para>
        /// </summary>
        bool IsClosed
#if TFF_DEFAULT_INTERFACE
            => IntervalEngine.IsClosed<IIntervalOperations<T>, T>(this);
#else
        { get; }
#endif

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is half-open.
        /// </para>
        /// <para>
        /// A half-open interval includes only one of its endpoints.
        /// </para>
        /// </summary>
        bool IsHalfOpen
#if TFF_DEFAULT_INTERFACE
            => IntervalEngine.IsHalfOpen<IIntervalOperations<T>, T>(this);
#else
        { get; }
#endif

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
        /// Gets a value indicating whether the interval is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is a degenerate.
        /// </para>
        /// <para>
        /// A degenerate interval <c>[x,x]</c> represents a set of exactly one element <c>{x}</c>.
        /// </para>
        /// </summary>
        bool IsDegenerate { get; }

        /// <summary>
        /// Determines whether the specified element is contained within the interval.
        /// </summary>
        /// <param name="item">The element to check for containment.</param>
        /// <returns><c>true</c> if element is contained within the interval; otherwise, <c>false</c>.</returns>
        bool Contains(T item);

        /// <summary>
        /// Produces the intersection of the current and specified intervals.
        /// </summary>
        /// <param name="other">The interval to produce the intersection with.</param>
        /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
        IInterval<T> Intersect(IInterval<T> other);

        /// <summary>
        /// Determines whether the current and the specified intervals overlap.
        /// </summary>
        /// <param name="other">The interval to check for overlapping.</param>
        /// <returns><see langword="true"/> if the current and <paramref name="other"/> intervals overlap; otherwise, <see langword="false"/>.</returns>
        bool Overlaps(IInterval<T> other);

        /// <summary>
        /// Determines whether the current interval is a subinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        bool IsSubintervalOf(IInterval<T> other);

        /// <summary>
        /// Determines whether the current interval is a superinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        bool IsSuperintervalOf(IInterval<T> other);

        /// <summary>
        /// Determines whether the current interval is a proper subinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a proper subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        bool IsProperSubintervalOf(IInterval<T> other);

        /// <summary>
        /// Determines whether the current interval is a proper superinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a proper superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        bool IsProperSuperintervalOf(IInterval<T> other);

        /// <summary>
        /// Determines whether this and the specified intervals are equal.
        /// </summary>
        /// <param name="other">The interval to check for equality.</param>
        /// <returns><see langword="true"/> if this and <paramref name="other"/> intervals are equal; otherwise, <see langword="false"/>.</returns>
        bool IntervalEquals(IInterval<T> other);
    }
}
