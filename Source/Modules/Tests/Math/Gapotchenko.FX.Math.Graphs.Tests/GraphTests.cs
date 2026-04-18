using Gapotchenko.FX.Math.Graphs.Tests.Engine;

namespace Gapotchenko.FX.Math.Graphs.Tests;

[TestClass]
public partial class GraphTests
{
    // This type is partial.
    // For the rest of the implementation, please take a look at the neighboring source files.

    [TestMethod]
    public void Graph_Vertices_Uniqueness()
    {
        var g = new Graph<int>
        {
            Vertices = { 1, 2, 3, 1 }
        };

        Assert.AreEqual(3, g.Vertices.Count);
    }

    [TestMethod]
    public void Graph_Vertices_Remove_1()
    {
        var g = new Graph<int>
        {
            Vertices = { 1, 2, 3 }
        };

        Assert.AreEqual(3, g.Vertices.Count);
        Assert.IsTrue(g.Vertices.SetEquals([1, 2, 3]));

        Assert.IsFalse(g.Vertices.Remove(4));
        Assert.AreEqual(3, g.Vertices.Count);
        Assert.IsTrue(g.Vertices.SetEquals([1, 2, 3]));
        Assert.AreEqual(0, g.Edges.Count);

        Assert.IsTrue(g.Vertices.Remove(3));
        Assert.AreEqual(2, g.Vertices.Count);
        Assert.IsTrue(g.Vertices.SetEquals([1, 2]));
        Assert.AreEqual(0, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Vertices_Remove_2()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        Assert.AreEqual(2, g.Edges.Count);

        g.Vertices.Remove(2);

        Assert.AreEqual(0, g.Edges.Count);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_Vertices_Remove_NeighborPreservation(bool directed)
    {
        // Bug: removing the source vertex of an edge dropped the destination
        // vertex when it had no key entry of its own in the adjacency list
        // (i.e., it never appeared as an edge source).
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (1, 2) }
        };

        g.Vertices.Remove(1);

        Assert.IsTrue(g.Vertices.Contains(2), "Neighbor vertex must survive removal of its only incident vertex.");
        Assert.AreEqual(1, g.Vertices.Count);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_Vertices_Remove_NeighborPreservation_ReverseAdjacency(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (1, 2) }
        };

        // Materialize the reverse adjacency list.
        _ = g.IncomingVerticesAdjacentTo(2).ToList();

        g.Vertices.Remove(1);

