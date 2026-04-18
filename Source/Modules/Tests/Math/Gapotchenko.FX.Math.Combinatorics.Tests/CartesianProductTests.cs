using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Math.Combinatorics.Tests;

[TestClass]
public class CartesianProductTests
{
    [TestMethod]
    [DataRow(new int[] { 1, 2, 3 }, 6)]
    [DataRow(new int[] { 5 }, 5)]
    [DataRow(new int[] { 5, 6 }, 30)]
    [DataRow(new int[] { 5, 6, 7 }, 210)]
    [DataRow(new int[] { 5, 0, 7 }, 0)]
    [DataRow(new int[] { 0 }, 0)]
    [DataRow(new int[] { }, 1)]
    [DataRow(new int[] { -1 }, null)]
    [DataRow(new int[] { 1, -1, 3 }, null)]
    [DataRow(new int[] { 1, 2, -1 }, null)]
    [DataRow(new int[] { 0, -1 }, null)]
    [DataRow(null, null)]
    public void CartesianProduct_Cardinality(int[]? factors, int? expectedCardinality)
    {
        if (factors is null)
        {
            Assert.IsNull(expectedCardinality);
            Assert.ThrowsExactly<ArgumentNullException>(() => CartesianProduct.Cardinality((int[])null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => CartesianProduct.Cardinality((long[])null!));
        }
        else
        {
            var longFactors = factors.Select(x => (long)x);

            if (expectedCardinality is { } cardinality)
            {
                Assert.AreEqual(cardinality, CartesianProduct.Cardinality(factors));
                Assert.AreEqual(cardinality, CartesianProduct.Cardinality(longFactors));
            }
            else
            {
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CartesianProduct.Cardinality(factors));
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CartesianProduct.Cardinality(longFactors));
            }
        }
    }

    [TestMethod]
    public void CartesianProduct_Of_0()
    {
        var p = CartesianProduct.Of<int>([]).ReifyList();
        Assert.HasCount(1, p);
        Assert.IsEmpty(p[0]);
    }

    [TestMethod]
    public void CartesianProduct_Of_2x0()
    {
        int[][] factors =
        [
            [1, 2],
            []
        ];

        int l1 = factors[0].Length;
        int l2 = factors[1].Length;

        int cardinality = CartesianProduct.Cardinality(factors.Select(x => x.Length));
        Assert.AreEqual(l1 * l2, cardinality);

        var p = CartesianProduct.Of(factors);
        Assert.AreEqual(cardinality, p.Count());
    }

    [TestMethod]
    public void CartesianProduct_Of_2x2_Distinct_Linq()
    {
        int[][] factors =
        [
            [1, 1],
            [2, 3]
        ];

        var p = CartesianProduct.Of(factors).AsEnumerable();
        Assert.HasCount(2, p.Distinct());
    }

    [TestMethod]
    public void CartesianProduct_Of_2x2_Distinct_WithCustomComparer()
    {
        string[][] factors =
        [
            ["a", "A"],
            ["b", "B"]
        ];

        var comparer = StringComparer.OrdinalIgnoreCase;

        var p = CartesianProduct.Of(factors).Distinct(comparer).ReifyCollection();
        Assert.HasCount(1, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer.Create(comparer));

        Assert.HasCount(1, s);
        Assert.Contains(["a", "b"], s);
    }

    [TestMethod]
    public void CartesianProduct_Of_2x3()
    {
        int[][] factors =
        [
            [1, 2],
            [5, 6, 7]
        ];

        int l1 = factors[0].Length;
        int l2 = factors[1].Length;

        int cardinality = CartesianProduct.Cardinality(factors.Select(x => x.Length));
        Assert.AreEqual(l1 * l2, cardinality);

        var p = CartesianProduct.Of(factors).ReifyList();

        Assert.HasCount(cardinality, p);

        Assert.IsTrue(p[0].SequenceEqual([1, 5]));
        Assert.IsTrue(p[1].SequenceEqual([2, 5]));
        Assert.IsTrue(p[2].SequenceEqual([1, 6]));
        Assert.IsTrue(p[3].SequenceEqual([2, 6]));
        Assert.IsTrue(p[4].SequenceEqual([1, 7]));
        Assert.IsTrue(p[5].SequenceEqual([2, 7]));
    }

    [TestMethod]
    public void CartesianProduct_Of_2x3_Projection()
    {
        const int l1 = 2;
        const int l2 = 3;

        int cardinality = CartesianProduct.Cardinality([l1, l2]);
        Assert.AreEqual(l1 * l2, cardinality);

        var p =
            CartesianProduct.Of(
                [1, 2],
                ["A", "B", "C"],
                ValueTuple.Create)
            .ReifyList();

        Assert.HasCount(cardinality, p);

        Assert.AreEqual((1, "A"), p[0]);
        Assert.AreEqual((2, "A"), p[1]);
        Assert.AreEqual((1, "B"), p[2]);
        Assert.AreEqual((2, "B"), p[3]);
        Assert.AreEqual((1, "C"), p[4]);
        Assert.AreEqual((2, "C"), p[5]);
    }
}
