namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    public void Graph_Edges_IncidentWith_Distinct()
    {
        var g = new Graph<int>
        {
            IsDirected = false,
            Edges = { (1, 2) }
        };

        Assert.HasCount(1, g.EdgesIncidentWith(1));
    }
}
