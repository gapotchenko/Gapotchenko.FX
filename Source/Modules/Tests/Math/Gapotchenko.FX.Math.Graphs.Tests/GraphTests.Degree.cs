// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    public void Graph_GetVertexDegree()
    {
        var g = new Graph<char>();
        Assert.AreEqual(0, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };
        Assert.AreEqual(0, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };
        Assert.AreEqual(0, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        Assert.AreEqual(1, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };
        Assert.AreEqual(2, g.GetVertexDegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'c') }
        };
        Assert.AreEqual(2, g.GetVertexDegree('a'));
        Assert.AreEqual(1, g.GetVertexDegree('b'));
        Assert.AreEqual(1, g.GetVertexDegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a') }
        };
        Assert.AreEqual(2, g.GetVertexDegree('a'));
        Assert.AreEqual(1, g.GetVertexDegree('b'));
        Assert.AreEqual(1, g.GetVertexDegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a'), ('a', 'a') }
        };
        Assert.AreEqual(4, g.GetVertexDegree('a'));
        Assert.AreEqual(1, g.GetVertexDegree('b'));
        Assert.AreEqual(1, g.GetVertexDegree('c'));
    }

    [TestMethod]
    public void Graph_GetVertexIndegree()
    {
        var g = new Graph<char>();
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexIndegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'c') }
        };
        Assert.AreEqual(0, g.GetVertexIndegree('a'));
        Assert.AreEqual(1, g.GetVertexIndegree('b'));
        Assert.AreEqual(1, g.GetVertexIndegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a') }
        };
        Assert.AreEqual(2, g.GetVertexIndegree('a'));
        Assert.AreEqual(0, g.GetVertexIndegree('b'));
        Assert.AreEqual(0, g.GetVertexIndegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a'), ('a', 'a') }
        };
        Assert.AreEqual(3, g.GetVertexIndegree('a'));
        Assert.AreEqual(0, g.GetVertexIndegree('b'));
        Assert.AreEqual(0, g.GetVertexIndegree('c'));
    }

    [TestMethod]
    public void Graph_GetVertexOutdegree()
    {
        var g = new Graph<char>();
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b') }
        };
        Assert.AreEqual(1, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a') }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexOutdegree('a'));

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('a', 'c') }
        };
        Assert.AreEqual(2, g.GetVertexOutdegree('a'));
        Assert.AreEqual(0, g.GetVertexOutdegree('b'));
        Assert.AreEqual(0, g.GetVertexOutdegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a') }
        };
        Assert.AreEqual(0, g.GetVertexOutdegree('a'));
        Assert.AreEqual(1, g.GetVertexOutdegree('b'));
        Assert.AreEqual(1, g.GetVertexOutdegree('c'));

        g = new Graph<char>
        {
            Edges = { ('b', 'a'), ('c', 'a'), ('a', 'a') }
        };
        Assert.AreEqual(1, g.GetVertexOutdegree('a'));
        Assert.AreEqual(1, g.GetVertexOutdegree('b'));
        Assert.AreEqual(1, g.GetVertexOutdegree('c'));
    }
}
