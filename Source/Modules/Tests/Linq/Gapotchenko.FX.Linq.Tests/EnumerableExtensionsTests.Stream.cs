// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Linq.Tests;

public partial class EnumerableExtensionsTests
{
    [TestMethod]
    public void Linq_Enumerable_Stream()
    {
        var source = Enumerable.Range(1, int.MaxValue).Stream();
        CollectionAssert.AreEqual(new int[] { 1, 2 }, source.Take(2).ToList());
        CollectionAssert.AreEqual(new int[] { 3, 4, 5 }, source.Take(3).ToList());
    }
}
