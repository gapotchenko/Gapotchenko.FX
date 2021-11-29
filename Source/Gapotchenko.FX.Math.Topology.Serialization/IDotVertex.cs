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
    public interface IDotVertex
    {
        /// <summary>
        /// Vertex identifier.
        /// </summary>
        string? Identifier { get; }

        /// <summary>
        /// Vertex attributes.
        /// </summary>
        IReadOnlyDictionary<string, string>? Attributes { get; }
    }
}
