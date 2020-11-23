using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class CartesianProduct
    {
        static IEnumerable<Row<T>> Multiply<T>(IEnumerable<IEnumerable<T>> factors)
        {
            var items = factors.SelectExceptLast(EnumerableEx.Memoize).AsReadOnly();

            int rank = items.Count;

            if (rank == 0)
                yield break;

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
                var result = new T[rank];
                for (int i = 0; i != rank; i++)
                    result[i] = enumerators[i].Current;

                yield return new Row<T>(result);

                for (int i = 0; i != rank; i++)
                {
                    var enumerator = enumerators[i];
                    if (enumerator.MoveNext())
                        break;

                    var newEnumerator = items[i].GetEnumerator();
                    if (!newEnumerator.MoveNext())
                        throw new InvalidOperationException("Cartesian product pool is prematurely exhausted.");

                    enumerators[i] = newEnumerator;

                    if (i == rank - 1)
                        yield break;
                }
            }
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
    }
}
