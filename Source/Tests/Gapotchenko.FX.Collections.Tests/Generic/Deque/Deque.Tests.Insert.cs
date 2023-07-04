// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Utils;
using Gapotchenko.FX.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    const int Insert_DataSize = 24;

    public static IEnumerable<object[]> Insert_ValidIndeces()
    {
        const int n = Insert_DataSize;

        yield return new object[] { 0 };
        yield return new object[] { 1 };

        int half = n / 2;
        yield return new object[] { half - 1 };
        yield return new object[] { half };
        yield return new object[] { half + 1 };

        yield return new object[] { n - 1 };
        yield return new object[] { n };
    }

    [Theory]
    [MemberData(nameof(Insert_ValidIndeces))]
    public void Insert(int index)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(Insert_DataSize).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.Insert(index, item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Insert(index, item));

        Assert.Equal(list, deque);
    }
}
