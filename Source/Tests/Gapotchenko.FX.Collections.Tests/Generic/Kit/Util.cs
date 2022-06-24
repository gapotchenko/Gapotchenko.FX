using Gapotchenko.FX.Math.Combinatorics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kit;

static class Util
{
    public static IEnumerable<IEnumerable<T>> SetsEnumerable<T>(IEnumerable<T> elements)
    {
        yield return elements;

        static IEnumerable<IEnumerable<T>> Repack(IEnumerable<T> elements)
        {
            yield return elements;

            static IEnumerable<T> Enumerate(IEnumerable<T> source)
            {
                foreach (var i in source)
                    yield return i;
            }

            yield return Enumerate(elements);

            if (elements is not Array)
                yield return elements.ToArray();

            var hs = new HashSet<T>(elements);
            yield return hs;

            yield return new ReadOnlySetChimera<T>(hs);
        }

        foreach (var p in elements.Permute())
            foreach (var i in Repack(p))
                yield return i;
    }

    public static IEnumerable<IEnumerable<T>> Sets<T>(params T[] elements) => SetsEnumerable(elements);
}
