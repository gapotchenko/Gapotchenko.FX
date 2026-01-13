using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Math.Combinatorics.Tests;

[TestClass]
public class PermutationTests
{
    [TestMethod]
    [DataRow(new[] { 1, 2 })]
    [DataRow(new[] { 2, 1 })]
    public void Permutations_Of2UniqueElements(int[] source)
    {
        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(2, cardinality);

        var p = Permutations.Of(source).ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 2], s);
        Assert.Contains([2, 1], s);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2 })]
    [DataRow(new[] { 2, 1 })]
    public void Permutations_Of2UniqueElements_Distinct(int[] source)
    {
        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(2, cardinality);

        var p = Permutations.Of(source).Distinct().ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 2], s);
        Assert.Contains([2, 1], s);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2 })]
    [DataRow(new[] { 2, 1 })]
    public void Permutations_Of2UniqueElements_Count(int[] source)
    {
        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(2, cardinality);

        int count = Permutations.Of(source).Count();
        Assert.AreEqual(cardinality, count);
    }

    [TestMethod]
    public void Permutations_Of2DuplicateElements()
    {
        var source = new[] { 1, 1 };
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(2, cardinality);

        var p = Permutations.Of(source).ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToMultiset(ArrayEqualityComparer<int>.Default);

        int distinctCardinality = cardinality / countOfDuplicateElements;
        Assert.HasCount(distinctCardinality, s);

        Assert.IsTrue(s.TryGetValue([1, 1], out var count) && count == 2);
    }

    [TestMethod]
    public void Permutations_Of2DuplicateElements_Distinct()
    {
        var source = new[] { 1, 1 };
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
        Assert.AreEqual(1, cardinality);

        var p = Permutations.Of(source).Distinct().ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 1], s);
    }

    [TestMethod]
    public void Permutations_Of2DuplicateElements_Distinct_Count()
    {
        var source = new[] { 1, 1 };
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
        Assert.AreEqual(1, cardinality);

        int count = Permutations.Of(source).Distinct().Count();
        Assert.AreEqual(cardinality, count);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 3 })]
    [DataRow(new[] { 1, 3, 2 })]
    [DataRow(new[] { 2, 1, 3 })]
    [DataRow(new[] { 2, 3, 1 })]
    [DataRow(new[] { 3, 1, 2 })]
    [DataRow(new[] { 3, 2, 1 })]
    public void Permutations_Of3UniqueElements(int[] source)
    {
        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(6, cardinality);

        var p = Permutations.Of(source).ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 2, 3], s);
        Assert.Contains([1, 3, 2], s);
        Assert.Contains([2, 1, 3], s);
        Assert.Contains([2, 3, 1], s);
        Assert.Contains([3, 1, 2], s);
        Assert.Contains([3, 2, 1], s);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 3 })]
    [DataRow(new[] { 1, 3, 2 })]
    [DataRow(new[] { 2, 1, 3 })]
    [DataRow(new[] { 2, 3, 1 })]
    [DataRow(new[] { 3, 1, 2 })]
    [DataRow(new[] { 3, 2, 1 })]
    public void Permutations_Of3UniqueElements_Distinct(int[] source)
    {
        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(6, cardinality);

        var p = Permutations.Of(source).Distinct().ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 2, 3], s);
        Assert.Contains([1, 3, 2], s);
        Assert.Contains([2, 1, 3], s);
        Assert.Contains([2, 3, 1], s);
        Assert.Contains([3, 1, 2], s);
        Assert.Contains([3, 2, 1], s);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 3 })]
    [DataRow(new[] { 1, 3, 2 })]
    [DataRow(new[] { 2, 1, 3 })]
    [DataRow(new[] { 2, 3, 1 })]
    [DataRow(new[] { 3, 1, 2 })]
    [DataRow(new[] { 3, 2, 1 })]
    public void Permutations_Of3UniqueElements_Count(int[] source)
    {
        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(6, cardinality);

        int count = Permutations.Of(source).Count();
        Assert.AreEqual(cardinality, count);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 2 })]
    [DataRow(new[] { 2, 1, 2 })]
    [DataRow(new[] { 2, 2, 1 })]
    public void Permutations_Of1UniqueAnd2DuplicateElements(int[] source)
    {
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length);
        Assert.AreEqual(6, cardinality);

        var p = Permutations.Of(source).ReifyList();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToMultiset(ArrayEqualityComparer<int>.Default);

        int distinctCardinality = cardinality / countOfDuplicateElements;
        Assert.HasCount(distinctCardinality, s);

        { Assert.IsTrue(s.TryGetValue([1, 2, 2], out var count) && count == 2); }
        { Assert.IsTrue(s.TryGetValue([2, 1, 2], out var count) && count == 2); }
        { Assert.IsTrue(s.TryGetValue([2, 2, 1], out var count) && count == 2); }
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 2 })]
    [DataRow(new[] { 2, 1, 2 })]
    [DataRow(new[] { 2, 2, 1 })]
    public void Permutations_Of1UniqueAnd2DuplicateElements_Distinct(int[] source)
    {
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
        Assert.AreEqual(3, cardinality);

        var p = Permutations.Of(source).Distinct().ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 2, 2], s);
        Assert.Contains([2, 1, 2], s);
        Assert.Contains([2, 2, 1], s);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 2 })]
    [DataRow(new[] { 2, 1, 2 })]
    [DataRow(new[] { 2, 2, 1 })]
    public void Permutations_Of1UniqueAnd2DuplicateElements_Distinct_Count(int[] source)
    {
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
        Assert.AreEqual(3, cardinality);

        int count = Permutations.Of(source).Distinct().Count();
        Assert.AreEqual(cardinality, count);
    }

    [TestMethod]
    [DataRow(new[] { 1, 2, 2 })]
    [DataRow(new[] { 2, 1, 2 })]
    [DataRow(new[] { 2, 2, 1 })]
    public void Permutations_Of1UniqueAnd2DuplicateElements_Distinct_NoAcceleration(int[] source)
    {
        const int countOfDuplicateElements = 2;

        int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
        Assert.AreEqual(3, cardinality);

        var p = Enumerable.Distinct(Permutations.Of(source)).ReifyCollection();
        Assert.HasCount(cardinality, p);

        var s = p
            .Select(x => x.ToArray())
            .ToHashSet(ArrayEqualityComparer<int>.Default);

        Assert.HasCount(cardinality, s);

        Assert.Contains([1, 2, 2], s);
        Assert.Contains([2, 1, 2], s);
        Assert.Contains([2, 2, 1], s);
    }
}
