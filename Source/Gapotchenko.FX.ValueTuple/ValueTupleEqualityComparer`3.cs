namespace Gapotchenko.FX.ValueTuple;

sealed class ValueTupleEqualityComparer<T1, T2, T3> : IEqualityComparer<ValueTuple<T1, T2, T3>>
{
    public ValueTupleEqualityComparer(IEqualityComparer<T1>? comparer1, IEqualityComparer<T2>? comparer2, IEqualityComparer<T3>? comparer3)
    {
        m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
        m_Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
        m_Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
    }

    readonly IEqualityComparer<T1> m_Comparer1;
    readonly IEqualityComparer<T2> m_Comparer2;
    readonly IEqualityComparer<T3> m_Comparer3;

    public bool Equals(ValueTuple<T1, T2, T3> x, ValueTuple<T1, T2, T3> y) =>
        m_Comparer1.Equals(x.Item1, y.Item1) &&
        m_Comparer2.Equals(x.Item2, y.Item2) &&
        m_Comparer3.Equals(x.Item3, y.Item3);

    public int GetHashCode(ValueTuple<T1, T2, T3> obj) =>
        HashCode.Combine(
            Util.GetSafeHashCode(m_Comparer1, obj.Item1),
            Util.GetSafeHashCode(m_Comparer2, obj.Item2),
            Util.GetSafeHashCode(m_Comparer3, obj.Item3));
}
