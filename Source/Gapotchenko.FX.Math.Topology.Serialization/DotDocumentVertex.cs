using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    sealed class DotDocumentVertex : IDotVertex
    {
        public DotDocumentVertex(
            string? identifier,
            int sourceDocumentIndex)
        {
            Identifier = identifier;
            SourceDocumentIndex = sourceDocumentIndex;
        }

        public string? Identifier { get; }

        public int SourceDocumentIndex { get; }

        Dictionary<string, string>? _attributes;

        public IDictionary<string, string> Attributes =>
            _attributes ??= new Dictionary<string, string>();

        public bool HasAttributes =>
            _attributes?.Count > 0;

        static readonly Dictionary<string, string> _noAttributes = new();

        IReadOnlyDictionary<string, string>? IDotVertex.Attributes =>
            _attributes ?? _noAttributes;
    }
}
