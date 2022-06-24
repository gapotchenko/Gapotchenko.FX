using System;
using System.Linq;
using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Combinatorics.Tests;

[TestClass]
public class CartesianProductTests
{
    [TestMethod]
    public void CartesianProduct_Of_2x0()
    {
        const int l1 = 2;
        const int l2 = 0;

        var factors = new[]
        {
            new int[l1] { 1, 2 },
            new int[l2] { }
        };

        int cardinality = CartesianProduct.Cardinality(factors.Select(x => x.Length));
        Assert.AreEqual(l1 * l2, cardinality);

        var p = CartesianProduct.Of(factors);
        Assert.AreEqual(cardinality, p.Count());
    }

    [TestMethod]
    public void CartesianProduct_Of_2x3()
    {
        const int l1 = 2;
        const int l2 = 3;

        var factors = new[]
        {
            new int[l1] { 1, 2 },
            new int[l2] { 5, 6, 7 }
        };

        int cardinality = CartesianProduct.Cardinality(factors.Select(x => x.Length));
        Assert.AreEqual(l1 * l2, cardinality);

        var p = CartesianProduct.Of(factors).AsReadOnlyList();

        Assert.AreEqual(cardinality, p.Count);

        Assert.IsTrue(p[0].SequenceEqual(new[] { 1, 5 }));
        Assert.IsTrue(p[1].SequenceEqual(new[] { 2, 5 }));
        Assert.IsTrue(p[2].SequenceEqual(new[] { 1, 6 }));
        Assert.IsTrue(p[3].SequenceEqual(new[] { 2, 6 }));
        Assert.IsTrue(p[4].SequenceEqual(new[] { 1, 7 }));
        Assert.IsTrue(p[5].SequenceEqual(new[] { 2, 7 }));
    }

    [TestMethod]
    public void CartesianProduct_Of_2x3_Projection()
    {
        const int l1 = 2;
        const int l2 = 3;

        int cardinality = CartesianProduct.Cardinality(new[] { l1, l2 });
        Assert.AreEqual(l1 * l2, cardinality);

        var p =
            CartesianProduct.Of(
                new int[l1] { 1, 2 },
                new string[l2] { "A", "B", "C" },
                ValueTuple.Create)
            .AsReadOnlyList();

        Assert.AreEqual(cardinality, p.Count);

        Assert.AreEqual((1, "A"), p[0]);
        Assert.AreEqual((2, "A"), p[1]);
        Assert.AreEqual((1, "B"), p[2]);
        Assert.AreEqual((2, "B"), p[3]);
        Assert.AreEqual((1, "C"), p[4]);
        Assert.AreEqual((2, "C"), p[5]);
    }
}
