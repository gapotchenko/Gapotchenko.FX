using System;
using System.Diagnostics;

namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Represents an interval boundary.
    /// </summary>
    public readonly struct IntervalBoundary<T>
    {
        internal IntervalBoundary(IntervalBoundaryKind kind, T value)
        {
            Kind = kind;
            m_Value = value;
        }

        /// <summary>
        /// Gets interval boundary kind.
        /// </summary>
        public IntervalBoundaryKind Kind { get; }

        /// <summary>
        /// Gets a value indicating whether the boundary represents either negative or positive infinity.
        /// </summary>
        public bool IsInfinity => Kind is IntervalBoundaryKind.NegativeInfinity or IntervalBoundaryKind.PositiveInfinity;

        /// <summary>
        /// Gets a value indicating whether the current interval boundary has a value.
        /// </summary>
        public bool HasValue => !IsInfinity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly T m_Value;

        /// <summary>
        /// Gets interval boundary value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Interval boundary has no bound value.");
                return m_Value;
            }
        }

        /// <summary>
        /// Gets interval boundary value, or the default value of the underlying type <typeparamref name="T"/> when the boundary is infinite.
        /// </summary>
        /// <returns>The interval boundary value, or the default value of the underlying type <typeparamref name="T"/> when the boundary is infinite.</returns>
        public T GetValueOrDefault() => GetValueOrDefault(default!);

        /// <summary>
        /// Gets interval boundary value, or the specified default value when the boundary is infinite.
        /// </summary>
        /// <param name="defaultValue">A value to return if the boundary is infinite.</param>
        /// <returns>The interval boundary value, or the specified default value when the boundary is infinite.</returns>
        public T GetValueOrDefault(T defaultValue) => IsInfinity ? defaultValue : m_Value;

        /// <summary>
        /// Gets a positive infinity interval boundary.
        /// </summary>
        public static IntervalBoundary<T> PositiveInfinity { get; } = new(IntervalBoundaryKind.PositiveInfinity, default!);

        /// <summary>
        /// Gets a negative infinity interval boundary.
        /// </summary>
        public static IntervalBoundary<T> NegativeInfinity { get; } = new(IntervalBoundaryKind.NegativeInfinity, default!);
    }
}
