﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Represents a continuous interval of values.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
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
        IInterval<T> IInterval<T>.Interior => Interior;

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
        IInterval<T> IInterval<T>.Enclosure => Enclosure;

        /// <summary>
        /// Returns a copy of this interval clamped to the specified limits.
        /// </summary>
        /// <param name="limits">The limiting interval.</param>
        /// <returns>A new interval clamped to the bounds of <paramref name="limits"/>.</returns>
        public Interval<T> Clamp(IInterval<T> limits) => Intersect<IInterval<T>>(limits);

        /// <summary>
        /// Produces the intersection of the current and specified intervals.
        /// </summary>
        /// <param name="other">The interval to produce the intersection with.</param>
        /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
        /// <typeparam name="TOther">Type of the other interval to produce the intersection with.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Interval<T> Intersect<TOther>(TOther other) where TOther : IInterval<T> =>
            IntervalEngine.Intersect(
                this,
                other ?? throw new ArgumentNullException(nameof(other)),
                m_Comparer,
                Construct);

        /// <inheritdoc/>
        IInterval<T> IInterval<T>.Intersect(IInterval<T> limits) => Intersect<IInterval<T>>(limits);

        /// <inheritdoc/>
        public bool Overlaps(IInterval<T> other) => Overlaps<IInterval<T>>(other);

        /// <summary>
        /// Determines whether this and the specified intervals overlap.
        /// </summary>
        /// <param name="other">The interval to check for overlapping.</param>
        /// <returns><see langword="true"/> if this interval and <paramref name="other"/> overlap; otherwise, <see langword="false"/>.</returns>
        /// <typeparam name="TOther">Type of the interval to check for overlapping.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Overlaps<TOther>(TOther other) where TOther : IInterval<T> =>
            ReferenceEquals(other, this) ||
            IntervalEngine.Overlaps<Interval<T>, TOther, T>(
                this,
                other ?? throw new ArgumentNullException(nameof(other)),
                m_Comparer);

        // Minify unused record method.
        bool PrintMembers(StringBuilder _) => false;

        /// <inheritdoc/>
        public override string ToString() => IntervalEngine.ToString<Interval<T>, T>(this);
    }
}
