using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions;

sealed class ExpressionEqualityWorker
{
    public bool AreEqual(Expression x, Expression y) => _Visit(x, y);

    readonly ParameterExpressionRegistry m_XParameters = new ParameterExpressionRegistry();
    readonly ParameterExpressionRegistry m_YParameters = new ParameterExpressionRegistry();

    bool _Visit(Expression? x, Expression? y)
    {
        if (x == y)
            return true;
        if (x == null || y == null)
            return false;

        if (x.NodeType != y.NodeType || x.Type != y.Type)
            return false;

        return x.NodeType switch
        {
            ExpressionType.Add or ExpressionType.AddChecked or ExpressionType.And or ExpressionType.AndAlso or ExpressionType.ArrayIndex or ExpressionType.Coalesce or ExpressionType.Divide or ExpressionType.Equal or ExpressionType.ExclusiveOr or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.LeftShift or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.Modulo or ExpressionType.Multiply or ExpressionType.MultiplyChecked or ExpressionType.NotEqual or ExpressionType.Or or ExpressionType.OrElse or ExpressionType.RightShift or ExpressionType.Subtract or ExpressionType.SubtractChecked => _VisitBinary((BinaryExpression)x, (BinaryExpression)y),
            ExpressionType.ArrayLength or ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.Negate or ExpressionType.NegateChecked or ExpressionType.Not or ExpressionType.Quote or ExpressionType.TypeAs => _VisitUnary((UnaryExpression)x, (UnaryExpression)y),
            ExpressionType.Call => _VisitMethodCall((MethodCallExpression)x, (MethodCallExpression)y),
            ExpressionType.Conditional => _VisitConditional((ConditionalExpression)x, (ConditionalExpression)y),
            ExpressionType.Constant => _VisitConstant((ConstantExpression)x, (ConstantExpression)y),
            ExpressionType.Invoke => _VisitInvocation((InvocationExpression)x, (InvocationExpression)y),
            ExpressionType.Lambda => _VisitLambda((LambdaExpression)x, (LambdaExpression)y),
            ExpressionType.ListInit => _VisitListInit((ListInitExpression)x, (ListInitExpression)y),
            ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)x, (MemberExpression)y),
            ExpressionType.MemberInit => _VisitMemberInit((MemberInitExpression)x, (MemberInitExpression)y),
            ExpressionType.New => _VisitNew((NewExpression)x, (NewExpression)y),
            ExpressionType.NewArrayInit or ExpressionType.NewArrayBounds => _VisitNewArray((NewArrayExpression)x, (NewArrayExpression)y),
            ExpressionType.Parameter => _VisitParameter((ParameterExpression)x, (ParameterExpression)y),
            ExpressionType.TypeIs => _VisitTypeIs((TypeBinaryExpression)x, (TypeBinaryExpression)y),
            _ => x.Equals(y),
        };
    }

    bool _VisitListInit(ListInitExpression x, ListInitExpression y)
    {
        if (_VisitNew(x.NewExpression, y.NewExpression) && x.Initializers.Count == y.Initializers.Count)
            return Enumerable.All(x.Initializers.Zip(y.Initializers), p => _VisitElementInit(p.Key, p.Value));
        return false;
    }

    bool _VisitMemberInit(MemberInitExpression x, MemberInitExpression y)
    {
        if (x.Bindings.Count != y.Bindings.Count)
            return false;
        if (!_VisitNew(x.NewExpression, y.NewExpression))
            return false;
        if (!x.Bindings.Zip(y.Bindings).All(p => _VisitMemberBinding(p.Key, p.Value)))
            return false;
        return true;
    }

    bool _VisitMemberBinding(MemberBinding x, MemberBinding y)
    {
        if (x.BindingType != y.BindingType || x.Member != y.Member)
            return false;

        switch (x.BindingType)
        {
            case MemberBindingType.Assignment:
                return _Visit(((MemberAssignment)x).Expression, ((MemberAssignment)y).Expression);

            case MemberBindingType.MemberBinding:
                var memberMemberBinding1 = (MemberMemberBinding)x;
                var memberMemberBinding2 = (MemberMemberBinding)y;
                if (memberMemberBinding1.Bindings.Count == memberMemberBinding2.Bindings.Count)
                    return Enumerable.All(memberMemberBinding1.Bindings.Zip(memberMemberBinding2.Bindings), p => _VisitMemberBinding(p.Key, p.Value));
                return false;

            case MemberBindingType.ListBinding:
                var memberListBinding1 = (MemberListBinding)x;
                var memberListBinding2 = (MemberListBinding)y;
                if (memberListBinding1.Initializers.Count == memberListBinding2.Initializers.Count)
                    return Enumerable.All(memberListBinding1.Initializers.Zip(memberListBinding2.Initializers), p => _VisitElementInit(p.Key, p.Value));
                return false;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    bool _VisitElementInit(ElementInit x, ElementInit y)
    {
        if (x.AddMethod == y.AddMethod)
            return _ExpressionsEqual(x.Arguments, y.Arguments);
        return false;
    }

    bool _VisitInvocation(InvocationExpression x, InvocationExpression y)
    {
        if (!_Visit(x.Expression, y.Expression))
            return false;
        if (!_ExpressionsEqual(x.Arguments, y.Arguments))
            return false;
        return true;
    }

    bool _VisitNewArray(NewArrayExpression x, NewArrayExpression y)
    {
        return _ExpressionsEqual(x.Expressions, y.Expressions);
    }

    bool _VisitNew(NewExpression x, NewExpression y)
    {
        if (x.Constructor != y.Constructor || !_ExpressionsEqual(x.Arguments, y.Arguments))
            return false;
        return ArrayEqualityComparer.Equals(x.Members, y.Members);
    }

    bool _VisitLambda(LambdaExpression x, LambdaExpression y)
    {
        m_XParameters.AddRange(x.Parameters);
        m_YParameters.AddRange(y.Parameters);
        if (_ExpressionsEqual(x.Parameters, y.Parameters))
            return _Visit(x.Body, y.Body);
        return false;
    }

    bool _VisitMethodCall(MethodCallExpression x, MethodCallExpression y)
    {
        if (x.Method == y.Method && _Visit(x.Object, y.Object))
            return _ExpressionsEqual(x.Arguments, y.Arguments);
        return false;
    }

    bool VisitMemberAccess(MemberExpression x, MemberExpression y)
    {
        if (x.Member == y.Member)
            return _Visit(x.Expression, y.Expression);
        return false;
    }

    bool _VisitParameter(ParameterExpression x, ParameterExpression y) => m_XParameters.GetIndex(x) == m_YParameters.GetIndex(y);

    bool _VisitConstant(ConstantExpression x, ConstantExpression y) => _PrimitivesEqual(x.Value, y.Value);

    bool _VisitConditional(ConditionalExpression x, ConditionalExpression y)
    {
        if (_Visit(x.Test, y.Test) && _Visit(x.IfTrue, y.IfTrue))
            return _Visit(x.IfFalse, y.IfFalse);
        return false;
    }

    bool _VisitTypeIs(TypeBinaryExpression x, TypeBinaryExpression y)
    {
        if (x.TypeOperand == y.TypeOperand)
            return _Visit(x.Expression, y.Expression);
        return false;
    }

    bool _VisitBinary(BinaryExpression x, BinaryExpression y)
    {
        if (x.Method == y.Method && _Visit(x.Left, y.Left))
            return _Visit(x.Right, y.Right);
        return false;
    }

    bool _VisitUnary(UnaryExpression x, UnaryExpression y)
    {
        if (x.Method == y.Method)
            return _Visit(x.Operand, y.Operand);
        return false;
    }

    bool _ExpressionsEqual<T>(ReadOnlyCollection<T> x, ReadOnlyCollection<T> y) where T : Expression =>
        x.SequenceEqual(y, _Visit);

    bool _PrimitivesEqual(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;

        if (x is IEnumerable<object> xe && y is IEnumerable<object> ye)
            return xe.SequenceEqual(ye, _PrimitivesEqual);

        if (x is string xs && y is string ys)
            return string.Equals(xs, ys, StringComparison.Ordinal);

        if (x is Expression xExpr && y is Expression yExpr)
            return _Visit(xExpr, yExpr);

        return x.Equals(y);
    }
}
