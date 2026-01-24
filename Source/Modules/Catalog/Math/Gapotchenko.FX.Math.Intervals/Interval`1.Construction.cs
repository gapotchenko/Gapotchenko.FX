// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial record Interval<T> : ICloneableInterval
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified inclusive left and exclusive right bounds:
    /// <c>[from,to)</c>.
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
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
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
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
    /// </param>
    /// <exception cref="ArgumentException">If one interval boundary is empty, another should be empty too.</exception>
    public Interval(IntervalBoundary<T> from, IntervalBoundary<T> to, IComparer<T>? comparer = null)
    {
        IntervalEngine.ValidateBoundaryArguments(from, to);

        From = from;
        To = to;

        Comparer = comparer;
    }

    [return: NotNullIfNotNull(nameof(model))]
    internal static Interval<T>? Create(in IntervalModel<T>? model, IComparer<T>? comparer)
    {
        if (model is not { } value)
            return null;

        // Try to pool known intervals first.
        if (IntervalEngine.IsInfinite<IntervalModel<T>, T>(value))
            return Interval.Infinite(comparer);

        comparer ??= Comparer<T>.Default;

        if (IntervalEngine.IsEmpty(value, comparer))
            return Interval.Empty(comparer);

        return new(value, comparer);
    }

    internal Interval(in IntervalModel<T> model, IComparer<T>? comparer)
    {
        From = model.From;
        To = model.To;

        Comparer = comparer;
    }

    IInterval ICloneableInterval.CloneInterval() => this with { };
}
