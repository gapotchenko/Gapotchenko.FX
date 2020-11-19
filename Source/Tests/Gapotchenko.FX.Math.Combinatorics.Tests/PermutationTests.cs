using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics.Tests
{
    [TestClass]
    public class PermutationTests
    {
        [TestMethod]
        public void Permutations_Of2UniqueElements()
        {
            var source = new[] { 1, 2 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(2, cardinality);

            var p = Permutations.Of(source).AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 1 }));
        }

        [TestMethod]
        public void Permutations_Of2UniqueElements_Distinct()
        {
            var source = new[] { 1, 2 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(2, cardinality);

            var p = Permutations.Of(source).Distinct().AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 1 }));
        }

        [TestMethod]
        public void Permutations_Of2UniqueElements_Count()
        {
            var source = new[] { 1, 2 };

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

            int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
            Assert.AreEqual(1, cardinality);

            var p = Permutations.Of(source).Distinct().AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 1 }));
        }

        [TestMethod]
        public void Permutations_Of2DuplicateElements_Count()
        {
            var source = new[] { 1, 1 };
            const int countOfDuplicateElements = 2;

            int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
            Assert.AreEqual(1, cardinality);

            int count = Permutations.Of(source).Distinct().Count();
            Assert.AreEqual(cardinality, count);
        }

        [TestMethod]
        public void Permutations_Of3UniqueElements()
        {
            var source = new[] { 1, 2, 3 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(6, cardinality);

            var p = Permutations.Of(source).AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 2, 3 }));
            Assert.IsTrue(s.Contains(new[] { 1, 3, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 1, 3 }));
            Assert.IsTrue(s.Contains(new[] { 2, 3, 1 }));
            Assert.IsTrue(s.Contains(new[] { 3, 1, 2 }));
            Assert.IsTrue(s.Contains(new[] { 3, 2, 1 }));
        }

        [TestMethod]
        public void Permutations_Of3UniqueElements_Distinct()
        {
            var source = new[] { 1, 2, 3 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(6, cardinality);

            var p = Permutations.Of(source).Distinct().AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 2, 3 }));
            Assert.IsTrue(s.Contains(new[] { 1, 3, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 1, 3 }));
            Assert.IsTrue(s.Contains(new[] { 2, 3, 1 }));
            Assert.IsTrue(s.Contains(new[] { 3, 1, 2 }));
            Assert.IsTrue(s.Contains(new[] { 3, 2, 1 }));
        }

        [TestMethod]
        public void Permutations_Of3UniqueElements_Count()
        {
            var source = new[] { 1, 2, 3 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(6, cardinality);

            int count = Permutations.Of(source).Count();
            Assert.AreEqual(cardinality, count);
        }

        [TestMethod]
        public void Permutations_Of1UniqueAnd2DuplicateElements()
        {
            var source = new[] { 1, 2, 2 };
            const int countOfDuplicateElements = 2;

            int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
            Assert.AreEqual(3, cardinality);

            var p = Permutations.Of(source).Distinct().AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 2, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 1, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 2, 1 }));
        }

        [TestMethod]
        public void Permutations_Of1UniqueAnd2DuplicateElements_Count()
        {
            var source = new[] { 1, 2, 2 };
            const int countOfDuplicateElements = 2;

            int cardinality = Permutations.Cardinality(source.Length) / countOfDuplicateElements;
            Assert.AreEqual(3, cardinality);

            int count = Permutations.Of(source).Distinct().Count();
            Assert.AreEqual(cardinality, count);
        }
    }
}
