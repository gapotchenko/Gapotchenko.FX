using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_IsConnected(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges =
            {
                (1, 2), (2, 3), (3, 1),
                (4, 5), (5, 6), (6, 7), (7, 8), (8, 4), (5, 8), (6, 8)
            }
        };

        Assert.IsFalse(g.IsConnected);

        g.Edges.Add(2, 4);

        Assert.IsTrue(g.IsConnected);
    }
}
