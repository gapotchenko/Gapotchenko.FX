// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

#pragma warning disable CA1036 // Override methods on comparable types

namespace Gapotchenko.FX;

/// <summary>
/// A special type that allows only one value and thus can hold no information.
/// </summary>
public sealed class Unit : IEmptiable<Unit>, IEquatable<Unit>, IComparable<Unit>
{
    Unit()
    {
    }

    /// <summary>
    /// Gets the value of <see cref="Unit"/> type.
    /// </summary>
    public static Unit Value { get; } = new();

    #region Emptiness

    bool IEmptiable.IsEmpty => true;

#if TFF_STATIC_INTERFACE
    static Unit IEmptiable<Unit>.Empty => Value;
#endif

    #endregion

    #region Equality

    /// <inheritdoc/>
    public bool Equals(Unit? other) => other is not null;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Unit other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => ~0;

    #endregion

    #region Comparison

    /// <inheritdoc/>
    public int CompareTo(Unit? other) => other is null ? -1 : 0;

    #endregion
}
