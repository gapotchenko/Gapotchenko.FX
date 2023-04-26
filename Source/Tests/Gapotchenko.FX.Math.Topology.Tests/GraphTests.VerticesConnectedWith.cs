using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests;

partial class GraphTests
{
    #region Directed

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Incoming()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        var result = g.IncomingVerticesConnectedWith(2).ToList();
        Assert.AreEqual(4, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 3, 7 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Incoming_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        var result = g.IncomingVerticesConnectedWith(2).ToList();
        Assert.AreEqual(5, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 2, 3, 7 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Outgoing()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 0), (2, 1), (2, 3), (5, 2), (1, 7), (2, 7), (5, 7) }
        };

        var result = g.OutgoingVerticesConnectedWith(2).ToList();
        Assert.AreEqual(4, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 3, 7 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Outgoing_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 0), (2, 1), (2, 3), (5, 2), (1, 7), (2, 7), (5, 7), (2, 2) }
        };

        var result = g.OutgoingVerticesConnectedWith(2).ToList();
        Assert.AreEqual(5, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 2, 3, 7 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5) }
        };

        var result = g.VerticesConnectedWith(2).ToList();
        Assert.AreEqual(4, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 3, 5 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5), (2, 2) }
        };

        var result = g.VerticesConnectedWith(2).ToList();
        Assert.AreEqual(5, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 2, 3, 5 }.SetEquals(result));
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_WithBackReference()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5), (2, 6), (6, 8), (8, 0) }
        };

        var result = g.VerticesConnectedWith(2).ToList();
        Assert.AreEqual(7, result.Count);

        Assert.IsTrue(new HashSet<int> { 0, 1, 2, 3, 5, 6, 8 }.SetEquals(result));
    }

    #endregion

    #region Undirected

    #endregion
}
