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
        public override bool Equals(Expression x, Expression y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;

            var worker = new ExpressionEqualityWorker();
            return worker.AreEqual(x, y);
        }

        public override int GetHashCode(Expression obj)
        {
            if (obj == null)
                return 0;

            var worker = new ExpressionHashCodeWorker();
            return worker.CalculateHashCode(obj);
        }

        /// <summary>
        /// Returns the default instance of equality comparer for LINQ expressions.
        /// </summary>
        public new static IEqualityComparer<Expression> Default { get; } = new ExpressionEqualityComparer();
    }
}
