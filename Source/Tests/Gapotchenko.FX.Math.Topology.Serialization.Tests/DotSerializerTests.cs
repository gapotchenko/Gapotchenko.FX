using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology.Serialization.Tests
{
    [TestClass]
    public class DotSerializerTests
    {
        [TestMethod]
        public void DotSerializer_RoundtripTest_001()
        {
            var input = @"
digraph G {
  a
}";
            var expected = @"
digraph {
  a
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_002()
        {
            var input = @"
digraph G {
  a [style=filled]
  a -> b
}";
            var expected = @"
digraph {
  a -> b
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_003()
        {
            var input = @"
digraph G {
  a -> b
}";

            var expected = @"
digraph {
  a -> b
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_004()
        {
            var input = @"
digraph G {
  a -> { b c}
}";
            var expected = @"
digraph {
  a -> { b c }
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_005()
        {
            var input = @"
digraph G {
  a -> b -> c
}";
            var expected = @"
digraph {
  a -> b
  b -> c
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_006()
        {
            var input = @"
digraph G {
  n [label=""""]
}";
            var expected = @"
digraph {
  """"
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_007()
        {
            var input = @"
digraph G {
  node [shape=box]
  aaa -> bbb
  aaa -> BBB
  AAA -> BBB
  AAA -> bbb
}";
            var expected = @"
digraph {
  aaa -> { bbb BBB }
  AAA -> { bbb BBB }
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_008()
        {
            var input = @"
digraph automata_0 {
	0;
	2;
	0 -> 2 [ label = ""a "" ];
	0 -> 1 [ label = ""other "" ];
	1 -> 2 [ label = ""a "" ];
	1 -> 1 [ label = ""other "" ];
	2 -> 2 [ label = ""a "" ];
	2 -> 1 [ label = ""other "" ];
	""Machine: a"";
}";
            var expected = @"
digraph {
  0 -> { 2 1 }
  1 -> { 2 1 }
  2 -> { 2 1 }
  ""Machine: a""
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_009()
        {
            var input = @"
graph S {
  1 -- 6;
  2 -- 3 -- 6;
  4 -- 5 -- 6;
}";
            var expected = @"
digraph {
  1 -> 6
  6 -> { 1 3 5 }
  2 -> 3
  3 -> { 6 2 }
  4 -> 5
  5 -> { 6 4 }
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_010()
        {
            var input = @"
graph G {
  a -- b -- c -- a
  a -- B -- C -- a
  a -- 1 -- 2 -- a
}";
            var expected = @"
digraph {
  a -> { b c B C 1 2 }
  b -> { a c }
  c -> { a b }
  B -> { a C }
  C -> { a B }
  1 -> { a 2 }
  2 -> { a 1 }
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_011()
        {
            var input = @"
digraph G {
  subgraph cluster0 {
    a->{c b};
    label = ""cluster0"";
  }
}";
            var expected = @"
digraph {
  a -> { c b }
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_012()
        {
            var input = @"
digraph G {
	subgraph cluster_c0 {a0 -> a1 -> a2 -> a3;}
	subgraph cluster_c1 {b0 -> b1 -> b2 -> b3;}
	x -> a0;
	x -> b0;
	a1 -> a3;
	a3 -> a0;
}
";
            var expected = @"
digraph {
  a0 -> a1
  a1 -> { a2 a3 }
  a2 -> a3
  a3 -> a0
  b0 -> b1
  b1 -> b2
  b2 -> b3
  x -> { a0 b0 }
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_013()
        {
            var input = @"
digraph G {
  a
  a
  b
  b [label=b]
}";
            var expected = @"
digraph {
  a
  b
}";

            ExecuteRoundtripTest(input, expected);
        }

        [TestMethod]
        public void DotSerializer_RoundtripTest_014()
        {
            var input = @"
digraph G {
  { 1 2 } -> { 1 3 4 }
}";
            var expected = @"
digraph {
  1 -> { 1 3 4 }
  2 -> { 1 3 4 }
}";

            ExecuteRoundtripTest(input, expected);
        }

        static void ExecuteRoundtripTest(string inputDocument, string expectedDocument)
        {
            var graph = new Graph<string>();
            var serializer = new DotSerializer();
            serializer.Deserialize(graph, inputDocument);
            var actualDocument = serializer.Serialize(graph);

            var normalizedExpectedDocument = Utilities.NormalizeDotDocument(expectedDocument);
            var normalizedActualDocument = Utilities.NormalizeDotDocument(actualDocument);

            Assert.AreEqual(normalizedExpectedDocument, normalizedActualDocument);
        }

        [TestMethod]
        public void DotSerializer_Test001_DuplicateLabel()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var graph = new Graph<string>(ReferenceEqualityComparer.Instance);
            graph.Vertices.Add("a");
            graph.Vertices.Add(string.Copy("a"));
            graph.Vertices.Add(string.Copy("a"));
            graph.Vertices.Add("v4");
            graph.Vertices.Add(string.Copy("v4"));
#pragma warning restore CS0618 // Type or member is obsolete

            var actual = new DotSerializer().Serialize(graph);

            var expected = @"
digraph {
    a
    v1 [label = a]
    v2 [label = a]
    v4
    v4_1 [label = v4]
}";

            var normalizedExpectedDocument = Utilities.NormalizeDotDocument(expected);
            var normalizedActualDocument = Utilities.NormalizeDotDocument(actual);

            Assert.AreEqual(normalizedExpectedDocument, normalizedActualDocument);
        }

        sealed class DotSerializer_Test002_Class
        {
            public override string ToString() => "Class!";
        }

        [TestMethod]
        public void DotSerializer_Test002_CustomVertexType()
        {
            var graph = new Graph<DotSerializer_Test002_Class>();
            graph.Vertices.Add(new DotSerializer_Test002_Class());
            graph.Vertices.Add(new DotSerializer_Test002_Class());

            var actual = new DotSerializer().Serialize(graph);

            var expected = @"
digraph {
    ""Class!""
    v1 [label = ""Class!""]
}";

            var normalizedExpectedDocument = Utilities.NormalizeDotDocument(expected);
            var normalizedActualDocument = Utilities.NormalizeDotDocument(actual);

            Assert.AreEqual(normalizedExpectedDocument, normalizedActualDocument);
        }

#if NET5_0_OR_GREATER
        [TestMethod]
        public void DotSerializer_Test003_VersionRoundtrip()
        {
            var v1 = new Version(1, 0);
            var v2 = new Version(2, 0, 1);

            var graph = new Graph<Version>();
            graph.Edges.Add(v1, v2);

            var serialized = new DotSerializer().Serialize(graph);

            var expected = @"
digraph {
    1.0 -> ""2.0.1""
}";

            var normalizedExpectedDocument = Utilities.NormalizeDotDocument(expected);
            var normalizedActualDocument = Utilities.NormalizeDotDocument(serialized);

            Assert.AreEqual(normalizedExpectedDocument, normalizedActualDocument);

            graph = new();
            new DotSerializer().Deserialize(graph, serialized);

            var actualVertices = new HashSet<Version>(graph.Vertices);
            var expectedVertices = new HashSet<Version>() { v1, v2 };
            Assert.IsTrue(actualVertices.SetEquals(expectedVertices));
        }
#endif

        sealed class DotSerializer_Test004_Serializer : DotSerializer
        {
            public event EventHandler<object?>? NewVertexOccurred;
            HashSet<string?> _vertices = new();

            protected override T DeserializeVertex<T>(IDotVertex vertex)
            {
                var result = base.DeserializeVertex<T>(vertex);
                if (_vertices.Add(vertex.Identifier))
                {
                    NewVertexOccurred?.Invoke(this, result);
                }
                return result;
            }
        }

        [TestMethod]
        public void DotSerializer_Test004_VerticesOrder()
        {
            var source = @"
digraph {
    a -> b
    c -> d -> e
    f -> { d e g h }
    { i j } -> { k l }
}";

            var serializer = new DotSerializer_Test004_Serializer();
            List<string?> vertices = new();
            serializer.NewVertexOccurred += (o, vertex) => vertices.Add((string?)vertex);
            var graph = new Graph<string>();
            serializer.Deserialize(graph, source);
            var verticesString = string.Join(" ", vertices);
            Assert.AreEqual("a b c d e f g h i j k l", verticesString);
        }
    }
}
