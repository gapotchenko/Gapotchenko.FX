namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    public void Graph_IsCyclic_Suite_A()
    {
        IReadOnlyGraph<int> g = new Graph<int>
        {
            Vertices = { 5 },
            Edges = { (1, 2), (2, 3) }
        };

        Assert.IsFalse(g.IsCyclic);

        g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3), (3, 1) }
        };

        Assert.IsTrue(g.IsCyclic);

        g = new Graph<int>
        {
            Edges = { (1, 1) }
        };

        Assert.IsTrue(g.IsCyclic);

        g = new Graph<int>
        {
            Vertices = { 1 }
        };

        Assert.IsFalse(g.IsCyclic);

        g = new Graph<int>();

        Assert.IsFalse(g.IsCyclic);
    }

    [TestMethod]
    public void Graph_IsCyclic_CacheConsistency_A()
    {
        var g = new Graph<int>
        {
            Edges = { (1, 2), (2, 3) }
        };

        Assert.IsFalse(g.IsCyclic);
        Assert.IsFalse(g.IsCyclic);

        g.Edges.Add(3, 1);

        Assert.IsTrue(g.IsCyclic);
        Assert.IsTrue(g.IsCyclic);

        g.Edges.Remove(3, 1);

        Assert.IsFalse(g.IsCyclic);
        Assert.IsFalse(g.IsCyclic);

        var h = g.Clone();
        Assert.IsFalse(h.IsCyclic);
        h.Edges.Add(3, 1);
        Assert.IsTrue(h.IsCyclic);
        h.Edges.Clear();
        Assert.IsFalse(h.IsCyclic);
    }
}
