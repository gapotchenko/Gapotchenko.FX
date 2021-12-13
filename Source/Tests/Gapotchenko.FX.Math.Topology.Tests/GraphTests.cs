using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void Graph_Test1()
        {
            var g = new Graph<int>();
            Assert.AreEqual(0, g.Vertices.Count);
            Assert.AreEqual(0, g.Edges.Count);

            g.Vertices.Add(1);
            g.Vertices.Add(2);
            g.Vertices.Add(3);

            Assert.AreEqual(3, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));

            Assert.IsFalse(g.Vertices.Remove(4));
            Assert.AreEqual(3, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(0, g.Edges.Count);

            Assert.IsTrue(g.Vertices.Remove(3));
            Assert.AreEqual(2, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2 }));
            Assert.AreEqual(0, g.Edges.Count);
        }

        [TestMethod]
        public void Graph_Test2()
        {
            var g = new Graph<int>()
            {
                Vertices = { 1, 2 },
                Edges = { (1, 2), (2, 3) }
            };

            Assert.AreEqual(3, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(2, g.Edges.Count);
            Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2), (2, 3) }));

            Assert.IsFalse(g.Edges.Remove(1, 3));
            Assert.AreEqual(3, g.Vertices.Count);
            Assert.AreEqual(2, g.Edges.Count);

            Assert.IsTrue(g.Edges.Remove(2, 3));
            Assert.AreEqual(3, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(1, g.Edges.Count);
            Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2) }));
        }

        [TestMethod]
        public void Graph_Test3()
        {
            var g = new Graph<int>()
            {
                Vertices = { 1, 2 },
                Edges = { (1, 2), (2, 3) }
            };

            Assert.AreEqual(3, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(2, g.Edges.Count);
            Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2), (2, 3) }));

            g.Edges.Clear();
            Assert.AreEqual(3, g.Vertices.Count);
            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(0, g.Edges.Count);
        }

    }
}
