using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics.Tests
{
    [TestClass]
    public class PermutationTests
    {
        [TestMethod]
        public void Permutations_Of2Elements()
        {
            var source = new[] { 1, 2 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(2, cardinality);

            var p = Permutations.Generate(source).AsReadOnly();
            Assert.AreEqual(cardinality, p.Count);

            var s = p
                .Select(x => x.ToArray())
                .ToHashSet(ArrayEqualityComparer<int>.Default);

            Assert.AreEqual(cardinality, s.Count);

            Assert.IsTrue(s.Contains(new[] { 1, 2 }));
            Assert.IsTrue(s.Contains(new[] { 2, 1 }));
        }

        [TestMethod]
        public void Permutations_Of3Elements()
        {
            var source = new[] { 1, 2, 3 };

            int cardinality = Permutations.Cardinality(source.Length);
            Assert.AreEqual(6, cardinality);

            var p = Permutations.Generate(source).AsReadOnly();
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
    }
}
