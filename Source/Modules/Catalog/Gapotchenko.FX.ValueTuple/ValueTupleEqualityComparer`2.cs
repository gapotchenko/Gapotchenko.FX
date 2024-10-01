namespace Gapotchenko.FX.ValueTuple;

sealed class ValueTupleEqualityComparer<T1, T2> : IEqualityComparer<ValueTuple<T1, T2>>
{
    public ValueTupleEqualityComparer(IEqualityComparer<T1>? comparer1, IEqualityComparer<T2>? comparer2)
    {
        m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
        m_Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
    }

    readonly IEqualityComparer<T1> m_Comparer1;
    readonly IEqualityComparer<T2> m_Comparer2;

    public bool Equals(ValueTuple<T1, T2> x, ValueTuple<T1, T2> y) =>
        m_Comparer1.Equals(x.Item1, y.Item1) &&
        m_Comparer2.Equals(x.Item2, y.Item2);

    public int GetHashCode(ValueTuple<T1, T2> obj) =>
        HashCode.Combine(
            Util.GetSafeHashCode(m_Comparer1, obj.Item1),
            Util.GetSafeHashCode(m_Comparer2, obj.Item2));
}
