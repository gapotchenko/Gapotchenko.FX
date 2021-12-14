using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void Graph_DeduplicateVertices()
        {
            var g = new Graph<int>
            {
                Vertices = { 1, 2, 3, 1 }
            };

            Assert.AreEqual(3, g.Vertices.Count);
        }

        [TestMethod]
        public void Graph_RemoveVertices()
        {
            var g = new Graph<int>
            {
                Vertices = { 1, 2, 3 }
            };

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
        public void Graph_ClearVertices()
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

            g.Vertices.Clear();
            Assert.AreEqual(0, g.Vertices.Count);
            Assert.AreEqual(0, g.Edges.Count);
        }

        [TestMethod]
        public void Graph_Clear()
        {
            var g = new Graph<int>()
            {
                Vertices = { 1, 2 },
                Edges = { (1, 2), (2, 3) }
            };

            Assert.AreEqual(3, g.Vertices.Count);
            Assert.AreEqual(2, g.Edges.Count);

            g.Clear();
            Assert.AreEqual(0, g.Vertices.Count);
            Assert.AreEqual(0, g.Edges.Count);
        }

        [TestMethod]
        public void Graph_DeduplicateEdges()
        {
            var g = new Graph<int>()
            {
                Edges = { (1, 2), (2, 3), (1, 2) }
            };

            Assert.AreEqual(3, g.Vertices.Count);
            Assert.AreEqual(2, g.Edges.Count);
        }

        [TestMethod]
        public void Graph_RemoveEdges()
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
        public void Graph_ClearEdges()
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

        [TestMethod]
        public void Graph_Transpose()
        {
            var g = new Graph<int>()
            {
                Edges = { (1, 2), (2, 3) }
            };

            g.Transpose();

            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.IsTrue(g.Edges.SetEquals(new[] { (2, 1), (3, 2) }));
        }

        [TestMethod]
        public void Graph_GetTransposition()
        {
            var g = new Graph<int>()
            {
                Edges = { (1, 2), (2, 3) }
            };

            var g0 = g.Clone();
            var h = g.GetTransposition();
            Assert.IsTrue(g.GraphEquals(g0), "Original graph instance should not be modified.");

            Assert.AreNotSame(g, h);
            Assert.IsTrue(h.Vertices.SetEquals(new[] { 1, 2, 3 }));
            Assert.IsTrue(h.Edges.SetEquals(new[] { (2, 1), (3, 2) }));
        }

        [TestMethod]
        public void Graph_Clone()
        {
            var g = new Graph<int>()
            {
                Vertices = { 5 },
                Edges = { (1, 2), (2, 3) }
            };

            var h = g.Clone();

            Assert.AreNotSame(g, h);
            Assert.IsTrue(h.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
            Assert.IsTrue(h.Edges.SetEquals(new[] { (1, 2), (2, 3) }));
        }

        [TestMethod]
        public void Graph_SupportsReflexes()
        {
            var g = new Graph<int>()
            {
                Vertices = { 5 },
                Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
            };

            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
            Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 1), (1, 2), (2, 3), (3, 3) }));
        }

        [TestMethod]
        public void Graph_ReduceReflexes()
        {
            var g = new Graph<int>()
            {
                Vertices = { 5 },
                Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
            };

            g.ReduceReflexes();

            Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
            Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2), (2, 3) }));
        }

        [TestMethod]
        public void Graph_GetReflexiveReduction()
        {
            var g = new Graph<int>()
            {
                Vertices = { 5 },
                Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
            };

            var g0 = g.Clone();
            var h = g.GetReflexiveReduction();
            Assert.IsTrue(g.GraphEquals(g0), "Original graph instance should not be modified.");

            Assert.AreNotSame(g, h);
            Assert.IsTrue(h.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
            Assert.IsTrue(h.Edges.SetEquals(new[] { (1, 2), (2, 3) }));
        }

        [TestMethod]
        public void Graph_ReduceTransitions()
        {
            var g = new Graph<char>()
            {
                Vertices = { 'f' },
                Edges =
                {
                    ('a', 'b'), ('a', 'd'), ('a', 'c'), ('a', 'e'),
                    ('b', 'd'),
                    ('c', 'd'), ('c', 'e'),
                    ('d', 'e')
                }
            };

            g.ReduceTransitions();

            Assert.IsTrue(g.Vertices.SetEquals("abcdef"));
            Assert.IsTrue(g.Edges.SetEquals(
                new[]
                {
                    ('a', 'b'), ('a', 'c'),
                    ('b', 'd'),
                    ('c', 'd'),
                    ('d', 'e'),
                }));
        }

        [TestMethod]
        public void Graph_GetTransitiveReduction()
        {
            var g = new Graph<char>()
            {
                Vertices = { 'f' },
                Edges =
                {
                    ('a', 'b'), ('a', 'd'), ('a', 'c'), ('a', 'e'),
                    ('b', 'd'),
                    ('c', 'd'), ('c', 'e'),
                    ('d', 'e')
                }
            };

            var g0 = g.Clone();
            var h = g.GetTransitiveReduction();
            Assert.IsTrue(g.GraphEquals(g0), "Original graph instance should not be modified.");

            Assert.IsTrue(h.Vertices.SetEquals("abcdef"));
            Assert.IsTrue(h.Edges.SetEquals(
                new[]
                {
                    ('a', 'b'), ('a', 'c'),
                    ('b', 'd'),
                    ('c', 'd'),
                    ('d', 'e'),
                }));
        }

        [TestMethod]
        public void Graph_GraphEquals()
        {
            var g = new Graph<int>()
            {
                Vertices = { 5 },
                Edges = { (1, 2), (2, 3) }
            };

            IReadOnlyGraph<int> g0 = g.Clone();
            Assert.IsTrue(g.GraphEquals(g0));

            Assert.IsTrue(g.Vertices.Remove(5));
            Assert.IsFalse(g.GraphEquals(g0));

            Assert.IsTrue(g.Vertices.Add(5));
            Assert.IsTrue(g.Vertices.Add(6));
            Assert.IsFalse(g.GraphEquals(g0));

            Assert.IsTrue(g.Vertices.Remove(6));
            Assert.IsTrue(g.GraphEquals(g0));

            Assert.IsTrue(g.Edges.Remove(2, 3));
            Assert.IsFalse(g.GraphEquals(g0));

            Assert.IsTrue(g.Edges.Add(2, 3));
            Assert.IsTrue(g.Edges.Add(3, 5));
            Assert.IsFalse(g.GraphEquals(g0));

            Assert.IsTrue(g.Edges.Remove(3, 5));
            Assert.IsTrue(g.GraphEquals(g0));

            Assert.IsTrue(g.Vertices.Remove(5));
            Assert.IsTrue(g.Edges.Remove(2, 3));
            Assert.IsFalse(g.GraphEquals(g0));

            Assert.IsTrue(g.Vertices.Add(5));
            Assert.IsTrue(g.Vertices.Add(6));
            Assert.IsTrue(g.Edges.Add(2, 3));
            Assert.IsTrue(g.Edges.Add(3, 5));
            Assert.IsFalse(g.GraphEquals(g0));

            Assert.IsTrue(g.Vertices.Remove(6));
            Assert.IsTrue(g.Edges.Remove(3, 5));
            Assert.IsTrue(g.GraphEquals(g0));
        }
    }
}
