using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class UndirectedGraphTests
    {
        [TestMethod]
        public void UndirectedGraph_Edges_Uniqueness()
        {
            var g = new Graph<int>
            {
                Edges = { (1, 2), (2, 1) },
                IsDirected = false
            };

            Assert.AreEqual(2, g.Vertices.Count);
            Assert.AreEqual(1, g.Edges.Count);
        }

        [TestMethod]
        public void UndirectedGraph_Edges_Reversibility()
        {
            var g = new Graph<int>
            {
                IsDirected = false
            };

            Assert.IsTrue(g.Edges.Add(1, 2));
            Assert.AreEqual(1, g.Edges.Count);

            Assert.IsTrue(g.Edges.Contains(1, 2));
            Assert.IsTrue(g.Edges.Contains(2, 1));

            Assert.IsFalse(g.Edges.Add(2, 1));
            Assert.AreEqual(1, g.Edges.Count);
        }

        [TestMethod]
        public void UndirectedGraph_Clone()
        {
            var g = new Graph<int>
            {
                IsDirected = false,
                Vertices = { 5 },
                Edges = { (1, 2), (2, 3) }
            };

            var clone = g.Clone();

            Assert.IsFalse(clone.IsDirected);
            Assert.AreEqual(4, clone.Vertices.Count);
            Assert.AreEqual(2, clone.Edges.Count);

            Assert.IsTrue(clone.GraphEquals(g));
        }
    }
}
