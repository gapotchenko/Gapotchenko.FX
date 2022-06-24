using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class CartesianProduct
{
    static IEnumerable<IRow<T>> Multiply<T>(IEnumerable<IEnumerable<T>> factors)
    {
        var items = MemoizeMultipliers(factors).AsReadOnlyList();
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
        return factors.Select(x => gtc.Correlate(x));
    }

    static IEnumerable<IEnumerable<T>> DistinctMultipliers<T>(IEnumerable<IEnumerable<T>> factors, IEqualityComparer<T>? equalityComparer)
    {
        var gtc = new GroupTransformCorrelator<IEnumerable<T>>(x => Enumerable.Distinct(x, equalityComparer));
        factors = factors.Memoize();
        foreach (var factor in factors)
            gtc.Transform(factor);
        return factors.Select(x => gtc.Correlate(x));
    }

    sealed class GroupTransformCorrelator<T> where T : class
    {
        public GroupTransformCorrelator(Func<T, T> transform)
        {
            m_Transform = transform;
            m_Map = new Dictionary<T, T>(ReferenceEqualityComparer.Instance);
        }

        readonly Func<T, T> m_Transform;
        readonly Dictionary<T, T> m_Map;

        public void Transform(T value)
        {
            if (!m_Map.ContainsKey(value))
                m_Map.Add(value, m_Transform(value));
        }

        public T Correlate(T value) => m_Map.TryGetValue(value, out var transformedValue) ? transformedValue : value;
    }

    internal static IEnumerable<TResult> Multiply<TFirst, TSecond, TResult>(
        IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        first = first.Memoize();

        foreach (var i2 in second)
            foreach (var i1 in first)
                yield return resultSelector(i1, i2);
    }

    internal static IEnumerable<TResult> Multiply<TFirst, TSecond, TThird, TResult>(
        IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        Func<TFirst, TSecond, TThird, TResult> resultSelector)
    {
        first = first.Memoize();
        second = second.Memoize();

        foreach (var i3 in third)
            foreach (var i2 in second)
                foreach (var i1 in first)
                    yield return resultSelector(i1, i2, i3);
    }

    internal static IEnumerable<TResult> Multiply<TFirst, TSecond, TThird, TFourth, TResult>(
        IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        IEnumerable<TFourth> fourth,
        Func<TFirst, TSecond, TThird, TFourth, TResult> resultSelector)
    {
        first = first.Memoize();
        second = second.Memoize();
        third = third.Memoize();

        foreach (var i4 in fourth)
            foreach (var i3 in third)
                foreach (var i2 in second)
                    foreach (var i1 in first)
                        yield return resultSelector(i1, i2, i3, i4);
    }
}
