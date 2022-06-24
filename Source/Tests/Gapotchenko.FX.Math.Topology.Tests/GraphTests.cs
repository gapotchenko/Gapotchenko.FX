using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Math.Topology.Tests.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology.Tests;

[TestClass]
public class GraphTests
{
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
    public void Graph_Vertices_Clear()
    {
        var g = new Graph<int>
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
    public void Graph_Edges_Uniqueness()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3), (1, 2) }
        };

        Assert.AreEqual(3, g.Vertices.Count);
        Assert.AreEqual(2, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Edges_Reversibility()
    {
        var g = new Graph<int>();

        Assert.IsTrue(g.Edges.Add(1, 2));
        Assert.AreEqual(1, g.Edges.Count);

        Assert.IsTrue(g.Edges.Contains(1, 2));
        Assert.IsFalse(g.Edges.Contains(2, 1));

        Assert.IsTrue(g.Edges.Add(2, 1));
        Assert.AreEqual(2, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Edges_Remove()
    {
        var g = new Graph<int>
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
    public void Graph_Edges_Clear()
    {
        var g = new Graph<int>
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
        var g = new Graph<int>
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
        var g = new Graph<int>
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
        var g = new Graph<int>
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
    public void Graph_HasPath()
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (4, 4) }
        };

        Assert.IsTrue(g.HasPath(1, 2));
        Assert.IsFalse(g.HasPath(2, 1));

        Assert.IsTrue(g.HasPath(1, 3));
        Assert.IsFalse(g.HasPath(3, 1));

        Assert.IsTrue(g.HasPath(4, 4));

        Assert.IsFalse(g.HasPath(1, 5));
        Assert.IsFalse(g.HasPath(5, 1));

        Assert.IsFalse(g.HasPath(1, 10));
        Assert.IsFalse(g.HasPath(10, 1));

        Assert.IsFalse(g.HasPath(10, 20));
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

        Assert.IsTrue(g.IsolatedVertices.ToHashSet().SetEquals(new[] { 5 }));

        /***************/

        g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        Assert.AreEqual(0, g.IsolatedVertices.Count());
    }

    [TestMethod]
    public void Graph_IsCyclic()
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3) }
        };

        Assert.IsFalse(g.IsCyclic);

        g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3), (3, 1) }
        };

        Assert.IsTrue(g.IsCyclic);

        g = new Graph<int>
        {
            Edges = { (1, 1) }
        };

        Assert.IsTrue(g.IsCyclic);

        g = new Graph<int>
        {
            Vertices = { 1 }
        };

        Assert.IsFalse(g.IsCyclic);

        g = new Graph<int>();

        Assert.IsFalse(g.IsCyclic);
    }

    [TestMethod]
    public void Graph_IsCyclic_Cache()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        Assert.IsFalse(g.IsCyclic);
        Assert.IsFalse(g.IsCyclic);

        g.Edges.Add(3, 1);

        Assert.IsTrue(g.IsCyclic);
        Assert.IsTrue(g.IsCyclic);

        g.Edges.Remove(3, 1);

        Assert.IsFalse(g.IsCyclic);
        Assert.IsFalse(g.IsCyclic);

        var h = g.Clone();
        Assert.IsFalse(h.IsCyclic);
        h.Edges.Add(3, 1);
        Assert.IsTrue(h.IsCyclic);
        h.Edges.Clear();
        Assert.IsFalse(h.IsCyclic);
    }

    [TestMethod]
    public void Graph_Reflexes_Support()
    {
        var g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 1), (1, 2), (2, 3), (3, 3) }
        };

        Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
        Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 1), (1, 2), (2, 3), (3, 3) }));
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

        Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
        Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2), (2, 3) }));
    }

    [TestMethod]
    public void Graph_Relexes_GetReduction()
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
        Assert.IsTrue(h.Vertices.SetEquals(new[] { 1, 2, 3, 5 }));
        Assert.IsTrue(h.Edges.SetEquals(new[] { (1, 2), (2, 3) }));
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
        TestCore(g, new[] { 'b' }, expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'e') }
        };
        TestCore(g, new[] { 'a', 'b', 'e' }, expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Edges = { ('c', 'e'), ('d', 'e'), ('e', 'f') }
        };
        TestCore(g, new[] { 'c', 'd', 'e', 'f' }, expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Vertices = { 'b' }
        };
        TestCore(g, new[] { 'x', 'b' }, expectedSubgraph);

        TestCore(g, new char[] { }, new Graph<char>());

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
        TestCore(g, new GraphEdge<char>[] { ('a', 'b'), ('a', 'e') }, expectedSubgraph);

        expectedSubgraph = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        TestCore(g, new GraphEdge<char>[] { ('a', 'b'), ('e', 'a') }, expectedSubgraph);

        TestCore(g, new GraphEdge<char>[] { ('e', 'a') }, new Graph<char>());

        TestCore(g, new GraphEdge<char>[] { }, new Graph<char>());

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
    public void Graph_GetVertexDegree()
    {
        var g = new Graph<char>();
        Assert.AreEqual(0, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };
        Assert.AreEqual(0, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };
        Assert.AreEqual(0, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        Assert.AreEqual(1, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };
        Assert.AreEqual(2, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'c') }
        };
        Assert.AreEqual(2, g.GetVertexDegree('a'));
        Assert.AreEqual(1, g.GetVertexDegree('b'));
        Assert.AreEqual(1, g.GetVertexDegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a') }
        };
        Assert.AreEqual(2, g.GetVertexDegree('a'));
        Assert.AreEqual(1, g.GetVertexDegree('b'));
        Assert.AreEqual(1, g.GetVertexDegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a'), ('a', 'a') }
        };
        Assert.AreEqual(4, g.GetVertexDegree('a'));
        Assert.AreEqual(1, g.GetVertexDegree('b'));
        Assert.AreEqual(1, g.GetVertexDegree('c'));
    }

    [TestMethod]
    public void Graph_GetVertexIndegree()
    {
        var g = new Graph<char>();
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'c') }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));
        Assert.AreEqual(1, g.GetVertexIndegree('b'));
        Assert.AreEqual(1, g.GetVertexIndegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a') }
        };
        Assert.AreEqual(2, g.GetVertexIndegree('a'));
        Assert.AreEqual(0, g.GetVertexIndegree('b'));
        Assert.AreEqual(0, g.GetVertexIndegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a'), ('a', 'a') }
        };
        Assert.AreEqual(3, g.GetVertexIndegree('a'));
        Assert.AreEqual(0, g.GetVertexIndegree('b'));
        Assert.AreEqual(0, g.GetVertexIndegree('c'));
    }

    [TestMethod]
    public void Graph_GetVertexOutdegree()
    {
        var g = new Graph<char>();
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        Assert.AreEqual(1, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a') }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'c') }
        };
        Assert.AreEqual(2, g.GetVertexOutdegree('a'));
        Assert.AreEqual(0, g.GetVertexOutdegree('b'));
        Assert.AreEqual(0, g.GetVertexOutdegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a') }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));
        Assert.AreEqual(1, g.GetVertexOutdegree('b'));
        Assert.AreEqual(1, g.GetVertexOutdegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a'), ('a', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexOutdegree('a'));
        Assert.AreEqual(1, g.GetVertexOutdegree('b'));
        Assert.AreEqual(1, g.GetVertexOutdegree('c'));
    }

    [TestMethod]
    public void Graph_OrderTopologically()
    {
        var g = new Graph<char>();
        IEnumerable<char> order = g.OrderTopologically();
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologically();
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        foreach (var edges in
            Permutations.Of(new GraphEdge<char>[] { ('0', '1'), ('1', '2'), ('2', '3') }))
        {
            g = new();
            g.Edges.UnionWith(edges);

            var topoOrder = g.OrderTopologically();
            Assert.AreEqual("0 1 2 3", string.Join(" ", topoOrder));
        }

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologically().ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologically().ToList());
    }

    [TestMethod]
    public void Graph_OrderTopologically_Subsequent()
    {
        var g = new Graph<char>();

        IEnumerable<char> order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b", string.Join(" ", order));

        order = g.OrderTopologically().ThenBy(x => -x);
        Assert.AreEqual("b a", string.Join(" ", order));

        var customComparer = Comparer<char>.Create((x, y) => x.CompareTo(y));
        order = g.OrderTopologically().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("a b", string.Join(" ", order));

        customComparer = Comparer<char>.Create((x, y) => -x.CompareTo(y));
        order = g.OrderTopologically().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("b a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('1', '0'), ('2', '0'), ('3', '0') }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("1 2 3 0", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('4', '0'), ('4', '1'), ('1', '3'), ('2', '3'),
                ('5', '0'), ('5', '2')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("4 1 5 0 2 3", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('a', 'b'), ('b', 'c'), ('c', 'd'), ('d', 'Z'),
                ('Z', 'y'), ('d', 'z'), ('d', 'y'), ('d', 'e'),
                ('d', 'f')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b c d Z e f y z", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { '2' },
            Edges =
            {
                ('0', '1'), ('0', '4'), ('1', '3'), ('1', '5'),
                ('3', '5'), ('3', '7'), ('5', '6'), ('6', '7')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("0 1 2 3 4 5 6 7", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('1', '2'), ('2', '3'), ('2', '4'), ('2', '5'),
                ('2', '6'), ('3', '7'), ('3', '8'), ('4', '9'),
                ('6', '9'), ('7', 'a'), ('7', '9'), ('8', 'a'),
                ('8', '9')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("1 2 3 4 5 6 7 8 9 a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologically().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologically().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b', 'c', 'd', 'e' },
            Edges = { ('b', 'd') }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().ThenByDescending(Fn.Identity);
        Assert.AreEqual("e c b d a", string.Join(" ", order));

        /***************/

        var vertices = new[] { 'a', 'b', 'c', 'd', 'e' };
        g = new Graph<char>();
        g.Vertices.UnionWith(vertices);

        order = g.OrderTopologically().ThenBy(x => Array.IndexOf(vertices, x));
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().ThenBy(x => -Array.IndexOf(vertices, x));
        Assert.AreEqual("e d c b a", string.Join(" ", order));

        /***************/

        var g_001 = new Graph<(int a, int b)>
        {
            Vertices =
            {
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 0), (2, 1), (2, 2), (2, 3),
            },
            Edges = { ((1, 0), (2, 0)) }
        };

        var order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("10 11 12 13 20 21 22 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("21 22 23 10 20 11 12 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("13 12 11 10 23 22 21 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("23 22 21 13 12 11 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("10 20 11 21 12 22 13 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("13 23 12 22 11 21 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("10 20 21 11 22 12 23 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("23 13 22 12 21 11 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));
    }

    [TestMethod]
    public void Graph_OrderTopologicallyInReverse()
    {
        var g = new Graph<char>();
        IEnumerable<char> order = g.OrderTopologicallyInReverse();
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologicallyInReverse();
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        foreach (var edges in
            Permutations.Of(new GraphEdge<char>[] { ('0', '1'), ('1', '2'), ('2', '3') }))
        {
            g = new();
            g.Edges.UnionWith(edges);

            var topoOrder = g.OrderTopologicallyInReverse();
            Assert.AreEqual("3 2 1 0", string.Join(" ", topoOrder));
        }

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologicallyInReverse().ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologicallyInReverse().ToList());
    }

    [TestMethod]
    public void Graph_OrderTopologicallyInReverse_Subsequent()
    {
        var g = new Graph<char>();

        IEnumerable<char> order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("a b", string.Join(" ", order));

        order = g.OrderTopologicallyInReverse().ThenBy(x => -x);
        Assert.AreEqual("b a", string.Join(" ", order));

        var customComparer = Comparer<char>.Create((x, y) => x.CompareTo(y));
        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("a b", string.Join(" ", order));

        customComparer = Comparer<char>.Create((x, y) => -x.CompareTo(y));
        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("b a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('1', '0'), ('2', '0'), ('3', '0') }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("0 1 2 3", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('4', '0'), ('4', '1'), ('1', '3'), ('2', '3'),
                ('5', '0'), ('5', '2')
            }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("0 3 1 2 4 5", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('a', 'b'), ('b', 'c'), ('c', 'd'), ('d', 'Z'),
                ('Z', 'y'), ('d', 'z'), ('d', 'y'), ('d', 'e'),
                ('d', 'f')
            }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("e f y Z z d c b a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { '2' },
            Edges =
            {
                ('0', '1'), ('0', '4'), ('1', '3'), ('1', '5'),
                ('3', '5'), ('3', '7'), ('5', '6'), ('6', '7')
            }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("2 4 7 6 5 3 1 0", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('1', '2'), ('2', '3'), ('2', '4'), ('2', '5'),
                ('2', '6'), ('3', '7'), ('3', '8'), ('4', '9'),
                ('6', '9'), ('7', 'a'), ('7', '9'), ('8', 'a'),
                ('8', '9')
            }
        };

        order = g.OrderTopologicallyInReverse().ThenBy(Fn.Identity);
        Assert.AreEqual("5 9 4 6 a 7 8 3 2 1", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologicallyInReverse().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsException<CircularDependencyException>(() => g.OrderTopologicallyInReverse().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b', 'c', 'd', 'e' },
            Edges = { ('b', 'd') }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().ThenByDescending(Fn.Identity);
        Assert.AreEqual("e c b d a", string.Join(" ", order));

        /***************/

        var vertices = new[] { 'a', 'b', 'c', 'd', 'e' };
        g = new Graph<char>();
        g.Vertices.UnionWith(vertices);

        order = g.OrderTopologicallyInReverse().ThenBy(x => Array.IndexOf(vertices, x));
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologicallyInReverse().ThenBy(x => -Array.IndexOf(vertices, x));
        Assert.AreEqual("e d c b a", string.Join(" ", order));

        /***************/

        var g_001 = new Graph<(int a, int b)>
        {
            Vertices =
            {
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 0), (2, 1), (2, 2), (2, 3),
            },
            Edges = { ((1, 0), (2, 0)) }
        };

        var order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenBy(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("11 12 13 20 10 21 22 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenByDescending(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("20 21 22 23 10 11 12 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenBy(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("13 12 11 23 22 21 20 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenByDescending(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("23 22 21 20 13 12 11 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("10 20 11 21 12 22 13 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenByDescending(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("13 23 12 22 11 21 20 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenBy(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("20 10 21 11 22 12 23 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologicallyInReverse()
            .ThenByDescending(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("23 13 22 12 21 11 20 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));
    }

    [TestMethod]
    public void Graph_OrderTopologically_Proof4Vertices()
    {
        var proof = new TopologicalOrderProof
        {
            GraphSorter = g => g.OrderTopologically(),
            VerticesCount = 4,
            CircularDependenciesEnabled = false
        };
        proof.Run();
    }

    [TestMethod]
    public void Graph_OrderTopologicallyInReverse_Proof4Vertices()
    {
        var proof = new TopologicalOrderProof
        {
            GraphSorter = g => g.GetTransposition().OrderTopologicallyInReverse(),
            VerticesCount = 4,
            CircularDependenciesEnabled = false
        };
        proof.Run();
    }

    [TestMethod]
    public void Graph_OrderTopologically_Proof5Vertices()
    {
        var proof = new TopologicalOrderProof
        {
            GraphSorter = g => g.OrderTopologically(),
            VerticesCount = 5,
            CircularDependenciesEnabled = false
        };
        proof.Run();
    }
}
