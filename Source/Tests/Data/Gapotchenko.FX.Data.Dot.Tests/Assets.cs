using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Tests
{
    static class Assets
    {
        public static IEnumerable<(string title, string document)> Items
        {
            get
            {
                yield return ("001", @"
digraph G {
  a
}");

                yield return ("002", @"
digraph {
  a
}");

                yield return ("003", @"
digraph G {
  a [style=filled]
  a -> b
}");

                yield return ("004", @"
digraph {
  a -> b
}");

                yield return ("005", @"
digraph G {
  a -> b
}");

                yield return ("006", @"
digraph {
  a -> b
}");

                yield return ("007", @"
digraph G {
  a -> { b c}
}");

                yield return ("008", @"
digraph {
  a -> { b c }
}");

                yield return ("009", @"
digraph G {
  a -> b -> c
}");

                yield return ("010", @"
digraph {
  a -> b
  b -> c
}");

                yield return ("011", @"
digraph G {
  n [label=""""]
}");

                yield return ("012", @"
digraph {
  """"
}");

                yield return ("013", @"
digraph G {
  node [shape=box]
  aaa -> bbb
  aaa -> BBB
  AAA -> BBB
  AAA -> bbb
}");

                yield return ("014", @"
digraph {
  aaa -> { bbb BBB }
  AAA -> { bbb BBB }
}");

                yield return ("015", @"
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

                yield return ("016", @"
digraph {
  0 -> { 2 1 }
  1 -> { 2 1 }
  2 -> { 2 1 }
  ""Machine: a""
}");

                yield return ("017", @"
graph S {
  1 -- 6;
  2 -- 3 -- 6;
  4 -- 5 -- 6;
}");

                yield return ("018", @"
digraph {
  1 -> 6
  6 -> { 1 3 5 }
  2 -> 3
  3 -> { 6 2 }
  4 -> 5
  5 -> { 6 4 }
}");

                yield return ("019", @"
graph G {
  a -- b -- c -- a
  a -- B -- C -- a
  a -- 1 -- 2 -- a
}");

                yield return ("020", @"
digraph {
  a -> { b c B C 1 2 }
  b -> { a c }
  c -> { a b }
  B -> { a C }
  C -> { a B }
  1 -> { a 2 }
  2 -> { a 1 }
}");

                yield return ("021", @"
digraph G {
  subgraph cluster0 {
    a->{c b};
    label = ""cluster0"";
  }
}");

                yield return ("022", @"
digraph {
  a -> { c b }
}");

                yield return ("023", @"
digraph G {
	subgraph cluster_c0 {a0 -> a1 -> a2 -> a3;}
	subgraph cluster_c1 {b0 -> b1 -> b2 -> b3;}
	x -> a0;
	x -> b0;
	a1 -> a3;
	a3 -> a0;
}
");
                yield return ("024", @"
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

                yield return ("025", @"
digraph G {
  a
  a
  b
  b [label=b]
}");

                yield return ("026", @"
digraph {
  a
  b
}");

                yield return ("027", @"
digraph G {
  { 1 2 } -> { 1 3 4 }
}");

                yield return ("028", @"
digraph {
  1 -> { 1 3 4 }
  2 -> { 1 3 4 }
}");
            }
        }
    }
}
