using Gapotchenko.FX.Collections.Generic.Kits;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kits;

sealed class SetChimera<T>(HashSet<T> baseSet) : SetKit<T>
{
    public override IEqualityComparer<T> Comparer => baseSet.Comparer;

    public override int Count => baseSet.Count;

    public override bool Contains(T item) => baseSet.Contains(item);

    public override IEnumerator<T> GetEnumerator() => baseSet.GetEnumerator();

    public override bool Add(T item) => baseSet.Add(item);

    public override bool Remove(T item) => baseSet.Remove(item);

    public override void Clear() => baseSet.Clear();
}
