using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions
{
    sealed class ExpressionEqualityWorker
    {
        public bool AreEqual(Expression x, Expression y) => _Visit(x, y);

        readonly ParameterExpressionRegistry _XParameters = new ParameterExpressionRegistry();
        readonly ParameterExpressionRegistry _YParameters = new ParameterExpressionRegistry();

        bool _Visit(Expression? x, Expression? y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;

            if (x.NodeType != y.NodeType || x.Type != y.Type)
                return false;

            switch (x.NodeType)
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
                    return _VisitBinary((BinaryExpression)x, (BinaryExpression)y);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return _VisitUnary((UnaryExpression)x, (UnaryExpression)y);

                case ExpressionType.Call:
                    return _VisitMethodCall((MethodCallExpression)x, (MethodCallExpression)y);

                case ExpressionType.Conditional:
                    return _VisitConditional((ConditionalExpression)x, (ConditionalExpression)y);

                case ExpressionType.Constant:
                    return _VisitConstant((ConstantExpression)x, (ConstantExpression)y);

                case ExpressionType.Invoke:
                    return _VisitInvocation((InvocationExpression)x, (InvocationExpression)y);

                case ExpressionType.Lambda:
                    return _VisitLambda((LambdaExpression)x, (LambdaExpression)y);

                case ExpressionType.ListInit:
                    return _VisitListInit((ListInitExpression)x, (ListInitExpression)y);

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)x, (MemberExpression)y);

                case ExpressionType.MemberInit:
                    return _VisitMemberInit((MemberInitExpression)x, (MemberInitExpression)y);

                case ExpressionType.New:
                    return _VisitNew((NewExpression)x, (NewExpression)y);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return _VisitNewArray((NewArrayExpression)x, (NewArrayExpression)y);

                case ExpressionType.Parameter:
                    return _VisitParameter((ParameterExpression)x, (ParameterExpression)y);

                case ExpressionType.TypeIs:
                    return _VisitTypeIs((TypeBinaryExpression)x, (TypeBinaryExpression)y);

                default:
                    return x.Equals(y);
            }
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
            _XParameters.AddRange(x.Parameters);
            _YParameters.AddRange(y.Parameters);
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

        bool _VisitParameter(ParameterExpression x, ParameterExpression y)
        {
            return _XParameters.GetIndex(x) == _YParameters.GetIndex(y);
        }

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
}