        Assert.IsTrue(g.Vertices.SetEquals([2]));
    }

    [TestMethod]
    public void Graph_Vertices_Clear()
    {
        var g = new Graph<int>
        {
            Vertices = { 1, 2 },
            Edges = { (1, 2), (2, 3) }
        };

        Assert.AreEqual(3, g.Vertices.Count);
        Assert.IsTrue(g.Vertices.SetEquals([1, 2, 3]));
        Assert.AreEqual(2, g.Edges.Count);
        Assert.IsTrue(g.Edges.SetEquals([(1, 2), (2, 3)]));

        g.Vertices.Clear();
        Assert.AreEqual(0, g.Vertices.Count);
        Assert.AreEqual(0, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Clear()
    {
        var g = new Graph<int>
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
    public void Graph_Transpose()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        var g0 = g.Clone();
        g.Transpose();
        Assert.IsFalse(g.GraphEquals(g0), "Original graph instance should be modified.");

        Assert.IsTrue(g.Vertices.SetEquals([1, 2, 3]));
        Assert.IsTrue(g.Edges.SetEquals([(2, 1), (3, 2)]));
    }

    [TestMethod]
    public void Graph_Transpose_InvalidatesEnumeration()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        using var e = g.Edges.GetEnumerator();
        e.MoveNext();

        g.Transpose();

        Assert.ThrowsExactly<InvalidOperationException>(() => e.MoveNext());
    }

    [TestMethod]
    public void Graph_GetTransposition()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        var g0 = g.Clone();
        var h = g.GetTransposition();
        Assert.IsTrue(g.GraphEquals(g0), "Original graph instance should not be modified.");

        Assert.AreNotSame(g, h);
        Assert.IsTrue(h.Vertices.SetEquals([1, 2, 3]));
        Assert.IsTrue(h.Edges.SetEquals([(2, 1), (3, 2)]));
    }

    [TestMethod]
    public void Graph_IsVertexIsolated()
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (4, 4) }
        };

        Assert.IsFalse(g.IsVertexIsolated(1));
        Assert.IsFalse(g.IsVertexIsolated(2));
        Assert.IsFalse(g.IsVertexIsolated(3));
        Assert.IsFalse(g.IsVertexIsolated(4));
        Assert.IsTrue(g.IsVertexIsolated(5));

        Assert.IsTrue(g.IsVertexIsolated(10));

        /***************/

        g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        Assert.IsFalse(g.IsVertexIsolated(1));
        Assert.IsFalse(g.IsVertexIsolated(2));
        Assert.IsFalse(g.IsVertexIsolated(3));
    }

    [TestMethod]
    public void Graph_IsolatedVertices()
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (4, 4) }
        };

        Assert.IsTrue(g.IsolatedVertices.ToHashSet().SetEquals([5]));

        /***************/

        g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        Assert.AreEqual(0, g.IsolatedVertices.Count());
    }

    [TestMethod]
    public void Graph_Reflexes_Support()
    {
        var g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
        };

        Assert.IsTrue(g.Vertices.SetEquals([1, 2, 3, 5]));
        Assert.IsTrue(g.Edges.SetEquals([(1, 1), (1, 2), (2, 3), (3, 3)]));
    }

    [TestMethod]
    public void Graph_Reflexes_Reduce()
    {
        var g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
        };

        g.ReduceReflexes();

        Assert.IsTrue(g.Vertices.SetEquals([1, 2, 3, 5]));
        Assert.IsTrue(g.Edges.SetEquals([(1, 2), (2, 3)]));
    }

    [TestMethod]
    public void Graph_Reflexes_GetReduction()
    {
        var g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
        };

        var g0 = g.Clone();
        var h = g.GetReflexiveReduction();
        Assert.IsTrue(g.GraphEquals(g0), "Original graph instance should not be modified.");

        Assert.AreNotSame(g, h);
        Assert.IsTrue(h.Vertices.SetEquals([1, 2, 3, 5]));
        Assert.IsTrue(h.Edges.SetEquals([(1, 2), (2, 3)]));
    }

    [TestMethod]
    public void Graph_Transitions_Reduce()
    {
        var g = new Graph<char>
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
    public void Graph_Transitions_GetReduction()
    {
        var g = new Graph<char>
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
        var g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3) }
        };

        Assert.IsTrue(g.GraphEquals(g));

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

    [TestMethod]
    public void Graph_IsProperSubgraphOf()
    {
        var g = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsFalse(g.IsProperSubgraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsFalse(h.IsProperSubgraphOf(g));

        h = new Graph<char>
        {
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(h.IsProperSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'w' },
        };

        Assert.IsTrue(h.IsProperSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v' },
        };

        Assert.IsTrue(h.IsProperSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'x' },
        };

        Assert.IsFalse(h.IsProperSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'w', 'x' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsProperSubgraphOf(h));
    }

    [TestMethod]
    public void Graph_IsProperSupergraphOf()
    {
        var g = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsFalse(g.IsProperSupergraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsFalse(g.IsProperSupergraphOf(h));

        h = new Graph<char>
        {
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsProperSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'w' },
        };

        Assert.IsTrue(g.IsProperSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v' },
        };

        Assert.IsTrue(g.IsProperSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'x' },
        };

        Assert.IsFalse(g.IsProperSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'w', 'x' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(h.IsProperSupergraphOf(g));
    }

    [TestMethod]
    public void Graph_IsSupergraphOf()
    {
        var g = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsSupergraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsSupergraphOf(h));

        h = new Graph<char>
        {
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'w' },
        };

        Assert.IsTrue(g.IsSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v' },
        };

        Assert.IsTrue(g.IsSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'x' },
        };

        Assert.IsFalse(g.IsSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'w', 'x' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(h.IsSupergraphOf(g));
    }

    [TestMethod]
    public void Graph_IsSubgraphOf()
    {
        var g = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsSubgraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'w' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(h.IsSubgraphOf(g));

        h = new Graph<char>
        {
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(h.IsSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'w' },
        };

        Assert.IsTrue(h.IsSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v' },
        };

        Assert.IsTrue(h.IsSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'u', 'v', 'x' },
        };

        Assert.IsFalse(h.IsSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'w', 'x' },
            Edges = { ('u', 'v') }
        };

        Assert.IsTrue(g.IsSubgraphOf(h));
    }

    [TestMethod]
    public void Graph_IsVertexInducedSubgraphOf()
    {
        var g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'c'), ('b', 'f'), ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };

        Assert.IsTrue(g.IsVertexInducedSubgraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'b' },
            Edges = { ('a', 'e') }
        };

        Assert.IsFalse(h.IsVertexInducedSubgraphOf(g));

        h = new Graph<char>
        {
            Edges = { ('a', 'e'), ('a', 'b') }
        };

        Assert.IsTrue(h.IsVertexInducedSubgraphOf(g));

        h = new Graph<char>
        {
            Edges = { ('d', 'e'), ('e', 'f'), ('c', 'e') }
        };

        Assert.IsTrue(h.IsVertexInducedSubgraphOf(g));
    }

    [TestMethod]
    public void Graph_IsVertexInducedSupergraphOf()
    {
        var g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'c'), ('b', 'f'), ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };

        Assert.IsTrue(g.IsVertexInducedSupergraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'b' },
            Edges = { ('a', 'e') }
        };

        Assert.IsFalse(g.IsVertexInducedSupergraphOf(h));

        h = new Graph<char>
        {
            Edges = { ('a', 'e'), ('a', 'b') }
        };

        Assert.IsTrue(g.IsVertexInducedSupergraphOf(h));

        h = new Graph<char>
        {
            Edges = { ('d', 'e'), ('e', 'f'), ('c', 'e') }
        };

        Assert.IsTrue(g.IsVertexInducedSupergraphOf(h));
    }

    [TestMethod]
    public void Graph_IsEdgeInducedSubgraphOf()
    {
        var g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'c'), ('b', 'f'), ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };

        Assert.IsTrue(g.IsEdgeInducedSubgraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'f' },
            Edges = { ('a', 'b'), ('a', 'e') }
        };

        Assert.IsFalse(h.IsEdgeInducedSubgraphOf(g));

        h = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'f') }
        };

        Assert.IsTrue(h.IsEdgeInducedSubgraphOf(g));

        h = new Graph<char>
        {
            Vertices = { 'd' },
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'f') }
        };

        Assert.IsFalse(h.IsEdgeInducedSubgraphOf(g));

        h = new Graph<char>
        {
            Edges = { ('d', 'e'), ('b', 'f'), ('a', 'b'), ('b', 'c') }
        };

        Assert.IsTrue(h.IsEdgeInducedSubgraphOf(g));

        h = new Graph<char>();

        Assert.IsTrue(h.IsEdgeInducedSubgraphOf(g));
    }

    [TestMethod]
    public void Graph_IsEdgeInducedSupergraphOf()
    {
        var g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'c'), ('b', 'f'), ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };

        Assert.IsTrue(g.IsEdgeInducedSupergraphOf(g));

        IReadOnlyGraph<char> h = new Graph<char>
        {
            Vertices = { 'f' },
            Edges = { ('a', 'b'), ('a', 'e') }
        };

        Assert.IsFalse(g.IsEdgeInducedSupergraphOf(h));

        h = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'f') }
        };

        Assert.IsTrue(g.IsEdgeInducedSupergraphOf(h));

        h = new Graph<char>
        {
            Vertices = { 'd' },
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'f') }
        };

        Assert.IsFalse(g.IsEdgeInducedSupergraphOf(h));

        h = new Graph<char>
        {
            Edges = { ('d', 'e'), ('b', 'f'), ('a', 'b'), ('b', 'c') }
        };

        Assert.IsTrue(g.IsEdgeInducedSupergraphOf(h));

        h = new Graph<char>();

        Assert.IsTrue(g.IsEdgeInducedSupergraphOf(h));
    }

    [TestMethod]
    public void Graph_GetSubgraph_VertexInduced()
    {
        var g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'c'), ('b', 'f'), ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };

        TestCore(g, g.Vertices, g);

        var expectedSubgraph = new Graph<char>
        {
            Vertices = { 'b' }
        };
        TestCore(g, ['b'], expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e') }
        };
        TestCore(g, ['a', 'b', 'e'], expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Edges = { ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };
        TestCore(g, ['c', 'd', 'e', 'f'], expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Vertices = { 'b' }
        };
        TestCore(g, ['x', 'b'], expectedSubgraph);

        TestCore(g, [], new Graph<char>());

        static void TestCore<T>(Graph<T> g, IEnumerable<T> subgraphVertices, IReadOnlyGraph<T> expectedSubgraph)
        {
            var actualSubgraph = g.GetSubgraph(subgraphVertices);
            Assert.IsTrue(expectedSubgraph.GraphEquals(actualSubgraph));
            Assert.IsTrue(expectedSubgraph.IsVertexInducedSupergraphOf(actualSubgraph));

            var actualSubgraphInline = g.Clone();
            actualSubgraphInline.Subgraph(subgraphVertices);
            Assert.IsTrue(actualSubgraph.GraphEquals(actualSubgraphInline));
        }
    }

    [TestMethod]
    public void Graph_GetSubgraph_EdgeInduced()
    {
        var g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e'), ('b', 'c'), ('b', 'f'), ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };

        TestCore(g, g.Edges, g);

        var expectedSubgraph = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e') }
        };
        TestCore(g, [('a', 'b'), ('a', 'e')], expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        TestCore(g, [('a', 'b'), ('e', 'a')], expectedSubgraph);

        TestCore(g, [('e', 'a')], new Graph<char>());

        TestCore(g, [], new Graph<char>());

        static void TestCore<T>(Graph<T> g, IEnumerable<GraphEdge<T>> subgraphEdges, IReadOnlyGraph<T> expectedSubgraph)
        {
            var actualSubgraph = g.GetSubgraph(subgraphEdges);
            Assert.IsTrue(expectedSubgraph.GraphEquals(actualSubgraph));
            Assert.IsTrue(g.IsEdgeInducedSupergraphOf(actualSubgraph));

            var actualSubgraphInline = g.Clone();
            actualSubgraphInline.Subgraph(subgraphEdges);
            Assert.IsTrue(actualSubgraph.GraphEquals(actualSubgraphInline));
        }
    }

    [TestMethod]
    public void Graph_ConnectedComponents()
    {
        var g = new Graph<int>
        {
            Edges =
            {
                // Subgraph 1
                (1, 7), (9, 7),
                (7, 2), (7, 4), (7, 8), (7, 10),
                (2, 4),

                // Subgraph 2
                (5, 11),

                // Subgraph 3
                (6, 3), (6, 12)
            }
        };

        var connectedComponents = g.ConnectedComponents.ToHashSet(GraphEqualityComparer<int>.Default);
        Assert.HasCount(3, connectedComponents);

        Assert.Contains(
            new Graph<int>
            {
                Edges =
                {
                    (1, 7), (9, 7),
                    (7, 2), (7, 4), (7, 8), (7, 10),
                    (2, 4),
                }
            },
            connectedComponents,
            "Subgraph 1 is missing.");

        Assert.Contains(
            new Graph<int>
            {
                Edges =
                {
                    (5, 11)
                }
            },
            connectedComponents,
            "Subgraph 2 is missing.");

        Assert.Contains(
            new Graph<int>
            {
                Edges =
                {
                    (6, 3), (6, 12)
                }
            },
            connectedComponents,
            "Subgraph 3 is missing.");
    }
}
