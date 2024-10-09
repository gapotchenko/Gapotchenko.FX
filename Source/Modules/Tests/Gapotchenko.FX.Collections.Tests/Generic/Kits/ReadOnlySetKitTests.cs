using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kits;

public class ReadOnlySetKitTests
{
    [Fact]
    public void ReadOnlySetKit_IsProperSubsetOf()
    {
        var s1 = new ReadOnlySetChimera<int>([3, 2, 1]);

        foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
            Assert.True(s1.IsProperSubsetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.False(s1.IsProperSubsetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2 }))
            Assert.False(s1.IsProperSubsetOf(s));

        foreach (var s in Util.SetsEnumerable(s1))
            Assert.False(s1.IsProperSubsetOf(s));

        var s2 = ReadOnlySetChimera<int>.Empty;

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.True(s2.IsProperSubsetOf(s));

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.False(s2.IsProperSubsetOf(s));

        foreach (var s in Util.SetsEnumerable(s2))
            Assert.False(s2.IsProperSubsetOf(s));
    }

    [Fact]
    public void ReadOnlySetKit_IsProperSupersetOf()
    {
        var s1 = new ReadOnlySetChimera<int>([3, 2, 1]);

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.True(s1.IsProperSupersetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2 }))
            Assert.True(s1.IsProperSupersetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.False(s1.IsProperSupersetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
            Assert.False(s1.IsProperSupersetOf(s));

        foreach (var s in Util.SetsEnumerable(s1))
            Assert.False(s1.IsProperSupersetOf(s));

        var s2 = ReadOnlySetChimera<int>.Empty;

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.False(s2.IsProperSupersetOf(s));

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.False(s2.IsProperSupersetOf(s));

        foreach (var s in Util.SetsEnumerable(s2))
            Assert.False(s2.IsProperSupersetOf(s));
    }

    [Fact]
    public void ReadOnlySetKit_IsSubsetOf()
    {
        var s1 = new ReadOnlySetChimera<int>([3, 2, 1]);

        foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
            Assert.True(s1.IsSubsetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.True(s1.IsSubsetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2 }))
            Assert.False(s1.IsSubsetOf(s));

        foreach (var s in Util.SetsEnumerable(s1))
            Assert.True(s1.IsSubsetOf(s));

        var s2 = ReadOnlySetChimera<int>.Empty;

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.True(s2.IsSubsetOf(s));

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.True(s2.IsSubsetOf(s));

        foreach (var s in Util.SetsEnumerable(s2))
            Assert.True(s2.IsSubsetOf(s));
    }

    [Fact]
    public void ReadOnlySetKit_IsSupersetOf()
    {
        var s1 = new ReadOnlySetChimera<int>([3, 2, 1]);

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.True(s1.IsSupersetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2 }))
            Assert.True(s1.IsSupersetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.True(s1.IsSupersetOf(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
            Assert.False(s1.IsSupersetOf(s));

        foreach (var s in Util.SetsEnumerable(s1))
            Assert.True(s1.IsSupersetOf(s));
    }

    [Fact]
    public void ReadOnlySetKit_Overlaps()
    {
        var s1 = new ReadOnlySetChimera<int>([3, 2, 1]);

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.False(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 1, 2 }))
            Assert.True(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.True(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
            Assert.True(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 10, 20 }))
            Assert.False(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 10, 20, 30 }))
            Assert.False(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 10, 20, 30, 40 }))
            Assert.False(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 1, 20 }))
            Assert.True(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 10, 2, 30 }))
            Assert.True(s1.Overlaps(s));

        foreach (var s in Util.Sets(new[] { 10, 20, 3, 40 }))
            Assert.True(s1.Overlaps(s));

        foreach (var s in Util.SetsEnumerable(s1))
            Assert.True(s1.Overlaps(s));

        var s2 = ReadOnlySetChimera<int>.Empty;

        foreach (var s in Util.SetsEnumerable(s2))
            Assert.False(s2.Overlaps(s));
    }

    [Fact]
    public void ReadOnlySetKit_SetEquals()
    {
        var s1 = new ReadOnlySetChimera<int>([3, 2, 1]);

        foreach (var s in Util.Sets(Array.Empty<int>()))
            Assert.False(s1.SetEquals(s));

        foreach (var s in Util.Sets(new[] { 1, 2 }))
            Assert.False(s1.SetEquals(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
            Assert.True(s1.SetEquals(s));

        foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
            Assert.False(s1.SetEquals(s));
    }

    [Fact]
    public void ReadOnlySetKit_CopyTo()
    {
        var s1 = new ReadOnlySetChimera<int>([1, 2, 3]);

        int[] arr;

        arr = new int[3];
        s1.CopyTo(arr);
        Assert.True(new HashSet<int>(arr).SetEquals(s1));

        arr = new int[2];
        Assert.Throws<ArgumentException>(() => s1.CopyTo(arr));

        arr = new int[4];
        s1.CopyTo(arr);
        Assert.True(new HashSet<int>(arr.Take(3)).SetEquals(new[] { 1, 2, 3 }));
        Assert.Equal(0, arr[3]);

        arr = new int[4];
        s1.CopyTo(arr, 1);
        Assert.True(new HashSet<int>(arr.Skip(1)).SetEquals(new[] { 1, 2, 3 }));
        Assert.Equal(0, arr[0]);

        arr = new int[4];
        s1.CopyTo(arr, 1, 2);
        Assert.True(new HashSet<int>(arr.Skip(1).Take(2)).SetEquals(new[] { 1, 2 }));
        Assert.Equal(0, arr[0]);
        Assert.Equal(0, arr[3]);
    }
}
