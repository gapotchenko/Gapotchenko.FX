using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kits;

public class SetKitTests
{
    [Fact]
    public void SetKit_ExceptWith()
    {
        var s1 = new SetChimera<int>([1, 2, 3, 4, 5]);
        s1.ExceptWith([1, 3]);
        s1.SetEquals([2, 4, 5]);

        s1.ExceptWith(s1);
        Assert.Equal(0, s1.Count);
    }

    [Fact]
    public void SetKit_IntersectWith()
    {
        var s1 = new SetChimera<int>([]);
        s1.IntersectWith([1, 2, 3]);
        Assert.Equal(0, s1.Count);

        s1 = new SetChimera<int>([1, 2, 3]);
        s1.IntersectWith([]);
        Assert.Equal(0, s1.Count);

        foreach (var s2 in Util.SetsOf<int>())
        {
            s1 = new SetChimera<int>([1, 2, 3, 4, 5]);
            s1.IntersectWith(s2);
            Assert.Equal(0, s1.Count);
        }

        foreach (var s2 in Util.SetsOf(10, 2, 3, 40))
        {
            s1 = new SetChimera<int>([1, 2, 3, 4, 5]);
            s1.IntersectWith(s2);
            s1.SetEquals([2, 3]);
        }
    }

    [Fact]
    public void SetKit_SymmetricExceptWith()
    {
        foreach (var s2 in Util.SetsOf(3, 4, 5))
        {
            var s1 = new SetChimera<int>([1, 2, 3]);
            s1.SymmetricExceptWith(s2);
            s1.SetEquals([1, 2, 4, 5]);
        }
    }

    [Fact]
    public void SetKit_UnionWith()
    {
        var s1 = new SetChimera<int>([1, 2, 3]);
        s1.UnionWith([3, 4, 5]);
        s1.SetEquals([1, 2, 3, 4, 5]);
    }
}
