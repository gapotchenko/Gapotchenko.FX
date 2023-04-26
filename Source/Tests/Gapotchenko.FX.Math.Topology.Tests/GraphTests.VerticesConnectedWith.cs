using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests;

partial class GraphTests
{
    [TestMethod]
    [DataRow(true)]
    public void Graph_VerticesConnectedWith(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 3, 5 });
    }

    static void Graph_VerticesConnectedWith<T>(Graph<T> graph, T from, HashSet<T> expectedResult)
    {
        var actualResult = graph.VerticesConnectedWith(from).ToList();
        Graph_VerticesConnectedWith(actualResult, expectedResult);

        if (!graph.IsDirected)
        {
            //Assert.IsTrue(actualResult.SequenceEqual(graph.IncomingVerticesConnectedWith(from)));
            //Assert.IsTrue(actualResult.SequenceEqual(graph.OutgoingVerticesConnectedWith(from)));
        }
    }

    static void Graph_VerticesConnectedWith<T>(IEnumerable<T> actualResult, HashSet<T> expectedResult)
    {
        actualResult = actualResult.Memoize();

        Assert.IsTrue(expectedResult.SetEquals(actualResult));
        Assert.AreEqual(expectedResult.Count, actualResult.Count(), "The result contains duplicates.");
    }

    [TestMethod]
    [DataRow(true)]
    public void Graph_VerticesConnectedWith_WithBuckle(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5), (2, 2) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 2, 3, 5 });
    }

    [TestMethod]
    [DataRow(true)]
    public void Graph_VerticesConnectedWith_WithBackReference(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5), (2, 6), (6, 8), (8, 0) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 2, 3, 5, 6, 8 });
    }

    #region Directed

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Incoming()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        Graph_VerticesConnectedWith(
            g.IncomingVerticesConnectedWith(2),
            new HashSet<int> { 0, 1, 3, 7 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Incoming_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        Graph_VerticesConnectedWith(
            g.IncomingVerticesConnectedWith(2),
            new HashSet<int> { 0, 1, 2, 3, 7 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Outgoing()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 0), (2, 1), (2, 3), (5, 2), (1, 7), (2, 7), (5, 7) }
        };

        Graph_VerticesConnectedWith(
            g.OutgoingVerticesConnectedWith(2),
            new HashSet<int> { 0, 1, 3, 7 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_Outgoing_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 0), (2, 1), (2, 3), (5, 2), (1, 7), (2, 7), (5, 7), (2, 2) }
        };

        Graph_VerticesConnectedWith(
            g.OutgoingVerticesConnectedWith(2),
            new HashSet<int> { 0, 1, 2, 3, 7 });
    }

    #endregion
}
