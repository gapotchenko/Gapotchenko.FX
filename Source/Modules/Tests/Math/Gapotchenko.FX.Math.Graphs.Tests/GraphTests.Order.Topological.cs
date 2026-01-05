using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Math.Graphs.Tests.Engine;

namespace Gapotchenko.FX.Math.Graphs.Tests;

partial class GraphTests
{
    [TestMethod]
    public void Graph_OrderTopologically()
    {
        var g = new Graph<char>();
        IEnumerable<char> order = g.OrderTopologically();
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologically();
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        foreach (var edges in
            Permutations.Of<GraphEdge<char>>([('0', '1'), ('1', '2'), ('2', '3')]))
        {
            g = new();
            g.Edges.UnionWith(edges);

            var topoOrder = g.OrderTopologically();
            Assert.AreEqual("0 1 2 3", string.Join(" ", topoOrder));
        }

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().ToList());
    }

    [TestMethod]
    public void Graph_OrderTopologically_Subsequent()
    {
        var g = new Graph<char>();

        IEnumerable<char> order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b", string.Join(" ", order));

        order = g.OrderTopologically().ThenBy(x => -x);
        Assert.AreEqual("b a", string.Join(" ", order));

        var customComparer = Comparer<char>.Create((x, y) => x.CompareTo(y));
        order = g.OrderTopologically().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("a b", string.Join(" ", order));

        customComparer = Comparer<char>.Create((x, y) => y.CompareTo(x));
        order = g.OrderTopologically().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("b a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('1', '0'), ('2', '0'), ('3', '0') }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("1 2 3 0", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('4', '0'), ('4', '1'), ('1', '3'), ('2', '3'),
                ('5', '0'), ('5', '2')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("4 1 5 0 2 3", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('a', 'b'), ('b', 'c'), ('c', 'd'), ('d', 'Z'),
                ('Z', 'y'), ('d', 'z'), ('d', 'y'), ('d', 'e'),
                ('d', 'f')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b c d Z e f y z", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { '2' },
            Edges =
            {
                ('0', '1'), ('0', '4'), ('1', '3'), ('1', '5'),
                ('3', '5'), ('3', '7'), ('5', '6'), ('6', '7')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("0 1 2 3 4 5 6 7", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('1', '2'), ('2', '3'), ('2', '4'), ('2', '5'),
                ('2', '6'), ('3', '7'), ('3', '8'), ('4', '9'),
                ('6', '9'), ('7', 'a'), ('7', '9'), ('8', 'a'),
                ('8', '9')
            }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("1 2 3 4 5 6 7 8 9 a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b', 'c', 'd', 'e' },
            Edges = { ('b', 'd') }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().ThenByDescending(Fn.Identity);
        Assert.AreEqual("e c b d a", string.Join(" ", order));

        /***************/

        var vertices = new[] { 'a', 'b', 'c', 'd', 'e' };
        g = new Graph<char>();
        g.Vertices.UnionWith(vertices);

        order = g.OrderTopologically().ThenBy(x => Array.IndexOf(vertices, x));
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().ThenBy(x => -Array.IndexOf(vertices, x));
        Assert.AreEqual("e d c b a", string.Join(" ", order));

        /***************/

        var g_001 = new Graph<(int a, int b)>
        {
            Vertices =
            {
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 0), (2, 1), (2, 2), (2, 3),
            },
            Edges = { ((1, 0), (2, 0)) }
        };

        var order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("10 11 12 13 20 21 22 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("21 22 23 10 20 11 12 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("13 12 11 10 23 22 21 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("23 22 21 13 12 11 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("10 20 11 21 12 22 13 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("13 23 12 22 11 21 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("10 20 21 11 22 12 23 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenByDescending(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("23 13 22 12 21 11 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));
    }

    [TestMethod]
    public void Graph_OrderTopologicallyInReverse()
    {
        var g = new Graph<char>();
        IEnumerable<char> order = g.OrderTopologically().Reverse();
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologically().Reverse();
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        foreach (var edges in
            Permutations.Of<GraphEdge<char>>([('0', '1'), ('1', '2'), ('2', '3')]))
        {
            g = new();
            g.Edges.UnionWith(edges);

            var topoOrder = g.OrderTopologically().Reverse();
            Assert.AreEqual("3 2 1 0", string.Join(" ", topoOrder));
        }

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().Reverse().ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().Reverse().ToList());
    }

    [TestMethod]
    public void Graph_OrderTopologicallyInReverse_Subsequent()
    {
        var g = new Graph<char>();

        IEnumerable<char> order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual(0, order.Count());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a' }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b' }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("a b", string.Join(" ", order));

        order = g.OrderTopologically().Reverse().ThenBy(x => -x);
        Assert.AreEqual("b a", string.Join(" ", order));

        var customComparer = Comparer<char>.Create((x, y) => x.CompareTo(y));
        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("a b", string.Join(" ", order));

        customComparer = Comparer<char>.Create((x, y) => y.CompareTo(x));
        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity, customComparer);
        Assert.AreEqual("b a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('1', '0'), ('2', '0'), ('3', '0') }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("0 1 2 3", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('4', '0'), ('4', '1'), ('1', '3'), ('2', '3'),
                ('5', '0'), ('5', '2')
            }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("0 3 1 2 4 5", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('a', 'b'), ('b', 'c'), ('c', 'd'), ('d', 'Z'),
                ('Z', 'y'), ('d', 'z'), ('d', 'y'), ('d', 'e'),
                ('d', 'f')
            }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("e f y Z z d c b a", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Vertices = { '2' },
            Edges =
            {
                ('0', '1'), ('0', '4'), ('1', '3'), ('1', '5'),
                ('3', '5'), ('3', '7'), ('5', '6'), ('6', '7')
            }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("2 4 7 6 5 3 1 0", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges =
            {
                ('1', '2'), ('2', '3'), ('2', '4'), ('2', '5'),
                ('2', '6'), ('3', '7'), ('3', '8'), ('4', '9'),
                ('6', '9'), ('7', 'a'), ('7', '9'), ('8', 'a'),
                ('8', '9')
            }
        };

        order = g.OrderTopologically().Reverse().ThenBy(Fn.Identity);
        Assert.AreEqual("5 9 4 6 a 7 8 3 2 1", string.Join(" ", order));

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().Reverse().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Edges = { ('a', 'b'), ('b', 'c'), ('c', 'a') }
        };

        Assert.ThrowsExactly<GraphCircularReferenceException>(() => g.OrderTopologically().Reverse().ThenBy(Fn.Identity).ToList());

        /***************/

        g = new Graph<char>
        {
            Vertices = { 'a', 'b', 'c', 'd', 'e' },
            Edges = { ('b', 'd') }
        };

        order = g.OrderTopologically().ThenBy(Fn.Identity);
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().ThenByDescending(Fn.Identity);
        Assert.AreEqual("e c b d a", string.Join(" ", order));

        /***************/

        var vertices = new[] { 'a', 'b', 'c', 'd', 'e' };
        g = new Graph<char>();
        g.Vertices.UnionWith(vertices);

        order = g.OrderTopologically().Reverse().ThenBy(x => Array.IndexOf(vertices, x));
        Assert.AreEqual("a b c d e", string.Join(" ", order));

        order = g.OrderTopologically().Reverse().ThenBy(x => -Array.IndexOf(vertices, x));
        Assert.AreEqual("e d c b a", string.Join(" ", order));

        /***************/

        var g_001 = new Graph<(int a, int b)>
        {
            Vertices =
            {
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 0), (2, 1), (2, 2), (2, 3),
            },
            Edges = { ((1, 0), (2, 0)) }
        };

        var order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenBy(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("11 12 13 20 10 21 22 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenByDescending(x => x.a)
            .ThenBy(x => x.b);
        Assert.AreEqual("20 21 22 23 10 11 12 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenBy(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("13 12 11 23 22 21 20 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenByDescending(x => x.a)
            .ThenByDescending(x => x.b);
        Assert.AreEqual("23 22 21 20 13 12 11 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .ThenBy(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("10 20 11 21 12 22 13 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenByDescending(x => x.b)
            .ThenBy(x => x.a);
        Assert.AreEqual("13 23 12 22 11 21 20 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenBy(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("20 10 21 11 22 12 23 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

        order_001 = g_001
            .OrderTopologically()
            .Reverse()
            .ThenByDescending(x => x.b)
            .ThenByDescending(x => x.a);
        Assert.AreEqual("23 13 22 12 21 11 20 10", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));
    }

    [TestMethod]
    public void Graph_OrderTopologically_Proof4Vertices()
    {
        var proof = new TopologicalOrderProof
        {
            GraphSorter = g => g.OrderTopologically(),
            VerticesCount = 4,
            CircularDependenciesEnabled = false
        };
        proof.Run();
    }

    [TestMethod]
    public void Graph_OrderTopologicallyInReverse_Proof4Vertices()
    {
        var proof = new TopologicalOrderProof
        {
            GraphSorter = g => g.GetTransposition().OrderTopologically().Reverse(),
            VerticesCount = 4,
            CircularDependenciesEnabled = false
        };
        proof.Run();
    }

    [TestMethod]
    public void Graph_OrderTopologically_Proof5Vertices()
    {
        var proof = new TopologicalOrderProof
        {
            GraphSorter = g => g.OrderTopologically(),
            VerticesCount = 5,
            CircularDependenciesEnabled = false
        };
        proof.Run();
    }
}
