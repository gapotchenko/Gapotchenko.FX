using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions;

sealed class ExpressionHashCodeWorker : ExpressionVisitor<int>
{
    public int CalculateHashCode(Expression expression) => Visit(expression);

    readonly ParameterExpressionRegistry _Parameters = new();

    protected override int Visit(Expression? e)
    {
        if (e == null)
            return 0;
        else
            return HashCode.Combine(base.Visit(e), e.NodeType, e.Type);
    }

    protected override int VisitBinary(BinaryExpression b) =>
        HashCode.Combine(Visit(b.Left), Visit(b.Right), b.Method);

    protected override int VisitUnary(UnaryExpression u) =>
        HashCode.Combine(Visit(u.Operand), u.Method);

    protected override int VisitMethodCall(MethodCallExpression mc)
    {
        var hc = new HashCode();
        hc.Add(Visit(mc.Object));
        hc.Add(mc.Method);
        hc.AddRange(mc.Arguments.Select(Visit));
        return hc.ToHashCode();
    }

    protected override int VisitConditional(ConditionalExpression c) =>
        HashCode.Combine(Visit(c.Test), Visit(c.IfTrue), Visit(c.IfFalse));

    protected override int VisitConstant(ConstantExpression c) => _GetPrimitiveHashCode(c.Value);

    protected override int VisitInvocation(InvocationExpression i)
    {
        var hc = new HashCode();
        hc.AddRange(i.Arguments.Select(Visit));
        hc.Add(Visit(i.Expression));
        return hc.ToHashCode();
    }

    protected override int VisitLambda(LambdaExpression l)
    {
        _Parameters.AddRange(l.Parameters);

        var hc = new HashCode();
        hc.AddRange(l.Parameters.Select(Visit));
        hc.Add(Visit(l.Body));
        return hc.ToHashCode();
    }

    protected override int VisitListInit(ListInitExpression li)
    {
        var hc = new HashCode();
        hc.Add(VisitNew(li.NewExpression));
        foreach (var i in li.Initializers)
        {
            hc.Add(i.AddMethod);
            hc.AddRange(i.Arguments.Select(Visit));
        }
        return hc.ToHashCode();
    }

    protected override int VisitMemberAccess(MemberExpression m) =>
        HashCode.Combine(Visit(m.Expression), m.Member);

    protected override int VisitMemberInit(MemberInitExpression mi)
    {
        var hc = new HashCode();
        hc.Add(Visit(mi.NewExpression));
        foreach (var i in mi.Bindings)
        {
            hc.Add(i.BindingType);
            hc.Add(i.Member);
        }
        return hc.ToHashCode();
    }

    protected override int VisitNew(NewExpression n)
    {
        var hc = new HashCode();
        hc.Add(n.Constructor);
        hc.AddRange(n.Members);
        hc.AddRange(n.Arguments.Select(Visit));
        return hc.ToHashCode();
    }

    protected override int VisitNewArray(NewArrayExpression na) => HashCodeEx.SequenceCombine(na.Expressions.Select(Visit));

    protected override int VisitParameter(ParameterExpression p) => _Parameters.GetIndex(p);

    protected override int VisitTypeIs(TypeBinaryExpression tb) =>
        HashCode.Combine(Visit(tb.Expression), tb.TypeOperand);

    protected override int VisitOther(Expression e) => HashCode.Combine(e);

    int _GetPrimitiveHashCode(object? obj) =>
        obj switch
        {
            null => 0,
            IEnumerable<object> e => HashCodeEx.SequenceCombine(e.Select(_GetPrimitiveHashCode)),
            string s => StringComparer.Ordinal.GetHashCode(s),
            Expression expr => Visit(expr),
            _ => obj.GetHashCode(),
        };
}
