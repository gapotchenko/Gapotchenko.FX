using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    public enum DotToken
    {
        EOF = 128,
        DIGRAPH,
        GRAPH,
        ARROW,
        SUBGRAPH,
        NODE,
        EDGE,
        ID,
        LINECOMMENT,
        MLINECOMMENT,
        WHITESPACE,
    }
}
