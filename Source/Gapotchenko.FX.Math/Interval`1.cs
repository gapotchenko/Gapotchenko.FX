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
        /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified lower and upper bounds.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the interval.</param>
        /// <param name="upperBound">The upper bound of the interval.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
        /// or <c>null</c> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
        /// </param>
        public Interval(T lowerBound, T upperBound, IComparer<T>? comparer = null) :
            this(lowerBound, upperBound, true, false, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified lower and upper bounds and their limit point inclusions.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the interval.</param>
        /// <param name="upperBound">The upper bound of the interval.</param>
        /// <param name="inclusiveLowerBound">Indicates whether the lower bound limit point is included in the interval.</param>
        /// <param name="inclusiveUpperBound">Indicates whether the upper bound limit point is included in the interval.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
        /// or <c>null</c> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
        /// </param>
        public Interval(
            T lowerBound, T upperBound,
            bool inclusiveLowerBound, bool inclusiveUpperBound,
            IComparer<T>? comparer = null) :
            this(
                lowerBound, upperBound,
                inclusiveLowerBound, inclusiveUpperBound,
                lowerBound is not null, upperBound is not null,
                comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified lower and upper bounds, their limit point inclusions and boundedness.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the interval.</param>
        /// <param name="upperBound">The upper bound of the interval.</param>
        /// <param name="inclusiveLowerBound">Indicates whether the lower bound limit point is included in the interval.</param>
        /// <param name="inclusiveUpperBound">Indicates whether the upper bound limit point is included in the interval.</param>
        /// <param name="hasLowerBound">Indicates whether the interval is lower-bounded.</param>
        /// <param name="hasUpperBound">Indicates whether the interval is upper-bounded.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
        /// or <c>null</c> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
        /// </param>
        public Interval(
            T lowerBound, T upperBound,
            bool inclusiveLowerBound, bool inclusiveUpperBound,
            bool hasLowerBound, bool hasUpperBound,
            IComparer<T>? comparer = null)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;

            var flags = IntervalFlags.None;
            if (inclusiveLowerBound)
                flags |= IntervalFlags.InclusiveLowerBound;
            if (inclusiveUpperBound)
                flags |= IntervalFlags.InclusiveUpperBound;
            if (hasLowerBound)
                flags |= IntervalFlags.HasLowerBound;
            if (hasUpperBound)
                flags |= IntervalFlags.HasUpperBound;

            m_Flags = flags;
            Comparer = comparer;
        }

        /// <summary>
        /// The lower bound.
        /// </summary>
        public T LowerBound { get; init; }

        /// <summary>
        /// The upper bound.
        /// </summary>
        public T UpperBound { get; init; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IntervalFlags m_Flags;

        /// <summary>
        /// Indicates whether the lower bound limit point is included in the interval.
        /// </summary>
        public bool InclusiveLowerBound
        {
            get => (m_Flags & IntervalFlags.InclusiveLowerBound) != 0;
            init => m_Flags = IntervalHelpers.SetFlag(m_Flags, IntervalFlags.InclusiveLowerBound, value);
        }

        /// <summary>
        /// Indicates whether the upper bound limit point is included in the interval.
        /// </summary>
        public bool InclusiveUpperBound
        {
            get => (m_Flags & IntervalFlags.InclusiveUpperBound) != 0;
            init => m_Flags = IntervalHelpers.SetFlag(m_Flags, IntervalFlags.InclusiveUpperBound, value);
        }

        /// <summary>
        /// Indicates whether the interval is lower-bounded.
        /// </summary>
        public bool HasLowerBound
        {
            get => (m_Flags & IntervalFlags.HasLowerBound) != 0;
            init => m_Flags = IntervalHelpers.SetFlag(m_Flags, IntervalFlags.HasLowerBound, value);
        }

        /// <summary>
        /// Indicates whether the interval is upper-bounded.
        /// </summary>
        public bool HasUpperBound
        {
            get => (m_Flags & IntervalFlags.HasUpperBound) != 0;
            init => m_Flags = IntervalHelpers.SetFlag(m_Flags, IntervalFlags.HasUpperBound, value);
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

        Interval<T> Construct(
            T lowerBound, T upperBound,
            bool inclusiveLowerBound, bool inclusiveUpperBound,
            bool hasLowerBound, bool hasUpperBound) =>
            new Interval<T>(
                LowerBound, UpperBound,
                inclusiveLowerBound, inclusiveUpperBound,
                HasLowerBound, HasUpperBound,
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

        /// <inheritdoc/>
        public bool IsEmpty => IntervalHelpers.IsEmpty<Interval<T>, T>(this, m_Comparer);

        /// <inheritdoc/>
        public bool IsSingleton => IntervalHelpers.IsSingleton<Interval<T>, T>(this, m_Comparer);

        /// <inheritdoc/>
        public bool Contains(T item) => IntervalHelpers.Contains(this, item, m_Comparer);

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
