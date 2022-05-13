using System;
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
            this(IntervalBoundary.Inclusive, from, to, IntervalBoundary.Exclusive, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified bounds and their types.
        /// </summary>
        /// <param name="fromBoundary">
        /// The left boundary.
        /// Represents a type of boundary the interval starts with.
        /// </param>
        /// <param name="from">
        /// The left bound of the interval.
        /// Represents a value the interval starts with.
        /// </param>
        /// <param name="to">
        /// The right bound of the interval.
        /// Represents a value the interval ends with.
        /// </param>
        /// <param name="toBoundary">
        /// The right boundary.
        /// Represents a type of boundary the interval ends with.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
        /// or <c>null</c> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
        /// </param>
        public Interval(
            IntervalBoundary fromBoundary,
            T from,
            T to,
            IntervalBoundary toBoundary,
            IComparer<T>? comparer = null)
        {
            From = from;
            To = to;

            FromBoundary = fromBoundary;
            ToBoundary = toBoundary;

            Comparer = comparer;
        }

        /// <summary>
        /// The left bound of the interval.
        /// </summary>
        public T From { get; init; }

        /// <summary>
        /// The right bound of the interval.
        /// </summary>
        public T To { get; init; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IntervalFlags m_Flags;

        /// <summary>
        /// The left boundary.
        /// Represents a type of boundary the interval starts with.
        /// </summary>
        public IntervalBoundary FromBoundary
        {
            get => IntervalHelpers.GetBoundary(m_Flags, IntervalFlags.LeftBounded, IntervalFlags.LeftClosed);
            init => m_Flags = IntervalHelpers.SetBoundary(m_Flags, IntervalFlags.LeftBounded, IntervalFlags.LeftClosed, value);
        }

        /// <summary>
        /// The right boundary.
        /// Represents a type of boundary the interval ends with.
        /// </summary>
        public IntervalBoundary ToBoundary
        {
            get => IntervalHelpers.GetBoundary(m_Flags, IntervalFlags.RightBounded, IntervalFlags.RightClosed);
            init => m_Flags = IntervalHelpers.SetBoundary(m_Flags, IntervalFlags.RightBounded, IntervalFlags.RightClosed, value);
        }

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
        public bool IsBounded => IntervalHelpers.IsBounded<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsHalfBounded => IntervalHelpers.IsHalfBounded<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsOpen => IntervalHelpers.IsOpen<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsClosed => IntervalHelpers.IsClosed<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsHalfOpen => IntervalHelpers.IsHalfOpen<Interval<T>, T>(this);

        /// <inheritdoc/>
        public bool IsEmpty => IntervalHelpers.IsEmpty<Interval<T>, T>(this, m_Comparer);

        /// <inheritdoc/>
        public bool IsDegenerate => IntervalHelpers.IsDegenerate<Interval<T>, T>(this, m_Comparer);

        /// <inheritdoc/>
        public bool Contains(T item) => IntervalHelpers.Contains(this, item, m_Comparer);

        Interval<T> Construct(
            IntervalBoundary fromBoundary, T from,
            T to, IntervalBoundary toBoundary) =>
            new(
                fromBoundary, from,
                to, toBoundary,
                m_Comparer);

        /// <summary>
        /// <para>
        /// Gets the interval interior.
        /// </para>
        /// <para>
        /// The interior of an interval <c>I</c> is the largest open interval that is contained in <c>I</c>;
        /// it is also the set of points in <c>I</c> which are not the endpoints of <c>I</c>.
        /// </para>
        /// </summary>
        public Interval<T> Interior => IntervalHelpers.Interior<Interval<T>, T>(this, Construct);

        /// <inheritdoc/>
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
        public Interval<T> Enclosure => IntervalHelpers.Enclosure<Interval<T>, T>(this, Construct);

        /// <inheritdoc/>
        IInterval<T> IInterval<T>.Enclosure => Enclosure;

        /// <summary>
        /// Returns a copy of this interval clamped to the specified limits.
        /// </summary>
        /// <param name="limits">The limiting interval.</param>
        /// <returns>A new interval clamped to the bounds of <paramref name="limits"/>.</returns>
        public Interval<T> Clamp(IInterval<T> limits) => Clamp<IInterval<T>>(limits);

        /// <summary>
        /// Returns a copy of this interval clamped to the specified limits.
        /// </summary>
        /// <param name="limits">The limiting interval.</param>
        /// <returns>A new interval clamped to the bounds of <paramref name="limits"/>.</returns>
        /// <typeparam name="TLimits">Type of the limiting interval.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Interval<T> Clamp<TLimits>(TLimits limits) where TLimits : IInterval<T> =>
            IntervalHelpers.Clamp(
                this,
                limits ?? throw new ArgumentNullException(nameof(limits)),
                m_Comparer,
                Construct);

        /// <inheritdoc/>
        IInterval<T> IInterval<T>.Clamp(IInterval<T> limits) => Clamp<IInterval<T>>(limits);

        /// <inheritdoc/>
        public bool Overlaps(IInterval<T> other) => Overlaps<IInterval<T>>(other);

        /// <summary>
        /// Determines whether this and the specified intervals overlap (i.e., contain at least one element in common).
        /// </summary>
        /// <param name="other">The interval to check for overlapping.</param>
        /// <returns><c>true</c> if this interval and <paramref name="other"/> overlap; otherwise, false.</returns>
        /// <typeparam name="TOther">Type of the interval to check for overlapping.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Overlaps<TOther>(TOther other) where TOther : IInterval<T> =>
            IntervalHelpers.Overlaps(
                this,
                other ?? throw new ArgumentNullException(nameof(other)),
                m_Comparer);

        // Minify unused record method.
        bool PrintMembers(StringBuilder _) => false;

        /// <inheritdoc/>
        public override string ToString() => IntervalHelpers.ToString<Interval<T>, T>(this);
    }
}
