using Gapotchenko.FX.Collections.Generic.Kits;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kits;

sealed class ReadOnlySetChimera<T>(HashSet<T> baseSet) : ReadOnlySetBase<T>, IEmptiable<ReadOnlySetChimera<T>>
{
    public override IEqualityComparer<T> Comparer => baseSet.Comparer;

    public override int Count => baseSet.Count;

    public override bool Contains(T item) => baseSet.Contains(item);

    public override IEnumerator<T> GetEnumerator() => baseSet.GetEnumerator();

    public bool IsEmpty => Count == 0;

    public static ReadOnlySetChimera<T> Empty => EmptyFactory.Instance;

    static class EmptyFactory
    {
        public static ReadOnlySetChimera<T> Instance { get; } = new([]);
    }
}
