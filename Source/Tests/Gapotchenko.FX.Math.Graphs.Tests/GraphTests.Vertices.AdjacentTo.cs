using Gapotchenko.FX.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Graphs.Tests;

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

        Graph_VerticesAdjacentTo(g, 2, new HashSet<int> { 1, 3, 5, 7 });
    }

    static void Graph_VerticesAdjacentTo<T>(Graph<T> graph, T from, HashSet<T> expectedResult)
    {
        var actualResult = graph.VerticesAdjacentTo(from).ToList();
        Graph_VerticesAdjacentTo(actualResult, expectedResult);

        if (!graph.IsDirected)
        {
            Assert.IsTrue(actualResult.SequenceEqual(graph.IncomingVerticesAdjacentTo(from)));
            Assert.IsTrue(actualResult.SequenceEqual(graph.OutgoingVerticesAdjacentTo(from)));
        }
    }

    static void Graph_VerticesAdjacentTo<T>(IEnumerable<T> actualResult, HashSet<T> expectedResult)
    {
        actualResult = actualResult.Memoize();

        Assert.IsTrue(expectedResult.SetEquals(actualResult));
        Assert.AreEqual(expectedResult.Count, actualResult.Count(), "The result contains duplicates.");
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

        Graph_VerticesAdjacentTo(g, 2, new HashSet<int> { 1, 2, 3, 5, 7 });
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

        Graph_VerticesAdjacentTo(g, 2, new HashSet<int> { 1, 3, 5, 7 });
    }

    #region Directed

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Incoming()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        Graph_VerticesAdjacentTo(
            g.IncomingVerticesAdjacentTo(2),
            new HashSet<int> { 1, 3, 7 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Incoming_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        Graph_VerticesAdjacentTo(
            g.IncomingVerticesAdjacentTo(2),
            new HashSet<int> { 1, 2, 3, 7 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Outgoing()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5) }
        };

        Graph_VerticesAdjacentTo(
            g.OutgoingVerticesAdjacentTo(2),
            new HashSet<int> { 5 });
    }

    [TestMethod]
    public void Graph_Directed_VerticesAdjacentTo_Outgoing_WithBuckle()
    {
        var g = new Graph<int>
        {
            Edges = { (0, 1), (1, 2), (3, 2), (2, 5), (7, 1), (7, 2), (7, 5), (2, 2) }
        };

        Graph_VerticesAdjacentTo(
            g.OutgoingVerticesAdjacentTo(2),
            new HashSet<int> { 2, 5 });
    }

    #endregion
}
