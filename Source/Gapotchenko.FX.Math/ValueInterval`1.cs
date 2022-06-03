﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Represents a continuous value interval.
    /// </summary>
    /// <typeparam name="T">The type of interval value.</typeparam>
    public readonly struct ValueInterval<T> : IInterval<T>, IEquatable<ValueInterval<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// Initializes a new <see cref="ValueInterval{T}"/> instance with the specified bounds.
        /// </summary>
        /// <param name="from">
        /// The left bound of the interval.
        /// Represents a value the interval starts with.
        /// The corresponding limit point is included in the interval.
        /// </param>
        /// <param name="to">
        /// The right bound of the interval.
        /// Represents a value the interval ends with.
        /// The corresponding limit point is not included in the interval.
        /// </param>
        public ValueInterval(T from, T to) :
            this(IntervalBoundary.Inclusive(from), IntervalBoundary.Exclusive(to))
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ValueInterval{T}"/> instance with the specified boundaries.
        /// </summary>
        /// <param name="from">
        /// The left boundary of the interval.
        /// Represents a boundary the interval starts with.
        /// </param>
        /// <param name="to">
        /// The right boundary of the interval.
        /// Represents a boundary the interval ends with.
        /// </param>
        public ValueInterval(IntervalBoundary<T> from, IntervalBoundary<T> to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Returns an empty <see cref="ValueInterval{T}"/>.
        /// </summary>
        public static ValueInterval<T> Empty { get; } = new(IntervalBoundary<T>.Empty, IntervalBoundary<T>.Empty);

        /// <summary>
        /// Returns an infinite <see cref="ValueInterval{T}"/>.
        /// </summary>
        public static ValueInterval<T> Infinite { get; } = new(IntervalBoundary<T>.NegativeInfinity, IntervalBoundary<T>.PositiveInfinity);

        /// <summary>
        /// The left boundary of the interval.
        /// Represents a boundary the interval starts with.
        /// </summary>
        public IntervalBoundary<T> From { get; init; }

        /// <summary>
        /// The right boundary of the interval.
        /// Represents a boundary the interval ends with.
        /// </summary>
        public IntervalBoundary<T> To { get; init; }

        /// <inheritdoc/>
        public bool IsBounded => IntervalEngine.IsBounded<ValueInterval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsHalfBounded => IntervalEngine.IsHalfBounded<ValueInterval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsOpen => IntervalEngine.IsOpen<ValueInterval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsClosed => IntervalEngine.IsClosed<ValueInterval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsHalfOpen => IntervalEngine.IsHalfOpen<ValueInterval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsEmpty => IntervalEngine.IsEmpty<ValueInterval<T>, T>(this, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool IsDegenerate => IntervalEngine.IsDegenerate<ValueInterval<T>, T>(this, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool Contains(T item) => IntervalEngine.Contains(this, item, Comparer<T>.Default);

        static ValueInterval<T> Construct(IntervalBoundary<T> from, IntervalBoundary<T> to) => new(from, to);

        /// <summary>
        /// <para>
        /// Gets the interval interior.
        /// </para>
        /// <para>
        /// The interior of an interval <c>I</c> is the largest open interval that is contained in <c>I</c>;
        /// it is also the set of points in <c>I</c> which are not the endpoints of <c>I</c>.
        /// </para>
        /// </summary>
        public ValueInterval<T> Interior => IntervalEngine.Interior<ValueInterval<T>, T>(this, Construct);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IInterval<T> IIntervalOperations<T>.Interior => Interior;

        /// <summary>
        /// <para>
        /// Gets the interval enclosure.
        /// </para>
        /// <para>
        /// The enclosure of an interval <c>I</c> is the smallest closed interval that contains <c>I</c>;
        /// which is also the set <c>I</c> augmented with its finite endpoints.
        /// </para>
        /// </summary>
        public ValueInterval<T> Enclosure => IntervalEngine.Enclosure<ValueInterval<T>, T>(this, Construct);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IInterval<T> IIntervalOperations<T>.Enclosure => Enclosure;

        /// <summary>
        /// Produces the intersection of the current and specified intervals.
        /// </summary>
        /// <param name="other">The interval to produce the intersection with.</param>
        /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
        /// <typeparam name="TOther">Type of the other interval to produce the intersection with.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValueInterval<T> Intersect<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.Intersect(
                this,
                other ?? throw new ArgumentNullException(nameof(other)),
                Comparer<T>.Default,
                Construct);

        IInterval<T> IIntervalOperations<T>.Intersect(IInterval<T> limits) => Intersect<IIntervalOperations<T>>(limits);

        /// <inheritdoc/>
        public bool Overlaps(IInterval<T> other) => Overlaps<IInterval<T>>(other);

        /// <summary>
        /// Determines whether this and the specified intervals overlap.
        /// </summary>
        /// <param name="other">The interval to check for overlapping.</param>
        /// <returns><see langword="true"/> if this interval and <paramref name="other"/> overlap; otherwise, <see langword="false"/>.</returns>
        /// <typeparam name="TOther">Type of the interval to check for overlapping.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Overlaps<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.Overlaps<ValueInterval<T>, TOther, T>(this, other, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool IsSubintervalOf(IInterval<T> other) => IsSubintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a subinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSubintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.IsSubintervalOf<ValueInterval<T>, TOther, T>(this, other, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool IsSuperintervalOf(IInterval<T> other) => IsSuperintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a superinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSuperintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.IsSuperintervalOf<ValueInterval<T>, TOther, T>(this, other, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool IsProperSubintervalOf(IInterval<T> other) => IsProperSubintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a proper subinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a proper subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsProperSubintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.IsProperSubintervalOf<ValueInterval<T>, TOther, T>(this, other, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool IsProperSuperintervalOf(IInterval<T> other) => IsProperSuperintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a proper superinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a proper superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsProperSuperintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.IsProperSuperintervalOf<ValueInterval<T>, TOther, T>(this, other, Comparer<T>.Default);

        /// <inheritdoc/>
        public bool IntervalEquals(IInterval<T> other) => IntervalEquals<IIntervalOperations<T>>(other);

        /// <summary>
        /// Determines whether this and the specified intervals are equal.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if this interval and <paramref name="other"/> are equal; otherwise, <see langword="false"/>.</returns>
        /// <typeparam name="TOther">Type of the interval to compare.</typeparam>
        public bool IntervalEquals<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.IntervalsEqual<ValueInterval<T>, TOther, T>(this, other, Comparer<T>.Default);

        /// <summary>
        /// Determines whether the specified intervals are equal.
        /// </summary>
        /// <param name="x">The first interval.</param>
        /// <param name="y">The second interval.</param>
        /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(ValueInterval<T> x, IInterval<T>? y) => EqualityOperator(x, y);

        /// <summary>
        /// Determines whether the specified intervals are not equal.
        /// </summary>
        /// <param name="x">The first interval.</param>
        /// <param name="y">The second interval.</param>
        /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ValueInterval<T> x, IInterval<T>? y) => !EqualityOperator(x, y);

        /// <summary>
        /// Determines whether the specified intervals are equal.
        /// </summary>
        /// <param name="x">The first interval.</param>
        /// <param name="y">The second interval.</param>
        /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator ==(ValueInterval<T> x, Interval<T>? y) => EqualityOperator(x, y);

        /// <summary>
        /// Determines whether the specified intervals are not equal.
        /// </summary>
        /// <param name="x">The first interval.</param>
        /// <param name="y">The second interval.</param>
        /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool operator !=(ValueInterval<T> x, Interval<T>? y) => !EqualityOperator(x, y);

        static bool EqualityOperator<TOther>(ValueInterval<T> x, TOther? y)
            where TOther : IIntervalOperations<T> =>
            y is not null &&
            x.IntervalEquals(y);

#if false && NET7_0_OR_GREATER
        static bool IInterval<T>.operator ==(IInterval<T>? x, IInterval<T>? y) => EqualityOperator(x, y);

        static bool IInterval<T>.operator !=(IInterval<T>? x, IInterval<T>? y) => !EqualityOperator(x, y);

        static bool EqualityOperator(IInterval<T>? x, IInterval<T>? y) =>
            ReferenceEquals(x, y) ||
            x is not null &&
            y is not null &&
            x.IntervalEquals(y);
#endif

        /// <inheritdoc/>
        public bool Equals(ValueInterval<T> other) => IntervalEngine.IntervalsEqual(this, other, EqualityComparer<T>.Default);

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is ValueInterval<T> other &&
            Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hc = new HashCode();

            hc.Add(From.Kind.GetHashCode());
            if (From.HasValue)
                hc.Add(From.Value);

            hc.Add(To.Kind.GetHashCode());
            if (To.HasValue)
                hc.Add(To.Value);

            return hc.ToHashCode();
        }

        /// <inheritdoc/>
        public override string ToString() => IntervalEngine.ToString<ValueInterval<T>, T>(this);
    }
}