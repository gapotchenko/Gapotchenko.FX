// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class CartesianProduct
{
    static IEnumerable<IRow<T>> Multiply<T>(IEnumerable<IEnumerable<T>> factors)
    {
        var items = MemoizeMultipliers(factors).ReifyList();
        int rank = items.Count;

        if (rank == 0)
        {
            // No multipliers.
            yield break;
        }

        var enumerators = items.Select(x => x.GetEnumerator()).ToArray();

        foreach (var i in enumerators)
        {
            if (!i.MoveNext())
            {
                // At least one multiplier is empty.
                yield break;
            }
        }

        for (; ; )
        {
            var row = new T[rank];
            for (int i = 0; i != rank; i++)
                row[i] = enumerators[i].Current;

            yield return new Row<T>(row);

            for (int i = 0; i != rank; i++)
            {
                var enumerator = enumerators[i];
                if (enumerator.MoveNext())
                    break;

                if (i == rank - 1)
                    yield break;

                // Reset the enumerator by retrieving a new one.
                var newEnumerator = items[i].GetEnumerator();
                if (!newEnumerator.MoveNext())
                    throw new InvalidOperationException("Cartesian product pool is exhausted prematurely.");
                enumerators[i] = newEnumerator;
            }
        }
    }

    static IEnumerable<IEnumerable<T>> MemoizeMultipliers<T>(IEnumerable<IEnumerable<T>> factors)
    {
        var gtc = new GroupTransformCorrelator<IEnumerable<T>>(EnumerableEx.Memoize);
        factors = factors.Memoize();
        foreach (var factor in factors.SkipLast(1))
            gtc.Transform(factor);
        return factors.Select(gtc.Correlate);
    }

    static IEnumerable<IEnumerable<T>> DistinctMultipliers<T>(IEnumerable<IEnumerable<T>> factors, IEqualityComparer<T>? equalityComparer)
    {
        var gtc = new GroupTransformCorrelator<IEnumerable<T>>(x => x.Distinct(equalityComparer));
        factors = factors.Memoize();
        foreach (var factor in factors)
            gtc.Transform(factor);
        return factors.Select(gtc.Correlate);
    }

    sealed class GroupTransformCorrelator<T>(Func<T, T> transform)
        where T : class
    {
        public void Transform(T value)
        {
            if (!m_Map.ContainsKey(value))
                m_Map.Add(value, transform(value));
        }

        public T Correlate(T value) => m_Map.TryGetValue(value, out var transformedValue) ? transformedValue : value;

        readonly Dictionary<T, T> m_Map = new(ReferenceEqualityComparer.Instance);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        Func<T1, T2, TResult> selector)
    {
        factor1 = factor1.Memoize();

        foreach (var i2 in factor2)
            foreach (var i1 in factor1)
                yield return selector(i1, i2);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, T3, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        Func<T1, T2, T3, TResult> selector)
    {
        factor1 = factor1.Memoize();
        factor2 = factor2.Memoize();

        foreach (var i3 in factor3)
            foreach (var i2 in factor2)
                foreach (var i1 in factor1)
                    yield return selector(i1, i2, i3);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, T3, T4, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        Func<T1, T2, T3, T4, TResult> selector)
    {
        factor1 = factor1.Memoize();
        factor2 = factor2.Memoize();
        factor3 = factor3.Memoize();

        foreach (var i4 in factor4)
            foreach (var i3 in factor3)
                foreach (var i2 in factor2)
                    foreach (var i1 in factor1)
                        yield return selector(i1, i2, i3, i4);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, T3, T4, T5, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        Func<T1, T2, T3, T4, T5, TResult> selector)
    {
        factor1 = factor1.Memoize();
        factor2 = factor2.Memoize();
        factor3 = factor3.Memoize();
        factor4 = factor4.Memoize();

        foreach (var i5 in factor5)
            foreach (var i4 in factor4)
                foreach (var i3 in factor3)
                    foreach (var i2 in factor2)
                        foreach (var i1 in factor1)
                            yield return selector(i1, i2, i3, i4, i5);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, T3, T4, T5, T6, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        IEnumerable<T6> factor6,
        Func<T1, T2, T3, T4, T5, T6, TResult> selector)
    {
        factor1 = factor1.Memoize();
        factor2 = factor2.Memoize();
        factor3 = factor3.Memoize();
        factor4 = factor4.Memoize();
        factor5 = factor5.Memoize();

        foreach (var i6 in factor6)
            foreach (var i5 in factor5)
                foreach (var i4 in factor4)
                    foreach (var i3 in factor3)
                        foreach (var i2 in factor2)
                            foreach (var i1 in factor1)
                                yield return selector(i1, i2, i3, i4, i5, i6);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, T3, T4, T5, T6, T7, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        IEnumerable<T6> factor6,
        IEnumerable<T7> factor7,
        Func<T1, T2, T3, T4, T5, T6, T7, TResult> selector)
    {
        factor1 = factor1.Memoize();
        factor2 = factor2.Memoize();
        factor3 = factor3.Memoize();
        factor4 = factor4.Memoize();
        factor5 = factor5.Memoize();
        factor6 = factor6.Memoize();

        foreach (var i7 in factor7)
            foreach (var i6 in factor6)
                foreach (var i5 in factor5)
                    foreach (var i4 in factor4)
                        foreach (var i3 in factor3)
                            foreach (var i2 in factor2)
                                foreach (var i1 in factor1)
                                    yield return selector(i1, i2, i3, i4, i5, i6, i7);
    }

    internal static IEnumerable<TResult> Multiply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        IEnumerable<T6> factor6,
        IEnumerable<T7> factor7,
        IEnumerable<T8> factor8,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> selector)
    {
        factor1 = factor1.Memoize();
        factor2 = factor2.Memoize();
        factor3 = factor3.Memoize();
        factor4 = factor4.Memoize();
        factor5 = factor5.Memoize();
        factor6 = factor6.Memoize();
        factor7 = factor7.Memoize();

        foreach (var i8 in factor8)
            foreach (var i7 in factor7)
                foreach (var i6 in factor6)
                    foreach (var i5 in factor5)
                        foreach (var i4 in factor4)
                            foreach (var i3 in factor3)
                                foreach (var i2 in factor2)
                                    foreach (var i1 in factor1)
                                        yield return selector(i1, i2, i3, i4, i5, i6, i7, i8);
    }
}
