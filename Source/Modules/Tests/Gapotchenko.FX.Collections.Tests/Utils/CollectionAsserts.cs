// (c) Portions of code from the .NET project by Microsoft and .NET Foundation

using System.Collections;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Utils;

static class CollectionAsserts
{
    public static void Equal(ICollection expected, ICollection actual)
    {
        Assert.Equal(expected == null, actual == null);
        if (expected == null)
        {
            return;
        }
        Assert.Equal(expected.Count, actual!.Count);
        IEnumerator e = expected.GetEnumerator();
        IEnumerator a = actual.GetEnumerator();
        while (e.MoveNext())
        {
            Assert.True(a.MoveNext(), "actual has fewer elements");
            if (e.Current == null)
            {
                Assert.Null(a.Current);
            }
            else
            {
                Assert.IsType(e.Current.GetType(), a.Current);
                Assert.Equal(e.Current, a.Current);
            }
        }
        Assert.False(a.MoveNext(), "actual has more elements");
    }

    public static void Equal<T>(ICollection<T> expected, ICollection<T> actual)
    {
        Assert.Equal(expected == null, actual == null);
        if (expected == null)
        {
            return;
        }
        Assert.Equal(expected.Count, actual!.Count);
        IEnumerator<T> e = expected.GetEnumerator();
        IEnumerator<T> a = actual.GetEnumerator();
        while (e.MoveNext())
        {
            Assert.True(a.MoveNext(), "actual has fewer elements");
            if (e.Current == null)
            {
                Assert.Null(a.Current);
            }
            else
            {
                Assert.IsType(e.Current.GetType(), a.Current);
                Assert.Equal(e.Current, a.Current);
            }
        }
        Assert.False(a.MoveNext(), "actual has more elements");
    }

    public static void EqualUnordered(ICollection expected, ICollection actual)
    {
        Assert.Equal(expected == null, actual == null);
        if (expected == null)
        {
            return;
        }

        // Lookups are an aggregated collections (enumerable contents), but ordered.
        ILookup<object, object> e = expected.Cast<object>().ToLookup(key => key);
        ILookup<object, object> a = actual!.Cast<object>().ToLookup(key => key);

        // Dictionaries can't handle null keys, which is a possibility
        Assert.Equal(e.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()), a.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()));

        // Get count of null keys.  Returns an empty sequence (and thus a 0 count) if no null key
        Assert.Equal(e[null!].Count(), a[null!].Count());
    }

    public static void EqualUnordered<T>(ICollection<T> expected, ICollection<T> actual)
    {
        Assert.Equal(expected == null, actual == null);
        if (expected == null)
        {
            return;
        }

        // Lookups are an aggregated collections (enumerable contents), but ordered.
        ILookup<object, object> e = expected.Cast<object>().ToLookup(key => key);
        ILookup<object, object> a = actual!.Cast<object>().ToLookup(key => key);

        // Dictionaries can't handle null keys, which is a possibility
        Assert.Equal(e.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()), a.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()));

        // Get count of null keys.  Returns an empty sequence (and thus a 0 count) if no null key
        Assert.Equal(e[null!].Count(), a[null!].Count());
    }
}
