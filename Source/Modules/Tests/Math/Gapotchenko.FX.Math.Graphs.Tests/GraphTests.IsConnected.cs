﻿namespace Gapotchenko.FX.Math.Graphs.Tests;

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

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_IsConnected_Disconnect(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Edges =
            {
                (1, 2), (3, 4), (2, 3)
            }
        };

        Assert.IsTrue(g.IsConnected);

        g.Vertices.Remove(2);

        Assert.IsFalse(g.IsConnected);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_IsConnected_Empty(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed
        };

        Assert.IsTrue(g.IsConnected);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Graph_IsConnected_Single(bool directed)
    {
        var g = new Graph<int>
        {
            IsDirected = directed,
            Vertices = { 1 }
        };

        Assert.IsTrue(g.IsConnected);
    }
}
