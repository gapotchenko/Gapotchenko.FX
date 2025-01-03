﻿using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions;

/// <summary>
/// Equality comparer for LINQ expressions.
/// </summary>
public sealed class ExpressionEqualityComparer : EqualityComparer<Expression>
{
    /// <summary>
    /// Returns a default equality comparer for LINQ expressions.
    /// </summary>
    public static new ExpressionEqualityComparer Default { get; } = new ExpressionEqualityComparer();

    /// <summary>
    /// Determines whether two <see cref="Expression"/> objects are equal.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(Expression? x, Expression? y)
    {
        if (x == y)
            return true;
        if (x == null || y == null)
            return false;

        var worker = new ExpressionEqualityWorker();
        return worker.AreEqual(x, y);
    }

    /// <summary>
    /// Gets a hash code for a specified <see cref="Expression"/> object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The hash code.</returns>
    public override int GetHashCode(Expression obj)
    {
        if (obj == null)
            return 0;

        var worker = new ExpressionHashCodeWorker();
        return worker.CalculateHashCode(obj);
    }
}
