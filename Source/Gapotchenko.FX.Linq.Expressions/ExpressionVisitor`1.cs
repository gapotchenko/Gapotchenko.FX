using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions
{
    abstract class ExpressionVisitor<TResult>
    {
        // [return: AllowNull] // There is an omission in AllowNullAttribute definition that prevents it from being used on return values.
        protected virtual TResult Visit(Expression? expression)
        {
            if (expression == null)
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.

            TResult result;

            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    result = VisitBinary((BinaryExpression)expression);
                    break;

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    result = VisitUnary((UnaryExpression)expression);
                    break;

                case ExpressionType.Call:
                    result = VisitMethodCall((MethodCallExpression)expression);
                    break;

                case ExpressionType.Conditional:
                    result = VisitConditional((ConditionalExpression)expression);
                    break;

                case ExpressionType.Constant:
                    result = VisitConstant((ConstantExpression)expression);
                    break;

                case ExpressionType.Invoke:
                    result = VisitInvocation((InvocationExpression)expression);
                    break;

                case ExpressionType.Lambda:
                    result = VisitLambda((LambdaExpression)expression);
                    break;

                case ExpressionType.ListInit:
                    result = VisitListInit((ListInitExpression)expression);
                    break;

                case ExpressionType.MemberAccess:
                    result = VisitMemberAccess((MemberExpression)expression);
                    break;

                case ExpressionType.MemberInit:
                    result = VisitMemberInit((MemberInitExpression)expression);
                    break;

                case ExpressionType.New:
                    result = VisitNew((NewExpression)expression);
                    break;

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    result = VisitNewArray((NewArrayExpression)expression);
                    break;

                case ExpressionType.Parameter:
                    result = VisitParameter((ParameterExpression)expression);
                    break;

                case ExpressionType.TypeIs:
                    result = VisitTypeIs((TypeBinaryExpression)expression);
                    break;

                default:
                    result = VisitOther(expression);
                    break;
            }

            return result;
        }

        [return: NotNullIfNotNull("expressions")]
        protected virtual IReadOnlyList<TResult>? VisitExpressions(IReadOnlyList<Expression>? expressions)
        {
            if (expressions == null)
                return null;
            else
                return expressions.Select(Visit).AsReadOnly();
        }

        protected abstract TResult VisitBinary(BinaryExpression expression);

        protected abstract TResult VisitUnary(UnaryExpression expression);

        protected abstract TResult VisitMethodCall(MethodCallExpression expression);

        protected abstract TResult VisitConditional(ConditionalExpression expression);

        protected abstract TResult VisitConstant(ConstantExpression expression);

        protected abstract TResult VisitInvocation(InvocationExpression expression);

        protected abstract TResult VisitLambda(LambdaExpression expression);

        protected abstract TResult VisitListInit(ListInitExpression expression);

        protected abstract TResult VisitMemberAccess(MemberExpression expression);

        protected abstract TResult VisitMemberInit(MemberInitExpression expression);

        protected abstract TResult VisitNew(NewExpression expression);

        protected abstract TResult VisitNewArray(NewArrayExpression expression);

        protected abstract TResult VisitParameter(ParameterExpression expression);

        protected abstract TResult VisitTypeIs(TypeBinaryExpression expression);

        protected virtual TResult VisitOther(Expression expression)
        {
            throw new NotSupportedException(string.Format("LINQ expression {0} is not supported.", expression));
        }
    }
}
