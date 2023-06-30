using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions;

abstract class ExpressionVisitor<TResult> where TResult : struct
{
    protected virtual TResult Visit(Expression? expression)
    {
        if (expression == null)
            return default;

        return expression.NodeType switch
        {
            ExpressionType.Add or ExpressionType.AddChecked or ExpressionType.And or ExpressionType.AndAlso or ExpressionType.ArrayIndex or ExpressionType.Coalesce or ExpressionType.Divide or ExpressionType.Equal or ExpressionType.ExclusiveOr or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.LeftShift or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.Modulo or ExpressionType.Multiply or ExpressionType.MultiplyChecked or ExpressionType.NotEqual or ExpressionType.Or or ExpressionType.OrElse or ExpressionType.RightShift or ExpressionType.Subtract or ExpressionType.SubtractChecked => VisitBinary((BinaryExpression)expression),
            ExpressionType.ArrayLength or ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.Negate or ExpressionType.NegateChecked or ExpressionType.Not or ExpressionType.Quote or ExpressionType.TypeAs => VisitUnary((UnaryExpression)expression),
            ExpressionType.Call => VisitMethodCall((MethodCallExpression)expression),
            ExpressionType.Conditional => VisitConditional((ConditionalExpression)expression),
            ExpressionType.Constant => VisitConstant((ConstantExpression)expression),
            ExpressionType.Invoke => VisitInvocation((InvocationExpression)expression),
            ExpressionType.Lambda => VisitLambda((LambdaExpression)expression),
            ExpressionType.ListInit => VisitListInit((ListInitExpression)expression),
            ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)expression),
            ExpressionType.MemberInit => VisitMemberInit((MemberInitExpression)expression),
            ExpressionType.New => VisitNew((NewExpression)expression),
            ExpressionType.NewArrayInit or ExpressionType.NewArrayBounds => VisitNewArray((NewArrayExpression)expression),
            ExpressionType.Parameter => VisitParameter((ParameterExpression)expression),
            ExpressionType.TypeIs => VisitTypeIs((TypeBinaryExpression)expression),
            _ => VisitOther(expression),
        };
    }

    [return: NotNullIfNotNull("expressions")]
    protected virtual IReadOnlyList<TResult>? VisitExpressions(IReadOnlyList<Expression?>? expressions)
    {
        if (expressions == null)
            return null;
        else
            return expressions.Select(Visit).ReifyList();
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
