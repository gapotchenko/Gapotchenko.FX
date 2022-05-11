﻿namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Provides the base interface for the abstraction of continuous intervals of values.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
    public interface IInterval<T>
    {
        /// <summary>
        /// Gets the left bound of the interval.
        /// </summary>
        public T LeftBound { get; }

        /// <summary>
        /// Gets the right bound of the interval.
        /// </summary>
        public T RightBound { get; }

        /// <summary>
        /// Gets a value indicating whether the left bound limit point is included in the interval.
        /// </summary>
        public bool IsLeftClosed { get; }

        /// <summary>
        /// Gets a value indicating whether the right bound limit point is included in the interval.
        /// </summary>
        public bool IsRightClosed { get; }

        /// <summary>
        /// Gets a value indicating whether the interval is left-bounded.
        /// </summary>
        public bool IsLeftBounded { get; }

        /// <summary>
        /// Gets a value indicating whether the interval is right-bounded.
        /// </summary>
        public bool IsRightBounded { get; }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is bounded.
        /// </para>
        /// <para>
        /// A bounded interval is both left- and right-bounded.
        /// </para>
        /// </summary>
        public bool IsBounded
#if TFF_DEFAULT_INTERFACE
            => IntervalHelpers.IsBounded<IInterval<T>, T>(this);
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
        public bool IsHalfBounded
#if TFF_DEFAULT_INTERFACE
            => IntervalHelpers.IsHalfBounded<IInterval<T>, T>(this);
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
        public bool IsOpen
#if TFF_DEFAULT_INTERFACE
            => IntervalHelpers.IsOpen<IInterval<T>, T>(this);
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
        public bool IsClosed
#if TFF_DEFAULT_INTERFACE
            => IntervalHelpers.IsClosed<IInterval<T>, T>(this);
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
        public bool IsHalfOpen
#if TFF_DEFAULT_INTERFACE
            => IntervalHelpers.IsHalfOpen<IInterval<T>, T>(this);
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
        public IInterval<T> Interior { get; }

        /// <summary>
        /// <para>
        /// Gets the interval enclosure.
        /// </para>
        /// <para>
        /// The enclosure of an interval <c>I</c> is the smallest closed interval that contains <c>I</c>;
        /// which is also the set <c>I</c> augmented with its finite endpoints.
        /// </para>
        /// </summary>
        public IInterval<T> Enclosure { get; }

        /// <summary>
        /// Gets a value indicating whether the interval is empty.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the interval is a degenerate.
        /// </para>
        /// <para>
        /// A degenerate interval <c>[x,x]</c> represents a set of exactly one element <c>{x}</c>.
        /// </para>
        /// </summary>
        public bool IsDegenerate { get; }

        /// <summary>
        /// Determines whether the specified element is contained within the interval.
        /// </summary>
        /// <param name="item">The element to check for containment.</param>
        /// <returns><c>true</c> if element is contained within the interval; otherwise, <c>false</c>.</returns>
        public bool Contains(T item);

        /// <summary>
        /// Returns a copy of this interval clamped to the specified limits.
        /// </summary>
        /// <param name="limits">The limiting interval.</param>
        /// <returns>A new interval clamped to the bounds of <paramref name="limits"/>.</returns>
        public IInterval<T> Clamp(IInterval<T> limits);

        /// <summary>
        /// Determines whether this and the specified intervals overlap (i.e., contain at least one element in common).
        /// </summary>
        /// <param name="other">The interval to check for overlapping.</param>
        /// <returns><c>true</c> if this interval and <paramref name="other"/> overlap; otherwise, false.</returns>
        public bool Overlaps(IInterval<T> other);
    }
}