using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    /// <summary>
    /// Represents DOT vertex data.
    /// </summary>
    public sealed class DotVertex : IDotVertex
    {
        static readonly Dictionary<string, string> _noAttributes = new();

        /// <summary>
        /// Initializes a new instance of <see cref="DotVertex"/> class.
        /// </summary>
        /// <param name="identifier">Vertex identifier.</param>
        /// <param name="attributes">Vertex attributes.</param>
        public DotVertex(
            string? identifier,
            IReadOnlyDictionary<string, string>? attributes = default)
        {
            Identifier = identifier;
            Attributes = attributes ?? _noAttributes;
        }

        /// <summary>
        /// Vertex identifier.
        /// </summary>
        public string? Identifier { get; }

        /// <summary>
        /// Vertex attributes.
        /// </summary>
        public IReadOnlyDictionary<string, string> Attributes { get; }
    }
}
