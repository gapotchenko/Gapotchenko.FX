using Gapotchenko.FX.Collections.Generic.Kit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kit;

sealed class ReadOnlySetChimera<T> : ReadOnlySetBase<T>
{
    public ReadOnlySetChimera(HashSet<T> baseSet)
    {
        m_BaseSet = baseSet;
    }

    readonly HashSet<T> m_BaseSet;

    public override IEqualityComparer<T> Comparer => m_BaseSet.Comparer;

    public override int Count => m_BaseSet.Count;

    public override bool Contains(T item) => m_BaseSet.Contains(item);

    public override IEnumerator<T> GetEnumerator() => m_BaseSet.GetEnumerator();

    public static ReadOnlySetChimera<T> Empty => EmptyFactory.Instance;

    static class EmptyFactory
    {
        public static ReadOnlySetChimera<T> Instance { get; } = new(new());
    }
}
