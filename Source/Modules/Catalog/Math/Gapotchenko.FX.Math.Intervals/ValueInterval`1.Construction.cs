// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial struct ValueInterval<T>
{
    /// <summary>
    /// Initializes a new <see cref="ValueInterval{T}"/> instance with the specified inclusive left and exclusive right bounds:
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
    /// <exception cref="ArgumentException">If one interval boundary is empty, another should be empty too.</exception>
    public ValueInterval(IntervalBoundary<T> from, IntervalBoundary<T> to)
    {
        IntervalEngine.ValidateBoundaryArguments(from, to);

        From = from;
        To = to;
    }

    [return: NotNullIfNotNull(nameof(model))]
    static ValueInterval<T>? Create(in IntervalModel<T>? model) => model is null ? null : new(model.Value);

    internal ValueInterval(in IntervalModel<T> model)
    {
        From = model.From;
        To = model.To;
    }
}
