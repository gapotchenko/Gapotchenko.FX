using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests;

partial class GraphTests
{
    #region Directed

    [TestMethod]
    public void Graph_Directed_Path_HasPath()
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
        Assert.IsFalse(g.HasPath(20, 10));
    }

    #endregion

    #region Undirected

    [TestMethod]
    [Ignore("TODO")]
    public void Graph_Undirected_Path_HasPath()
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            IsDirected = false,
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (4, 4) }
        };

        Assert.IsTrue(g.HasPath(1, 2));
        Assert.IsTrue(g.HasPath(2, 1));

        Assert.IsTrue(g.HasPath(1, 3));
        Assert.IsTrue(g.HasPath(3, 1));

        Assert.IsTrue(g.HasPath(4, 4));

        Assert.IsFalse(g.HasPath(1, 5));
        Assert.IsFalse(g.HasPath(5, 1));

        Assert.IsFalse(g.HasPath(1, 10));
        Assert.IsFalse(g.HasPath(10, 1));

        Assert.IsFalse(g.HasPath(10, 20));
        Assert.IsFalse(g.HasPath(20, 10));
    }

    #endregion
}
