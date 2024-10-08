using Gapotchenko.FX.Math.Graphs.Tests.Engine;

namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    #region Directed

    [TestMethod]
    public void Graph_Directed_Clone()
    {
        var g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (3, 2) }
        };

        var h = g.Clone();

        Assert.AreNotSame(g, h);
        Assert.IsTrue(h.IsDirected);
        Assert.IsTrue(h.Vertices.SetEquals([1, 2, 3, 5]));
        Assert.IsTrue(h.Edges.SetEquals(new[] { (1, 2), (2, 3), (3, 2) }));
    }

    #endregion

    #region Undirected

    [TestMethod]
    public void Graph_Undirected_Clone()
    {
        var g = new Graph<int>
        {
            IsDirected = false,
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (3, 2) }
        };

        var h = g.Clone();

        Assert.AreNotSame(g, h);
        Assert.IsFalse(h.IsDirected);
        Assert.IsTrue(h.Vertices.SetEquals([1, 2, 3, 5]));
        Assert.IsTrue(h.Edges.SetEquals(new[] { (2, 1), (2, 3) }));

        Assert.IsTrue(h.GraphEquals(g));
    }

    #endregion
}
