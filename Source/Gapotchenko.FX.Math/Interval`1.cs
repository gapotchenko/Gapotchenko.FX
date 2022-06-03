﻿using Gapotchenko.FX.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Represents a continuous interval.
    /// </summary>
    /// <typeparam name="T">The type of interval value.</typeparam>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed record Interval<T> : IInterval<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified bounds.
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
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
        /// or <c>null</c> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
        /// </param>
        public Interval(T from, T to, IComparer<T>? comparer = null) :
            this(IntervalBoundary.Inclusive(from), IntervalBoundary.Exclusive(to), comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified boundaries.
        /// </summary>
        /// <param name="from">
        /// The left boundary of the interval.
        /// Represents a boundary the interval starts with.
        /// </param>
        /// <param name="to">
        /// The right boundary of the interval.
        /// Represents a boundary the interval ends with.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
        /// or <c>null</c> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
        /// </param>
        public Interval(IntervalBoundary<T> from, IntervalBoundary<T> to, IComparer<T>? comparer = null)
        {
            From = from;
            To = to;

            Comparer = comparer;
        }

        /// <summary>
        /// Returns an empty <see cref="Interval{T}"/>.
        /// </summary>
        public static Interval<T> Empty { get; } = new(IntervalBoundary<T>.Empty, IntervalBoundary<T>.Empty);

        /// <summary>
        /// Returns an infinite <see cref="Interval{T}"/>.
        /// </summary>
        public static Interval<T> Infinite { get; } = new(IntervalBoundary<T>.NegativeInfinity, IntervalBoundary<T>.PositiveInfinity);

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IComparer<T> m_Comparer;

        /// <summary>
        /// The <see cref="IComparer{T}"/> object that is used to compare the values in the interval.
        /// </summary>
        [AllowNull]
        public IComparer<T> Comparer
        {
            get => m_Comparer;

            [MemberNotNull(nameof(m_Comparer))]
            init => m_Comparer = value ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public bool IsBounded => IntervalEngine.IsBounded<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsHalfBounded => IntervalEngine.IsHalfBounded<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsOpen => IntervalEngine.IsOpen<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsClosed => IntervalEngine.IsClosed<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsHalfOpen => IntervalEngine.IsHalfOpen<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsEmpty => IntervalEngine.IsEmpty<Interval<T>, T>(this, m_Comparer);

        /// <inheritdoc/>
        public bool IsDegenerate => IntervalEngine.IsDegenerate<Interval<T>, T>(this, m_Comparer);

        /// <inheritdoc/>
        public bool Contains(T item) => IntervalEngine.Contains(this, item, m_Comparer);

        Interval<T> Construct(IntervalBoundary<T> from, IntervalBoundary<T> to) => new(from, to, m_Comparer);

        /// <summary>
        /// <para>
        /// Gets the interval interior.
        /// </para>
        /// <para>
        /// The interior of an interval <c>I</c> is the largest open interval that is contained in <c>I</c>;
        /// it is also the set of points in <c>I</c> which are not the endpoints of <c>I</c>.
        /// </para>
        /// </summary>
        public Interval<T> Interior => IntervalEngine.Interior<Interval<T>, T>(this, Construct);

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
        public Interval<T> Enclosure => IntervalEngine.Enclosure<Interval<T>, T>(this, Construct);

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
        public Interval<T> Intersect<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IntervalEngine.Intersect(
                this,
                other ?? throw new ArgumentNullException(nameof(other)),
                m_Comparer,
                Construct);

        IInterval<T> IIntervalOperations<T>.Intersect(IInterval<T> limits) => Intersect<IIntervalOperations<T>>(limits);

        bool IsThis<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            !TypeTraits<TOther>.IsValueType &&
            ReferenceEquals(other, this);

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
            IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
            IntervalEngine.Overlaps<Interval<T>, TOther, T>(this, other, m_Comparer);

        /// <inheritdoc/>
        public bool IsSubintervalOf(IInterval<T> other) => IsSubintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a subinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSubintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
            IntervalEngine.IsSubintervalOf<Interval<T>, TOther, T>(this, other, m_Comparer);

        /// <inheritdoc/>
        public bool IsSuperintervalOf(IInterval<T> other) => IsSuperintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a superinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSuperintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
            IntervalEngine.IsSuperintervalOf<Interval<T>, TOther, T>(this, other, m_Comparer);

        /// <inheritdoc/>
        public bool IsProperSubintervalOf(IInterval<T> other) => IsProperSubintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a proper subinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a proper subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsProperSubintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            !IsThis(other ?? throw new ArgumentNullException(nameof(other))) &&
            IntervalEngine.IsProperSubintervalOf<Interval<T>, TOther, T>(this, other, m_Comparer);

        /// <inheritdoc/>
        public bool IsProperSuperintervalOf(IInterval<T> other) => IsProperSuperintervalOf<IInterval<T>>(other);

        /// <summary>
        /// Determines whether the current interval is a proper superinterval of the specified interval.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if the current interval is a proper superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsProperSuperintervalOf<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            !IsThis(other ?? throw new ArgumentNullException(nameof(other))) &&
            IntervalEngine.IsProperSuperintervalOf<Interval<T>, TOther, T>(this, other, m_Comparer);

        /// <inheritdoc/>
        public bool IntervalEquals(IInterval<T> other) => IntervalEquals<IIntervalOperations<T>>(other);

        /// <summary>
        /// Determines whether this and the specified intervals are equal.
        /// </summary>
        /// <param name="other">The interval to compare to the current interval.</param>
        /// <returns><see langword="true"/> if this interval and <paramref name="other"/> are equal; otherwise, <see langword="false"/>.</returns>
        /// <typeparam name="TOther">Type of the interval to compare.</typeparam>
        public bool IntervalEquals<TOther>(TOther other) where TOther : IIntervalOperations<T> =>
            IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
            IntervalEngine.IntervalsEqual<Interval<T>, TOther, T>(this, other, m_Comparer);

        /// <summary>
        /// Determines whether the specified intervals are equal.
        /// </summary>
        /// <param name="x">The first interval.</param>
        /// <param name="y">The second interval.</param>
        /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Interval<T>? x, IInterval<T>? y) => EqualityOperator(x, y);

        /// <summary>
        /// Determines whether the specified intervals are not equal.
        /// </summary>
        /// <param name="x">The first interval.</param>
        /// <param name="y">The second interval.</param>
        /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Interval<T>? x, IInterval<T>? y) => !EqualityOperator(x, y);

        static bool EqualityOperator(Interval<T>? x, IInterval<T>? y) =>
            ReferenceEquals(x, y) ||
            x is not null &&
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

        // Minify unused record method.
        bool PrintMembers(StringBuilder _) => false;

        /// <inheritdoc/>
        public override string ToString() => IntervalEngine.ToString<Interval<T>, T>(this);
    }
}
