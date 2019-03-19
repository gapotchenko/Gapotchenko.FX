using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Linq.Expressions
{
    /// <summary>
    /// Equality comparer for LINQ expressions.
    /// </summary>
    public sealed class ExpressionEqualityComparer : EqualityComparer<Expression>
    {
        /// <summary>
        /// Determines whether two <see cref="Expression"/> objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(Expression x, Expression y)
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

        /// <summary>
        /// Returns a default equality comparer for LINQ expressions.
        /// </summary>
        public new static ExpressionEqualityComparer Default { get; } = new ExpressionEqualityComparer();
    }
}
