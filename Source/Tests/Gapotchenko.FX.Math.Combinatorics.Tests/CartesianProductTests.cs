using System.Linq;
using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Combinatorics.Tests
{
    [TestClass]
    public class CartesianProductTests
    {
        [TestMethod]
        public void CartesianProduct_Of2x3Elements()
        {
            var factors = new[]
            {
                new[] { 1, 2 },
                new[] { 5, 6, 7 }
            };

            int cardinality = CartesianProduct.Cardinality(factors.Select(x => x.Length));
            Assert.AreEqual(6, cardinality);

            var p = CartesianProduct.Generate(factors).AsReadOnly();

            Assert.AreEqual(cardinality, p.Count);

            Assert.IsTrue(p[0].SequenceEqual(new[] { 1, 5 }));
            Assert.IsTrue(p[1].SequenceEqual(new[] { 2, 5 }));
            Assert.IsTrue(p[2].SequenceEqual(new[] { 1, 6 }));
            Assert.IsTrue(p[3].SequenceEqual(new[] { 2, 6 }));
            Assert.IsTrue(p[4].SequenceEqual(new[] { 1, 7 }));
            Assert.IsTrue(p[5].SequenceEqual(new[] { 2, 7 }));
        }

        [TestMethod]
        public void CartesianProduct_Of2x0Elements()
        {
            var factors = new[]
            {
                new[] { 1, 2 },
                new int[] { }
            };

            int cardinality = CartesianProduct.Cardinality(factors.Select(x => x.Length));
            Assert.AreEqual(0, cardinality);

            var p = CartesianProduct.Generate(factors);
            Assert.AreEqual(cardinality, p.Count());
        }
    }
}
