using Gapotchenko.FX.Collections.Concurrent;
using System;
using System.Collections.Generic;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Concurrent;

public class ConcurrentHashSetTests
{
    [Fact]
    public void ConcurrentHashSet_Ctor_1()
    {
        var hashSet = new ConcurrentHashSet<string>((IEqualityComparer<string>?)null);
        Assert.Empty(hashSet);
    }

    [Fact]
    public void ConcurrentHashSet_Ctor_2()
    {
        var hashSet = new ConcurrentHashSet<string>(StringComparer.Ordinal);
        Assert.Empty(hashSet);
    }

    [Fact]
    public void ConcurrentHashSet_NullStringKey_DefaultEqualityComparer()
    {
        var hashSet = new ConcurrentHashSet<string?>();

        Assert.DoesNotContain(null, hashSet);

        hashSet.Add(null);
        Assert.Contains(null, hashSet);

        Assert.Single(hashSet);

        Assert.True(hashSet.Add("A"));
        Assert.True(hashSet.Add("B"));
        Assert.True(hashSet.Add("C"));
        Assert.True(hashSet.Add("a"));
        Assert.True(hashSet.Add("b"));
        Assert.True(hashSet.Add("c"));

        Assert.Equal(7, hashSet.Count);

        Assert.Contains(null, hashSet);
        Assert.Contains("A", hashSet);
        Assert.Contains("B", hashSet);
        Assert.Contains("C", hashSet);
        Assert.DoesNotContain("D", hashSet);
        Assert.Contains("a", hashSet);
        Assert.Contains("b", hashSet);
        Assert.Contains("c", hashSet);
        Assert.DoesNotContain("d", hashSet);
    }

    [Fact]
    public void ConcurrentHashSet_NullStringKey_OicEqualityComparer()
    {
        var hashSet = new ConcurrentHashSet<string?>(StringComparer.OrdinalIgnoreCase);

        Assert.DoesNotContain(null, hashSet);

        hashSet.Add(null);
        Assert.Contains(null, hashSet);

        Assert.Single(hashSet);

        Assert.True(hashSet.Add("A"));
        Assert.True(hashSet.Add("B"));
        Assert.True(hashSet.Add("C"));
        Assert.False(hashSet.Add("a"));
        Assert.False(hashSet.Add("b"));
        Assert.False(hashSet.Add("c"));

        Assert.Equal(4, hashSet.Count);

        Assert.Contains(null, hashSet);
        Assert.Contains("A", hashSet);
        Assert.Contains("B", hashSet);
        Assert.Contains("C", hashSet);
        Assert.DoesNotContain("D", hashSet);
        Assert.Contains("a", hashSet);
        Assert.Contains("b", hashSet);
        Assert.Contains("c", hashSet);
        Assert.DoesNotContain("d", hashSet);
    }
}
