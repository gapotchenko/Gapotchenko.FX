namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Graph_Reduction_Transitive_PathGraph(bool directed)
    {
        // Undirected path graph 1-2-3: there is no alternative simple path from 1 to 2
        // (or from 2 to 3), so transitive reduction must keep both edges.
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (1, 2), (2, 3) }
        };

        var r = g.GetTransitiveReduction();

        Assert.IsTrue(r.Edges.Contains(1, 2));
        Assert.IsTrue(r.Edges.Contains(2, 3));
        Assert.AreEqual(2, r.Edges.Count);
    }
}
