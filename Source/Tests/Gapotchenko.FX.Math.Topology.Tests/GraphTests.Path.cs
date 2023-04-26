using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Topology.Tests;

partial class GraphTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_Path_HasPath(bool directed)
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            IsDirected = directed,
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3), (4, 4) }
        };

        Assert.IsTrue(g.HasPath(1, 2));
        Assert.AreEqual(!directed, g.HasPath(2, 1));

        Assert.IsTrue(g.HasPath(1, 3));
        Assert.AreEqual(!directed, g.HasPath(3, 1));

        Assert.IsTrue(g.HasPath(4, 4));

        Assert.IsFalse(g.HasPath(1, 5));
        Assert.IsFalse(g.HasPath(5, 1));

        Assert.IsFalse(g.HasPath(1, 10));
        Assert.IsFalse(g.HasPath(10, 1));

        Assert.IsFalse(g.HasPath(10, 20));
        Assert.IsFalse(g.HasPath(20, 10));
    }
}
