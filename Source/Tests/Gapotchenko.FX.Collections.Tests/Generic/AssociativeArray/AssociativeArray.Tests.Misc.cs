using Gapotchenko.FX.Collections.Generic;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.AssociativeArray;

public class AssociativeArray_Tests_Misc
{
    [Fact]
    public void AssociativeArray_NullTricks()
    {
        var array = new AssociativeArray<string?, string?>();
        Assert.False(array.ContainsKey(null));

        array.Add(null, null);
        Assert.True(array.ContainsKey(null));
        Assert.Null(array[null]);

        array[null] = string.Empty;
        Assert.Equal(string.Empty, array[null]);

        array.Remove(null);
        Assert.False(array.ContainsKey(null));

        array.AddRange(new[]
        {
            new KeyValuePair<string?, string?>(null, string.Empty),
            new KeyValuePair<string?, string?>(string.Empty, null),
        });
        Assert.Equal(2, array.Count);
        Assert.Equal(new[] { null, string.Empty }, array.Keys);
        Assert.Equal(new[] { string.Empty, null }, array.Values);
    }

    [Fact]
    public void AssociativeArray_CtorTricks()
    {
        var src1 = new AssociativeArray<string?, string>
        {
            ["a"] = "b",
            [null] = string.Empty
        };

        var dst = new AssociativeArray<string?, string>(src1);
        Assert.Equal("b", dst["a"]);
        Assert.Equal(string.Empty, dst[null]);

        var src2 = new Dictionary<string, string>
        {
            ["a"] = "b",
        };

#pragma warning disable CS8620
        dst = new AssociativeArray<string?, string>(src2);
#pragma warning restore CS8620
        Assert.Equal("b", dst["a"]);
        Assert.False(dst.ContainsKey(null));

        var src3 = new[]
        {
            new KeyValuePair<string?, string>("a", "b"),
            new KeyValuePair<string?, string>(null, string.Empty),
        };
        dst = new AssociativeArray<string?, string>(src3);
        Assert.Equal("b", dst["a"]);
        Assert.Equal(string.Empty, dst[null]);
    }
}
