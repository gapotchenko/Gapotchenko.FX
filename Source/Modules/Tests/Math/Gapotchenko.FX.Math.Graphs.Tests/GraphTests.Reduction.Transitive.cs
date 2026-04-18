namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Graph_Reduction_Transitive_PathGraph(bool directed)
    {
        // Path graph 1 -> 2 -> 3: there is no alternative simple path from 1 to 2
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

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Graph_Reduction_Transitive_CycleGraph_1(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (1, 2), (2, 3), (1, 3) }
        };

        var r = g.GetTransitiveReduction();

        Assert.IsTrue(r.Vertices.SetEquals([1, 2, 3]));
        Assert.IsTrue(r.HasPath(1, 2));
        Assert.IsTrue(r.HasPath(2, 3));
        Assert.IsTrue(r.HasPath(1, 3));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Graph_Reduction_Transitive_CycleGraph_2(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges = { (1, 2), (1, 3), (2, 3), (3, 2) }
        };

        var r = g.GetTransitiveReduction();

        Assert.IsTrue(r.Vertices.SetEquals([1, 2, 3]));
        Assert.IsTrue(r.HasPath(1, 2));
        Assert.IsTrue(r.HasPath(1, 3));
        Assert.IsTrue(r.HasPath(2, 3));
        Assert.IsTrue(r.HasPath(3, 2));
    }
}
