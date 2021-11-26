using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    sealed class DotSubgraph
    {
        public string? Identifier { get; }

        List<DotSubgraph>? _subgraphs;

        public IList<DotSubgraph> Subgraphs =>
            _subgraphs ??= new();

        HashSet<IDotVertex>? _vertices;

        public IEnumerable<IDotVertex> Vertices =>
            _vertices ?? Enumerable.Empty<IDotVertex>();

        public DotSubgraph(string? identifier)
        {
            Identifier = identifier;
        }

        public void AddVertex(IDotVertex node) =>
            (_vertices ??= new()).Add(node);
    }
}
