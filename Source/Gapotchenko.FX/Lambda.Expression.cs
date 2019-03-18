using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Gapotchenko.FX
{
    partial class Lambda
    {
        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda function delegate specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action> Expression(Expression<Action> expression) => expression;
    }
}
