using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    static void Graph_VerticesConnectedWith<T>(IEnumerable<T> actualResult, HashSet<T> expectedResult)
    {
        actualResult = actualResult.Memoize();

        Assert.IsTrue(expectedResult.SetEquals(actualResult));
        Assert.AreEqual(expectedResult.Count, actualResult.Count(), "The result contains duplicates.");
    }

    static void Graph_VerticesConnectedWith<T>(Graph<T> graph, T from, HashSet<T> expectedResult)
    {
        var actualResult = graph.VerticesConnectedWith(from).ToList();
        Graph_VerticesConnectedWith(actualResult, expectedResult);

        if (!graph.IsDirected)
        {
            Assert.IsTrue(actualResult.SequenceEqual(graph.IncomingVerticesConnectedWith(from)));
            Assert.IsTrue(actualResult.SequenceEqual(graph.OutgoingVerticesConnectedWith(from)));
        }
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

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 3, 5 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5), (2, 2) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 2, 3, 5 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesConnectedWith_WithBackReference()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5), (2, 6), (6, 8), (8, 0) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 2, 3, 5, 6, 8 });
    }

    #endregion

    #region Undirected

    [TestMethod]
    public void Graph_Undirected_VerticesConnectedWith()
    {
        var g = new Graph<int>
        {
            IsDirected = false,
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 5) }
        };

        Graph_VerticesConnectedWith(g, 2, new HashSet<int> { 0, 1, 2, 3, 5, 7 });
    }

    #endregion
}
