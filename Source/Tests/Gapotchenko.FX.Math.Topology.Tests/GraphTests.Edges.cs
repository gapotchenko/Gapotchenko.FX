using Gapotchenko.FX.Math.Topology.Tests.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests;

partial class GraphTests
{
    #region Directed

    [TestMethod]
    public void Graph_Directed_Edges_Uniqueness()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3), (1, 2), (2, 1) }
        };

        Assert.AreEqual(3, g.Vertices.Count);
        Assert.AreEqual(3, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Directed_Edges_Reversibility()
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
    public void Graph_Directed_Edges_Remove()
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
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_Edges_Clear(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
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

    #endregion

    #region Undirected

    [TestMethod]
    public void Graph_Undirected_Edges_Uniqueness()
    {
        var g = new Graph<int>
        {
            IsDirected = false,
            Edges = { (1, 2), (2, 3), (1, 2), (2, 1) },
        };

        Assert.AreEqual(3, g.Vertices.Count);
        Assert.AreEqual(2, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Undirected_Edges_Reversibility()
    {
        var g = new Graph<int>() { IsDirected = false };

        Assert.IsTrue(g.Edges.Add(1, 2));
        Assert.AreEqual(1, g.Edges.Count);

        Assert.IsTrue(g.Edges.Contains(1, 2));
        Assert.IsTrue(g.Edges.Contains(2, 1));

        Assert.IsFalse(g.Edges.Add(2, 1));
        Assert.AreEqual(1, g.Edges.Count);
    }

    [TestMethod]
    public void Graph_Undirected_Edges_Remove()
    {
        var g = new Graph<int>
        {
            IsDirected = false,
            Vertices = { 1, 2 },
            Edges = { (1, 2), (2, 3) }
        };

        Assert.AreEqual(3, g.Vertices.Count);
        Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
        Assert.AreEqual(2, g.Edges.Count);
        Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2), (2, 3) }));
        Assert.IsTrue(g.Edges.SetEquals(new[] { (2, 1), (3, 2) }));

        Assert.IsFalse(g.Edges.Remove(1, 3));
        Assert.AreEqual(3, g.Vertices.Count);
        Assert.AreEqual(2, g.Edges.Count);

        Assert.IsTrue(g.Edges.Remove(3, 2));
        Assert.AreEqual(3, g.Vertices.Count);
        Assert.IsTrue(g.Vertices.SetEquals(new[] { 1, 2, 3 }));
        Assert.AreEqual(1, g.Edges.Count);
        Assert.IsTrue(g.Edges.SetEquals(new[] { (1, 2) }));
        Assert.IsTrue(g.Edges.SetEquals(new[] { (2, 1) }));
    }

    #endregion
}
