using Gapotchenko.FX.Data.Dot.Dom;
using Gapotchenko.FX.Data.Dot.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Gapotchenko.FX.Data.Dot.Tests
{
    [TestClass]
    public class DotRoundtripTests
    {
        [TestMethod]
        public void DotRoundtrip_001() =>
            ExecuteRoundtripTest(@"
digraph G {
  a
}");

        [TestMethod]
        public void DotRoundtrip_002() =>
            ExecuteRoundtripTest(@"
digraph {
  a
}");

        [TestMethod]
        public void DotRoundtrip_003() =>
            ExecuteRoundtripTest(@"
digraph G {
  a [style=filled]
  a -> b
}");

        [TestMethod]
        public void DotRoundtrip_004() =>
            ExecuteRoundtripTest(@"
digraph {
  a -> b
}");

        [TestMethod]
        public void DotRoundtrip_005() =>
            ExecuteRoundtripTest(@"
digraph G {
  a -> b
}");

        [TestMethod]
        public void DotRoundtrip_006() =>
            ExecuteRoundtripTest(@"
digraph {
  a -> b
}");

        [TestMethod]
        public void DotRoundtrip_007() =>
            ExecuteRoundtripTest(@"
digraph G {
  a -> { b c}
}");

        [TestMethod]
        public void DotRoundtrip_008() =>
            ExecuteRoundtripTest(@"
digraph {
  a -> { b c }
}");

        [TestMethod]
        public void DotRoundtrip_009() =>
            ExecuteRoundtripTest(@"
digraph G {
  a -> b -> c
}");

        [TestMethod]
        public void DotRoundtrip_010() =>
            ExecuteRoundtripTest(@"
digraph {
  a -> b
  b -> c
}");

        [TestMethod]
        public void DotRoundtrip_011() =>
            ExecuteRoundtripTest(@"
digraph G {
  n [label=""""]
}");

        [TestMethod]
        public void DotRoundtrip_012() =>
            ExecuteRoundtripTest(@"
digraph {
  """"
}");

        [TestMethod]
        public void DotRoundtrip_013() =>
            ExecuteRoundtripTest(@"
digraph G {
  node [shape=box]
  aaa -> bbb
  aaa -> BBB
  AAA -> BBB
  AAA -> bbb
}");

        [TestMethod]
        public void DotRoundtrip_014() =>
            ExecuteRoundtripTest(@"
digraph {
  aaa -> { bbb BBB }
  AAA -> { bbb BBB }
}");

        [TestMethod]
        public void DotRoundtrip_015() =>
            ExecuteRoundtripTest(@"
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
}");

        [TestMethod]
        public void DotRoundtrip_016() =>
            ExecuteRoundtripTest(@"
digraph {
  0 -> { 2 1 }
  1 -> { 2 1 }
  2 -> { 2 1 }
  ""Machine: a""
}");

        [TestMethod]
        public void DotRoundtrip_017() =>
            ExecuteRoundtripTest(@"
graph S {
  1 -- 6;
  2 -- 3 -- 6;
  4 -- 5 -- 6;
}");

        [TestMethod]
        public void DotRoundtrip_018() =>
            ExecuteRoundtripTest(@"
digraph {
  1 -> 6
  6 -> { 1 3 5 }
  2 -> 3
  3 -> { 6 2 }
  4 -> 5
  5 -> { 6 4 }
}");

        [TestMethod]
        public void DotRoundtrip_019() =>
            ExecuteRoundtripTest(@"
graph G {
  a -- b -- c -- a
  a -- B -- C -- a
  a -- 1 -- 2 -- a
}");

        [TestMethod]
        public void DotRoundtrip_020() =>
            ExecuteRoundtripTest(@"
digraph {
  a -> { b c B C 1 2 }
  b -> { a c }
  c -> { a b }
  B -> { a C }
  C -> { a B }
  1 -> { a 2 }
  2 -> { a 1 }
}");

        [TestMethod]
        public void DotRoundtrip_021() =>
            ExecuteRoundtripTest(@"
digraph G {
  subgraph cluster0 {
    a->{c b};
    label = ""cluster0"";
  }
}");

        [TestMethod]
        public void DotRoundtrip_022() =>
            ExecuteRoundtripTest(@"
digraph {
  a -> { c b }
}");

        [TestMethod]
        public void DotRoundtrip_023() =>
            ExecuteRoundtripTest(@"
digraph G {
	subgraph cluster_c0 {a0 -> a1 -> a2 -> a3;}
	subgraph cluster_c1 {b0 -> b1 -> b2 -> b3;}
	x -> a0;
	x -> b0;
	a1 -> a3;
	a3 -> a0;
}
");
        [TestMethod]
        public void DotRoundtrip_024() =>
            ExecuteRoundtripTest(@"
digraph {
  a0 -> a1
  a1 -> { a2 a3 }
  a2 -> a3
  a3 -> a0
  b0 -> b1
  b1 -> b2
  b2 -> b3
  x -> { a0 b0 }
}");

        [TestMethod]
        public void DotRoundtrip_025() =>
            ExecuteRoundtripTest(@"
digraph G {
  a
  a
  b
  b [label=b]
}");

        [TestMethod]
        public void DotRoundtrip_026() =>
            ExecuteRoundtripTest(@"
digraph {
  a
  b
}");

        [TestMethod]
        public void DotRoundtrip_027() =>
            ExecuteRoundtripTest(@"
digraph G {
  { 1 2 } -> { 1 3 4 }
}");

        [TestMethod]
        public void DotRoundtrip_028() =>
            ExecuteRoundtripTest(@"
digraph {
  1 -> { 1 3 4 }
  2 -> { 1 3 4 }
}");

        [TestMethod]
        public void DotRoundtrip_029() =>
            ExecuteRoundtripTest(@"
digraph {
  ""abra\
cadabra""
  ""abra \"" \\ cadabra""
}");

        [TestMethod]
        public void DotRoundtrip_030() =>
            ExecuteRoundtripTest(@"
digraph {
  <htmlTag1></htmlTag1>
  <htmlTag2>
}");

        [TestMethod]
        public void DotRoundtrip_031() =>
            ExecuteRoundtripTest(@"
digraph {
  aHtmlTable [
   shape=plaintext
   label=<
     <table border='1' cellborder='0' />
  >];
}");

        [TestMethod]
        public void DotRoundtrip_032() =>
            ExecuteRoundtripTest(@"
#directive
digraph {
  // Comment 1
  /* Comment 2 */
  v # Comment 3
}");

        [TestMethod]
        public void DotRoundtrip_033() =>
            ExecuteRoundtripTest(@"
digraph {
  v1 [a;b;c];
  v2 [a;b;c;];
  v3 [a,b,c];
  v4 [a,b,c,];
}");

        [TestMethod]
        public void DotRoundtrip_034() =>
            ExecuteRoundtripTest("digraph Ж {}");

        static void ExecuteRoundtripTest(string document)
        {
            ExecuteLexerRoundtripTest(document);
            ExecuteDomRoundtripTest(document);
        }

        static void ExecuteLexerRoundtripTest(string inputDocument)
        {
            using var textReader = new StringReader(inputDocument);
            using var dotReader = DotReader.Create(textReader);

            using var textWriter = new StringWriter();
            using var dotWriter = DotWriter.Create(textWriter);

            while (dotReader.Read())
            {
                var tok = dotReader.TokenType;
                var val = dotReader.Value ?? string.Empty;

                dotWriter.Write(tok, val);
            }

            var outputDocument = textWriter.ToString();
            Assert.AreEqual(inputDocument, outputDocument);
        }

        static void ExecuteDomRoundtripTest(string inputDocument)
        {
            using var textReader = new StringReader(inputDocument);
            using var dotReader = DotReader.Create(textReader);

            using var textWriter = new StringWriter();
            using var dotWriter = DotWriter.Create(textWriter);

            var dotDocument = DotDocument.Load(dotReader);

            dotDocument.Save(dotWriter);

            var outputDocument = textWriter.ToString();
            Assert.AreEqual(inputDocument, outputDocument);
        }
    }
}
