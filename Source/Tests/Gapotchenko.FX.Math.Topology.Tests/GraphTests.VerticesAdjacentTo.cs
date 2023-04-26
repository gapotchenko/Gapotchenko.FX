using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests;

partial class GraphTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_VerticesAdjacentTo(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        var result = g.VerticesAdjacentTo(2).ToList();
        Assert.AreEqual(4, result.Count);

        Assert.IsTrue(new HashSet<int> { 1, 3, 5, 7 }.SetEquals(result));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_VerticesAdjacentTo_WithBuckle(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        var result = g.VerticesAdjacentTo(2).ToList();
        Assert.AreEqual(5, result.Count);

        Assert.IsTrue(new HashSet<int> { 1, 2, 3, 5, 7 }.SetEquals(result));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_VerticesAdjacentTo_WithBackReference(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (5, 2) }
        };

        var result = g.VerticesAdjacentTo(2).ToList();
        Assert.AreEqual(4, result.Count);

        Assert.IsTrue(new HashSet<int> { 1, 3, 5, 7 }.SetEquals(result));
    }

    #region Directed

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Incoming()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        var result = g.IncomingVerticesAdjacentTo(2).ToList();
        Assert.AreEqual(3, result.Count);

        Assert.IsTrue(new HashSet<int> { 1, 3, 7 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Incoming_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        var result = g.IncomingVerticesAdjacentTo(2).ToList();
        Assert.AreEqual(4, result.Count);

        Assert.IsTrue(new HashSet<int> { 1, 2, 3, 7 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Outgoing()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        var result = g.OutgoingVerticesAdjacentTo(2).ToList();
        Assert.AreEqual(1, result.Count);

        Assert.IsTrue(new HashSet<int> { 5 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Outgoing_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        var result = g.OutgoingVerticesAdjacentTo(2).ToList();
        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(new HashSet<int> { 2, 5 }.SetEquals(result));
    }

    #endregion

    #region Undirected


    #endregion
}
